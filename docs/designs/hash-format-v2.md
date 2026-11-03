# Persisted hash format v2

## Compatibility boundary

Version 2 defines the hash positions below. Existing unversioned/version 1 bitsets must be rejected by version 2 readers and rebuilt from the original keys. A bitset cannot be rehashed or migrated without those keys. Retain the old library to query old payloads during migration. Do not attach version 2 metadata to existing bits. Enum numeric identities are retained; the persisted format version distinguishes their changed meaning.

This document specifies hashing, not the higher-level numeric/object key encoding contract. Hash inputs are byte sequences. Every multi-byte word consumed by these algorithms is little-endian. UTF-8 is used for the string fixtures. All unsigned arithmetic wraps modulo 2^32, except the explicitly 48-bit LCG. Each built-in ComputeHash rejects null data, nonpositive m, and nonpositive k. Empty data is valid. Repeated positions are permitted, inputs are never modified, and exactly k positions in [0,m) are returned in order.

## Shared definitions

`fmix32(x)` is the Murmur3 final avalanche: `x ^= x >> 16; x *= 0x85ebca6b; x ^= x >> 13; x *= 0xc2b2ae35; x ^= x >> 16`. All shifts are unsigned logical shifts. Position reduction is unsigned `x % m`. The protected rejection helper, unused by v2 built-ins, accepts uint x below `2^32 - (2^32 mod m)` and otherwise returns -1; signed input is reinterpreted as uint, including int.MinValue.

FNV1 starts at 2166136261; for every byte b, multiply by 16777619 then XOR b. FNV1a reverses those two operations. Modified FNV1 uses the FNV1a result followed by `x += x << 13; x ^= x >> 7; x += x << 3; x ^= x >> 17; x += x << 5`.

## LCG methods (IDs 1-3)

Initialize 48-bit state to the corresponding unsigned FNV result. Before each position update `state = (state * 0x5deece66d + 11) & ((1 << 48) - 1)`; output `(state >> 16) % m`. The complete 48-bit state is retained. This replaces the old long-shift mask which limited positions to the first 16384 bits.

## RNG methods (IDs 4-6)

Initialize uint state to the corresponding FNV result. Before each position update `state += 0x9e3779b9`; output `fmix32(state) % m`. This fixed Weyl sequence plus avalanche replaces runtime-dependent System.Random. It is deterministic across .NET versions and platforms.

## Checksum methods (IDs 7-9)

For position index i starting at zero, compute the checksum of `data || LE32(i)`. CRC32 is reflected polynomial 0xedb88320, initial state 0xffffffff and final complement. CRC32u is the nonreflected polynomial 0x04c11db7, initial state 0xffffffff and final complement. Adler32 starts at 1 with modulus 65521. Feed all four counter bytes, including zero bytes. Let c be the unsigned checksum and f be FNV1a of the complete original data (without the counter). Output `fmix32((c XOR f) + 0x9e3779b9 * (i + 1)) % m`. This checksum/FNV hybrid prevents Adler32 structured key collisions from producing identical position sequences. Every iteration completes in bounded time; there are no rejection loops. Empty checksum updates are allowed.

## Murmur methods (IDs 10-12)

Murmur2 uses canonical MurmurHash2 32-bit arithmetic and explicit little-endian word reads. Murmur3 uses canonical MurmurHash3 x86_32 and explicit little-endian word reads. For IDs 10 and 11, position i uses seed `89478583 + i * 0x9e3779b9` and unsigned modulo m. Advancing the seed rather than the input avoids one-byte input cycles and supports empty keys.

For Kirsch-Mitzenmacher (ID 12), `h1 = Murmur3(data, seed=89478583)` and `h2 = Murmur3(data, seed=h1)`. Replace a zero h2 with 0x9e3779b9. Position i is `(h1 + i * h2) % m`, evaluated using nonnegative 64-bit arithmetic. This arithmetic does not wrap for the public int-sized k limit. The nonzero initial seed avoids the old empty-key all-zero result.

## SHA methods (IDs 13-16)

For block indices j starting at zero, independently hash `data || LE32(j)` using SHA-1, SHA-256, SHA-384 or SHA-512. Split each digest into consecutive four-byte little-endian unsigned words, reduce each modulo m, and append until k positions are produced. SHA-1 contributes five positions per block, SHA-256 eight, SHA-384 twelve, SHA-512 sixteen. There is no state shared across calls or blocks.

## Frozen verification and original failures

`M5.BloomFilter.UnitTests/HashVectorTests.cs` freezes eight positions for every ID using UTF-8 `Bloom v2 ☃`, m=1000003. Additional tests validate empty/binary inputs, determinism, preservation, k=1024, m=1, m=int.MaxValue and argument boundaries. Canonical Murmur cores are checked against independent known seed-zero vectors: Murmur2 hello=0xe56129cb; Murmur3 foo=0xf6a5c420 and hello=0x248bfa47.

Before the fix, an isolated executable built from original HEAD sources produced LCGWithFNV1 maximum position 14869 for that fixture and no position above 16383. CRC32, CRC32u and Adler32 each threw ArgumentOutOfRangeException for an empty key. The corrected focused test run passed all 20 cases on the pinned .NET 10.0.403 SDK.

## Checksum hybrid regression

The initial v2 checksum-only derivation produced 1500 false positives in 50000 disjoint structured-key probes for Adler32 at a requested 1% error rate (3%; six-sigma upper bound 1.267%). The corrected derivation includes the full original key through independent FNV1a discrimination; the statistical acceptance threshold is unchanged. The corrected Adler32 result is 466/50000 (0.932%). The combined golden-vector and all-16-method statistical run passes 36/36 tests. Checksum golden vectors were recalculated independently using Python zlib CRC32/Adler32, a bitwise nonreflected CRC32 implementation, and the specified FNV/avalanche arithmetic.
