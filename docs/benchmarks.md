# Benchmarks: 2.2.0 against 2.0.1

Recorded on 2026-11-11 on .NET 11. Version 2.2.0 runs the same code as 2.1.0.
One machine, one run per version. Read the numbers as the size and direction of
the change, not as what your hardware will do.

| | |
|---|---|
| Machine | Apple M5 Max, 18 cores |
| OS | macOS 27.2 (Darwin 27.2.0) |
| Runtime | .NET 11.0.0, Arm64 RyuJIT |
| SDK | 11.0.100 |
| Harness | BenchmarkDotNet 0.16.0-preview.2, default job |
| Baseline | `M5.BloomFilter` 2.0.1 from nuget.org (its `net10.0` build) |
| Current | this repository at 2.2.0 (its `net11.0` build) |

## Results

Mean time per call and bytes allocated per call.

| Operation | 2.0.1 | 2.2.0 | Faster by | Allocated |
|---|---:|---:|---:|---:|
| `Contains(string)`, key present | 31.08 ns | 21.17 ns | 1.5× | 96 B → 0 |
| `Contains(string)`, key absent | 37.59 ns | 18.46 ns | 2.0× | 95 B → 0 |
| `Contains(long)`, key present | 28.42 ns | 15.74 ns | 1.8× | 112 B → 0 |
| `Contains(byte[])`, key present | 24.85 ns | 16.35 ns | 1.5× | 56 B → 0 |
| snapshot `Contains(string)`, key present | 27.82 ns | 20.56 ns | 1.4× | 96 B → 0 |
| `ContainsKey(ReadOnlySpan<char>)`, key present | n/a | 20.34 ns | n/a | 0 |
| `Contains(string)` on 8 threads, per lookup | 169.6 ns | 3.69 ns | 45.9× | 96 B → 0 |
| `Add(string)`, new keys | 34.95 ns | 31.53 ns | 1.1× | 96 B → 0 |
| `Add(long)`, new keys | 34.25 ns | 25.06 ns | 1.4× | 112 B → 0 |
| `Add(string)`, key already present | 37.52 ns | 23.38 ns | 1.6× | 96 B → 0 |

Standard deviations for the single-thread rows were between 0.2 and 0.8 ns,
except 2.2.0 `Add(string)`, new keys, at 1.9 ns. The 2.0.1 eight-thread row had
a standard deviation of 7.7 ns; the 2.2.0 row 0.02 ns.

The same comparison on .NET 10.0.14, recorded on 2026-11-10 for 2.1.0, is in
this file's history. The ratios there are within 0.3× of these except the
eight-thread row, which was 34.6× (124.5 ns to 3.60 ns).

## What the rows measure

- **Lookups** run against a filter sized for two million keys at a 1% target
  and holding two million (one million strings such as `user:123456` and one
  million `long` values). Present keys cycle through 1,024 of the inserted
  strings; absent keys cycle through 1,024 strings that were never inserted.
- **Eight threads** runs 250,000 lookups of present keys on each of eight
  threads against one shared filter and divides the elapsed time by two million.
  In 2.0.1 every lookup takes the filter's lock, so the threads queue. From
  2.1.0 onward, lookups take no lock.
- **Add, new keys** fills an empty filter sized for one million keys with one
  million distinct keys in each iteration.
- **Add, key already present** adds keys the filter already holds. From 2.1.0 onward, the filter
  notices that no bit would change and does not take the lock.

All rows use the default strategy, `Murmur3KirschMitzenmacher`. The two Murmur3
strategies are the ones with an allocation-free path; the other fourteen run
the same code as 2.0.1 apart from lookups no longer locking.

## Why it changed

- Lookups read the bits without a lock. Insertions and snapshots still share
  one, so a snapshot is still a consistent state.
- Keys are encoded into a stack buffer and hashed there; 2.0.1 allocated a
  `byte[]` for the key and an `int[]` for its positions on every call, and boxed
  numeric keys.
- The default strategy tests each position as it is produced and stops at the
  first clear bit.
- Position `i` is `(h1 + i * h2) mod m`. It is now produced by stepping
  (one addition and a branch-free wrap) instead of a 64-bit division per
  position. The positions are identical; fixtures written by the published
  2.0.1 package pin them.

## Reproduce

```sh
cd benchmarks/M5.BloomFilter.Benchmarks

# 2.2.0 (project reference)
rm -rf bin obj && dotnet run -c Release -- --filter '*'

# 2.0.1 (published package, same source)
rm -rf bin obj && Baseline=true dotnet run -c Release -- --filter '*'
```

The project targets `net11.0` and uses a BenchmarkDotNet preview because 0.15.8,
the latest stable release, does not accept `net11.0` as a target.

`Baseline` is an environment variable, not `-p:Baseline=true`, because
BenchmarkDotNet builds a generated project of its own and only the environment
reaches it. Delete `bin` and `obj` when switching so the two variants do not
share restore state. Keep the machine otherwise idle.

The benchmark project is outside `M5.BloomFilter.slnx` and is not packable.
