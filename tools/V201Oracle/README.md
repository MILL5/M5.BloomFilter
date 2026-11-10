# V201Oracle

Writes compatibility fixtures with the **published** M5.BloomFilter 2.0.1
packages from nuget.org, and reads snapshots written by newer code. It is the
independent reference for "2.1 hashes and persists exactly what 2.0.1 did".

It is not in the solution, is never packed, and pins its three package
references to `[2.0.1]`. Do not point it at project references: fixtures written
by the code under test prove nothing.

## Write fixtures

```sh
dotnet run -c Release --project tools/V201Oracle -- <output directory>
```

| File | Contents | Committed under |
|---|---|---|
| `positions.txt` | `method;length;m;positions` for all sixteen strategies, twenty input lengths and three sizes, `k` = 8 | `M5.BloomFilter.UnitTests/Fixtures/v2.0.1/` |
| `keys.txt` | positions for ten strings and thirteen scalars through the public extensions | same |
| `large.*`, `small.*` | bits, core JSON, companion Newtonsoft and System.Text.Json, and protobuf for a 500-key and a 5-key filter | `M5.BloomFilter.Serialization.UnitTests/Fixtures/v2.0.1/` |

`Oracle.Input` and `Oracle.Strings` in `Program.cs` are mirrored in
`CompatibilityTests.cs`; change both together or not at all. The committed
fixtures were generated once from the published packages and should only change
if a new set of inputs is added.

## Read newer snapshots with 2.0.1 code

```sh
M5_ORACLE_OUT=/tmp/m5-current dotnet test M5.BloomFilter.Serialization.UnitTests
dotnet run -c Release --project tools/V201Oracle -- --read /tmp/m5-current
```

Each line reports the keys 2.0.1 could not find; every line must say
`missing=0`, and the process exits non-zero otherwise.
