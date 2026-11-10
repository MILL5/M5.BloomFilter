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
and null elements fail. Input enumerables are materialized into hash positions
before any bit is read or written, so a null element fails before a batch
insertion changes the filter. Very large batches require memory proportional to
key count times K. Chunk them when needed.

Strings use UTF-8. Numeric primitive encodings are little endian; DateTime uses
ticks without time-zone normalization. Generic batches dispatch primitives through
the same encoder as typed scalar calls. Other objects use invariant-culture text;
provide explicit stable bytes for types without stable textual representations.
Different types can have the same byte representation; use your own type prefix
if the application needs type-separated membership. Collections boxed as object
are rejected rather than hashed by their type name. Byte arrays are single keys;
IEnumerable<byte> is a batch of single-byte keys.

## Span keys

```csharp
ReadOnlySpan<char> line = "2026-11-10 customer:42 login";
filter.AddKey(line.Slice(11, 11));              // "customer:42", no substring
bool seen = filter.ContainsKey(line.Slice(11, 11));

Span<byte> id = stackalloc byte[16];
guid.TryWriteBytes(id);
filter.AddKey(id);                               // one key of sixteen bytes
```

`AddKey` and `ContainsKey` take one key as `ReadOnlySpan<byte>`,
`ReadOnlySpan<char>` or `string`. Character keys are encoded as UTF-8, so
`AddKey("x")`, `AddKey("x".AsSpan())` and `Add("x")` set the same bits. The
string overload rejects null. A null string converted to a span is an empty
span, which is the empty key.

They have their own names on purpose. `Add` and `Contains` already have batch
overloads, and `filter.Add(charArray)` means one key per character. A span
overload called `Add` could change which overload that call binds to and turn
it into a single key, which would not match a filter built before. A different
name cannot change what existing code means.

With a Murmur3 strategy (the default) these calls, `Add`/`Contains` for strings
and the numeric overloads do not allocate. Other strategies produce the same
results through the 2.0 code path.

## Occupancy

```csharp
BloomFilterOccupancy now = filter.GetOccupancy();
if (now.EstimatedFalsePositiveRate > 0.02) RebuildLarger();
```

`Statistics` holds the numbers the filter was designed with and does not change.
`GetOccupancy()` counts the bits set at that moment and derives:

- `FillRatio`: set bits divided by total bits. A filter at its design capacity
  is about half full.
- `EstimatedFalsePositiveRate`: `FillRatio` raised to the hash count, the chance
  that a key never added finds all of its bits set.
- `EstimatedCount`: `-(m / k) ln(1 - FillRatio)`, an estimate of distinct keys
  added. It is positive infinity when every bit is set.

These assume the hash spreads keys evenly. They are estimates, and on a filter
that is being written they are a moving reading.

## Combining filters

```csharp
var shards = Enumerable.Range(0, workers)
    .Select(_ => FilterBuilder.Build(expectedTotal, 0.01)).ToArray();
Parallel.For(0, workers, w => { foreach (var key in Partition(w)) shards[w].Add(key); });

var whole = FilterBuilder.Build(expectedTotal, 0.01);
foreach (var shard in shards) whole.UnionWith(shard);
```

Two filters can be combined when they have the same bit count, the same hash
count and the same hash strategy; `IsCompatibleWith` tests that, and `UnionWith`
and `Union` throw `ArgumentException` otherwise. Size every shard for the
expected total, not for its share, so that the sizes match and the result is not
overfilled. The combined filter has exactly the bits a single filter would have
after the same insertions.

`UnionWith` changes a filter created by `FilterBuilder`. `Union` returns a new
`ReadOnlyFilter` and accepts any two implementations. A union's `Statistics`
are the left filter's design values; use `GetOccupancy()` to see what it holds.

## Snapshots and concurrency

```csharp
var immutable = filter.ToImmutable();
filter.Add("later"); // Does not change immutable.
var exported = immutable.HashBits; // Detached copy; changing it cannot clear bits.
```

Lookups take no lock, so any number of threads can query one filter at once.
Insertions, `UnionWith` and snapshot exports share one lock: a snapshot captures
a consistent bit array, the state after some whole number of completed write
calls, and a batch insertion is in a snapshot entirely or not at all. A lookup
that overlaps an insertion may come before it, after it, or see part of it; a
lookup after a completed insertion sees that insertion. Writers do not run in
parallel with each other; to load a filter from several threads, fill one filter
per thread and combine them. Callers must not mutate their input byte arrays
while hashing them. Custom IBloomFilter and HashFunction implementations must
preserve these invariants themselves. A custom HashFunction that returns a
position outside the filter gets an ArgumentOutOfRangeException.

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
