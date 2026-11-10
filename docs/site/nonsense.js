/*
 * Makes pronounceable strings that cannot be entries in the word list.
 *
 * Every string contains one of the letter pairs in JOINS. None of those pairs
 * occurs anywhere in any of the 172,823 list entries, so a string that contains
 * one is not a word in the list, and if the filter says "probably" to it, that
 * is a false positive. tools/SiteFilter/check-nonsense.mjs verifies both facts.
 */
(function (root) {
  'use strict';

  var ONSETS = ['b', 'bl', 'br', 'ch', 'cl', 'cr', 'd', 'dr', 'f', 'fl', 'fr', 'g', 'gl', 'gr', 'h', 'j', 'k',
    'l', 'm', 'n', 'p', 'pl', 'pr', 'qu', 'r', 's', 'sc', 'sh', 'sk', 'sl', 'sm', 'sn', 'sp', 'st', 'str', 'sw',
    't', 'th', 'tr', 'tw', 'v', 'w', 'wh', 'y', 'z'];
  var VOWELS = ['a', 'e', 'i', 'o', 'u', 'ai', 'ea', 'ee', 'oo', 'ou', 'io', 'ia'];
  var CODAS = ['', '', 'b', 'ck', 'd', 'ft', 'g', 'k', 'l', 'll', 'm', 'mp', 'n', 'nd', 'ng', 'nk', 'nt',
    'p', 'r', 'rd', 'rk', 'rn', 'rt', 's', 'sh', 'sk', 'sp', 'ss', 'st', 't', 'th', 'x', 'z'];

  // Letter pairs that appear in no entry of the list: the end of one syllable
  // and the start of the next.
  var JOINS = ['xr', 'xk', 'xz', 'vt', 'vm', 'vp', 'vb', 'mx', 'tx', 'sx', 'rx', 'dx', 'kz', 'pz', 'fz', 'zr',
    'jl', 'jr', 'vw', 'jv'];

  // random() returns a number in [0, 1).
  function make(random) {
    function pick(list) { return list[Math.floor(random() * list.length)]; }
    for (;;) {
      var join = pick(JOINS);
      var word = pick(ONSETS) + pick(VOWELS) + join + pick(VOWELS) + pick(CODAS);
      while (word.length < 9) word += pick(ONSETS) + pick(VOWELS) + pick(CODAS);
      if (word.length <= 12) return word;
    }
  }

  var api = { make: make, joins: JOINS };
  if (typeof module === 'object' && module.exports) module.exports = api;
  else root.Nonsense = api;
})(typeof self !== 'undefined' ? self : this);
