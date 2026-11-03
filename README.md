# M5.BloomFilter

A Bloom filter for .NET 10, maintained by [MILL5](https://mill5.com) and
[Richard Crane, Microsoft MVP](https://richcrane.ai).

A lookup returns **definitely absent** or **possibly present**. Confirm positive
results against the original data. Inserted keys remain present; false positives
are allowed. There is no delete operation.

[Website](https://mill5.github.io/M5.BloomFilter/) ·
[Usage](docs/usage.md) · [Serialization and migration](docs/serialization.md) ·
[Design and review](docs/designs/bloom-filter-correctness.md)

## Get started

This repository targets .NET 10 and pins SDK 10.0.403 in `global.json`.
Version 2.0.0 introduces breaking changes; 1.x packages use the older .NET 5
implementation. Rebuild persisted filters before upgrading. Install the current
[NuGet package](https://www.nuget.org/packages/M5.BloomFilter/2.0.0):

```sh
dotnet add package M5.BloomFilter --version 2.0.0
```

```csharp
using M5.BloomFilter;

var filter = FilterBuilder.Build(expectedElements: 100_000, errorRate: 0.01);
filter.Add("customer:42");
bool maybe = filter.Contains("customer:42"); // true: possibly present

filter.Add(new[] { 12, 34, 56 });
bool all = filter.All(new[] { 12, 34, 56 });
var snapshot = filter.ToImmutable();
```

The default hash strategy is `Murmur3KirschMitzenmacher`; sixteen strategies are
available through `HashMethod`. Strings use UTF-8; primitive numbers use
little-endian bytes. Use the same value representation for insertion and lookup.
Empty byte arrays are valid keys; null keys are rejected.

## Guarantees and limits

- `Add` only sets bits; repeating an insertion has no effect.
- Built-in mutation, queries and snapshot exports synchronize access to bits.
  `ToImmutable()` returns a detached snapshot. `HashBits` returns a defensive copy.
- `Statistics.M`, `K`, `N`, and `P` describe bits, hashes, expected elements and
  the ideal independent-hash false-positive estimate at that capacity.
  Actual rates depend on the hash strategy and input distribution; overfilling
  increases them. A requested error rate is not an adversarial-data guarantee.
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
format vectors, false-positive rates, concurrent writers, snapshots, collection
encoding, populated persistence, and malformed bit streams.

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

[Apache-2.0 license](LICENSE).
