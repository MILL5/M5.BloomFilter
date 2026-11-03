# Using the filter

## Creation and sizing

`FilterBuilder.Build(int expectedElements, double errorRate = 0.01)` is expressed
through overloads; supply a `HashMethod` as the final argument to choose a strategy.
Expected element count must be positive and probability finite, strictly between
zero and one. Oversized requests throw instead of silently clamping capacity.
Allocation can still fail if the process does not have enough memory.

`Statistics` also accepts `(int m, long n)` and `(int m, short k)`. Computed K is
constrained to 1 through Int16.MaxValue. The calculated P is an approximation,
not a measurement or a security guarantee.

## Keys and batches

```csharp
var filter = FilterBuilder.Build(1000, 0.01);
filter.Add("東京");
filter.Add(Array.Empty<byte>());
filter.Add(new List<int> { 1, 2, 3 });
ICollection<bool> results = filter.Contains(new[] { 1, 2, 3, 4 });
bool all = filter.All(new[] { 1, 2, 3 });
```

Batch queries return one Boolean per key, preserving enumeration order. An empty
batch produces zero results and All returns true. Null keys, null collections,
and null elements fail. Input enumerables are materialized into hash positions;
very large batches require memory proportional to key count times K. Chunk them
when needed.

Strings use UTF-8. Numeric primitive encodings are little endian; DateTime uses
ticks without time-zone normalization. Generic batches dispatch primitives through
the same encoder as typed scalar calls. Other objects use invariant-culture text;
provide explicit stable bytes for types without stable textual representations.
Different types can have the same byte representation; use your own type prefix
if the application needs type-separated membership. Collections boxed as object
are rejected rather than hashed by their type name. Byte arrays are single keys;
IEnumerable<byte> is a batch of single-byte keys.

## Snapshots and concurrency

```csharp
var immutable = filter.ToImmutable();
filter.Add("later"); // Does not change immutable.
var exported = immutable.HashBits; // Detached copy; changing it cannot clear bits.
```

Built-in insertion and lookup lock access to mutable bits. A snapshot captures
a consistent bit array under the same lock. Overlapping queries may precede an
insertion; a query after completed insertion sees that insertion. Callers must not
mutate their input byte arrays while hashing them. Custom IBloomFilter and
HashFunction implementations must preserve these invariants themselves.

Async variants do the same synchronous work and return completed tasks. They do
not schedule background threads. ComputeHash returns exactly K in-range positions
for a valid built-in hash strategy.

## Choosing a strategy

Murmur3KirschMitzenmacher is the default. The FNV/LCG, FNV/RNG, CRC32, CRC32u,
Adler32, Murmur2, Murmur3 and SHA families are all available. Version 2 uses a
frozen local RNG and checksum/FNV hybrid expansion; names identify the underlying
families, not an assertion that checksum algorithms are cryptographic hashes.
See [the exact hash format](designs/hash-format-v2.md).

False positives are intrinsic. Do not assert that every noninserted key returns
false. The regression suite checks each strategy at 10000 inserted structured
keys against 50000 disjoint queries with a six-standard-deviation upper tolerance.
This finite deterministic test does not cover every input distribution.
