/*
 * A JavaScript port of M5.BloomFilter's default strategy,
 * Murmur3KirschMitzenmacher, persisted hash format version 2.
 *
 * Reference: M5.BloomFilter/HashAlgorithms/Murmur3.cs
 * Spec:      docs/designs/hash-format-v2.md
 *
 * positions() returns the same numbers as IBloomFilter.ComputeHash(string) in C#.
 * SiteVectorTests.cs holds the vectors this file is checked against.
 */
(function (root) {
  'use strict';

  var SEED = 89478583;
  var GOLDEN = 0x9e3779b9;
  var encoder = typeof TextEncoder === 'function' ? new TextEncoder() : null;

  // MurmurHash3 x86_32 over a Uint8Array. All arithmetic wraps modulo 2^32.
  function murmur3(seed, data) {
    var c1 = 0xcc9e2d51, c2 = 0x1b873593;
    var hash = seed >>> 0;
    var length = data.length;
    var blocks = length >>> 2;
    var k, i, o;

    for (i = 0; i < blocks; i++) {
      o = i * 4;
      k = data[o] | (data[o + 1] << 8) | (data[o + 2] << 16) | (data[o + 3] << 24);
      k = Math.imul(k, c1);
      k = (k << 15) | (k >>> 17);
      k = Math.imul(k, c2);
      hash ^= k;
      hash = (hash << 13) | (hash >>> 19);
      hash = (Math.imul(hash, 5) + 0xe6546b64) | 0;
    }

    k = 0;
    o = blocks * 4;
    switch (length & 3) {
      case 3: k ^= data[o + 2] << 16; // falls through
      case 2: k ^= data[o + 1] << 8;  // falls through
      case 1:
        k ^= data[o];
        k = Math.imul(k, c1);
        k = (k << 15) | (k >>> 17);
        k = Math.imul(k, c2);
        hash ^= k;
    }

    hash ^= length;
    hash ^= hash >>> 16;
    hash = Math.imul(hash, 0x85ebca6b);
    hash ^= hash >>> 13;
    hash = Math.imul(hash, 0xc2b2ae35);
    hash ^= hash >>> 16;
    return hash >>> 0;
  }

  // Strings are hashed as UTF-8, exactly as Add(string) does in C#.
  function utf8(text) {
    return encoder.encode(text);
  }

  // h1 = murmur(data, seed), h2 = murmur(data, h1); position i = (h1 + i * h2) mod m.
  // h1 + i * h2 stays below 2^47 for any k the library allows, so doubles are exact.
  function positions(text, m, k) {
    var data = utf8(text);
    var h1 = murmur3(SEED, data);
    var h2 = murmur3(h1, data);
    if (h2 === 0) h2 = GOLDEN;
    var out = new Array(k);
    for (var i = 0; i < k; i++) out[i] = (h1 + i * h2) % m;
    return { h1: h1, h2: h2, positions: out, bytes: data.length };
  }

  // Loads a snapshot written by BloomFilterJsonConverter:
  // {"hb":{"b":"<base64>","l":bits},"hm":12,"m":bits,"k":hashes,"n":keys,"p":estimate,"v":2}
  // Bit i is stored in byte i / 8 at bit i % 8.
  function load(snapshot) {
    if (!snapshot || snapshot.v !== 2) throw new Error('Unsupported filter format; expected version 2.');
    if (snapshot.hm !== 12) throw new Error('This port implements hash method 12 only.');
    var binary = atob(snapshot.hb.b);
    var bytes = new Uint8Array(binary.length);
    for (var i = 0; i < binary.length; i++) bytes[i] = binary.charCodeAt(i);
    if (snapshot.hb.l !== snapshot.m || bytes.length !== Math.ceil(snapshot.m / 8)) {
      throw new Error('Bit data does not match the declared length.');
    }
    return new Filter(bytes, snapshot.m, snapshot.k, snapshot.n, snapshot.p);
  }

  function Filter(bytes, m, k, n, p) {
    this.bytes = bytes;
    this.m = m;
    this.k = k;
    this.n = n;
    this.p = p;
  }

  Filter.prototype.bit = function (position) {
    return (this.bytes[position >>> 3] >>> (position & 7)) & 1;
  };

  // Every probe, in order, with whether its bit is set.
  Filter.prototype.probe = function (text) {
    var hashed = positions(text, this.m, this.k);
    var set = new Array(this.k);
    var firstBlank = -1;
    for (var i = 0; i < this.k; i++) {
      set[i] = this.bit(hashed.positions[i]) === 1;
      if (!set[i] && firstBlank < 0) firstBlank = i;
    }
    hashed.set = set;
    hashed.firstBlank = firstBlank;
    hashed.present = firstBlank < 0;
    return hashed;
  };

  // The same early-exit test Contains performs.
  Filter.prototype.contains = function (text) {
    var hashed = positions(text, this.m, this.k);
    for (var i = 0; i < this.k; i++) {
      if (this.bit(hashed.positions[i]) === 0) return false;
    }
    return true;
  };

  Filter.prototype.countSetBits = function () {
    var count = 0, bytes = this.bytes;
    for (var i = 0; i < bytes.length; i++) {
      var b = bytes[i];
      b = b - ((b >> 1) & 0x55);
      b = (b & 0x33) + ((b >> 2) & 0x33);
      count += (b + (b >> 4)) & 0x0f;
    }
    return count;
  };

  // Statistics(long n, double p) from M5.BloomFilter/Statistics.cs.
  var INT_MAX = 2147483647;
  var SHORT_MAX = 32767;

  function bestP(k, m, n) {
    return Math.pow(1 - Math.exp(-k * n / m), k);
  }

  function bestK(n, m) {
    var ideal = Math.LN2 * m / n;
    var lower = Math.min(Math.max(Math.floor(ideal), 1), SHORT_MAX);
    var upper = Math.min(Math.max(Math.ceil(ideal), 1), SHORT_MAX);
    return bestP(lower, m, n) <= bestP(upper, m, n) ? lower : upper;
  }

  function checkedBits(bits) {
    if (!isFinite(bits) || bits > INT_MAX) return null;
    return Math.max(1, Math.trunc(bits));
  }

  // Returns { m, k, p } or null when the library would throw (more than Int32 bits).
  function size(n, p) {
    if (!(n >= 1) || !(p > 0) || !(p < 1)) return null;
    var m = checkedBits(Math.ceil(-n * Math.log(p) / (Math.LN2 * Math.LN2)));
    if (m === null) return null;
    var k = bestK(n, m);
    if (bestP(k, m, n) > p) {
      var bits = Math.ceil(-k * n / Math.log(1 - Math.pow(p, 1 / k)));
      m = checkedBits(Math.max(m, bits));
      if (m === null) return null;
      k = bestK(n, m);
    }
    return { m: m, k: k, p: bestP(k, m, n) };
  }

  var api = {
    murmur3: murmur3,
    utf8: utf8,
    positions: positions,
    load: load,
    size: size,
    falsePositiveRate: bestP
  };

  if (typeof module === 'object' && module.exports) module.exports = api;
  else root.Bloom = api;
})(typeof self !== 'undefined' ? self : this);
