# M5.BloomFilter

A Bloom filter for .NET 10, maintained by [MILL5](https://mill5.com) and
[Richard Crane, Microsoft MVP](https://richcrane.ai).

A lookup returns **definitely absent** or **possibly present**. Confirm positive
results against the original data. Inserted keys remain present; false positives
are allowed. There is no delete operation.

[Website](https://mill5.github.io/M5.BloomFilter/) ·
[Usage](docs/usage.md) · [Serialization and migration](docs/serialization.md) ·
[Benchmarks](docs/benchmarks.md) ·
[Design and review](docs/designs/v2-1-lock-free-and-page.md)

## Get started

This repository targets .NET 10 and pins SDK 10.0.403 in `global.json`.
Install the current
[NuGet package](https://www.nuget.org/packages/M5.BloomFilter/2.1.0):

```sh
dotnet add package M5.BloomFilter --version 2.1.0
```

Moving from 2.0.x needs no code or data changes. Moving from 1.x does: version 2
changed hashing, so [rebuild persisted filters](#version-2-migration) first.

```csharp
using M5.BloomFilter;

var filter = FilterBuilder.Build(expectedElements: 100_000, errorRate: 0.01);
filter.Add("customer:42");
bool maybe = filter.Contains("customer:42"); // true: possibly present

filter.Add(new[] { 12, 34, 56 });
bool all = filter.All(new[] { 12, 34, 56 });
var snapshot = filter.ToImmutable();
```

## New in 2.1

```csharp
// Lookups take no lock, and string, numeric and span keys are hashed without
// allocating when the filter uses a Murmur3 strategy (the default).
bool maybe = filter.Contains(42L);
bool sliced = filter.ContainsKey(line.AsSpan(0, 11));   // one key from part of a buffer
filter.AddKey(utf8Bytes);                               // ReadOnlySpan<byte>

// How full is it, really?
BloomFilterOccupancy now = filter.GetOccupancy();
double fill = now.FillRatio;                    // set bits / total bits
double falsePositives = now.EstimatedFalsePositiveRate;
double keys = now.EstimatedCount;

// Fill one filter per worker, then combine them.
whole.UnionWith(shard);                         // same size, hash count and strategy
ReadOnlyFilter merged = snapshotA.Union(snapshotB);
```

Hashing, key encoding and the persisted format are unchanged: 2.0.x and 2.1.0
read each other's snapshots, and the test suite checks both directions against
fixtures written by the published 2.0.1 packages. Measured results are in
[docs/benchmarks.md](docs/benchmarks.md).

The default hash strategy is `Murmur3KirschMitzenmacher`; sixteen strategies are
available through `HashMethod`. Strings use UTF-8; primitive numbers use
little-endian bytes. Use the same value representation for insertion and lookup.
Empty byte arrays are valid keys; null keys are rejected.

## Guarantees and limits

- `Add` only sets bits; repeating an insertion has no effect.
- Lookups are lock-free and see every insertion that has completed. Insertions
  and snapshot exports share one lock, so a snapshot is always a consistent state.
  `ToImmutable()` returns a detached snapshot. `HashBits` returns a defensive copy.
- `Statistics.M`, `K`, `N`, and `P` describe bits, hashes, expected elements and
  the ideal independent-hash false-positive estimate at that capacity.
  Actual rates depend on the hash strategy and input distribution; overfilling
  increases them. A requested error rate is not an adversarial-data guarantee.
- `Statistics` describes the design. `GetOccupancy()` measures the bits set now
  and estimates the key count and false-positive rate from them.
- `UnionWith` and `Union` require equal bit count, hash count and hash strategy
  and throw otherwise. A union keeps every key of both filters.
- Async methods wrap synchronous in-memory work in completed tasks.
- Filters are not an exact set, authorization check, or replacement for stored keys.

## Build and test

```sh
dotnet restore M5.BloomFilter.slnx
dotnet test M5.BloomFilter.slnx
dotnet build M5.BloomFilter.slnx -c Release
dotnet pack M5.BloomFilter.slnx -c Release --no-build
```

The SDK migration preserves all ten solution projects (including the shared
project), both solution folders, and Debug/Release with Any CPU. The unused
`M5.BloomFilter.Serialization` skeleton remains outside the solution as before.
Regression tests cover all hash strategies, independent Murmur vectors, frozen
format vectors, false-positive rates, concurrent readers and writers, snapshots,
collection encoding, populated persistence, and malformed bit streams.

Two tool projects sit outside the solution, are never packed, and are run by
hand: `tools/V201Oracle` writes the compatibility fixtures from the published
2.0.1 packages, and `benchmarks/M5.BloomFilter.Benchmarks` produces
[docs/benchmarks.md](docs/benchmarks.md).

## Version 2 migration

Hash and numeric encoding corrections change bit positions. JSON/protobuf readers
require format version 2 and reject older or unversioned filter payloads.
**Rebuild from original keys. Never relabel an old bitset as version 2.** Upgrade
all readers together and store old/new payloads separately during migration.
Old binaries ignore new fields and cannot be trusted to read version 2 data.

Generic scalar extensions now accept `object`; generic `IEnumerable<T>` overloads
handle batches. Replace explicit scalar calls such as `Add<int>(42)` with `Add(42)`.
See [the migration guide](docs/serialization.md) for serializer examples.

## Development workflow

The complete [Google Elephant–Goldfish Model](https://github.com/google/elephant-goldfish-model)
runtime is installed under `.agents/`, pinned in `.agents/egm-source.json`.
Read [AGENTS.md](AGENTS.md); draft in `docs/designs/`, review in a fresh Goldfish
context, resolve material findings, implement, and run tests. The workflow skills
are `egm-brainstorm`, `egm-draft`, `egm-review`, `egm-resolve`, `egm-review-loop`,
`egm-drift-report`, and `egm-bootstrap`.

GitHub Actions tests the solution on Linux and Windows and deploys only `docs/site`
to GitHub Pages. Design/review artifacts are excluded from the website deployment.
Package release steps are in [the release guide](docs/releasing.md).

## License

Versions 2.0.1 and later use [PolyForm Strict 1.0.0](LICENSE), with copyright retained by
MILL5, LLC. This is source-available software; the license permits the uses it
expressly describes. Uses outside those terms require separate authorization
from MILL5. Read the full license before using the software.

The license change does not alter version 2 hashing, stored payloads, APIs, or
.NET requirements. Version 2.0.0 and earlier distributions retain their original
Apache-2.0 license. Bundled third-party materials retain their own licenses;
see [THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md).
