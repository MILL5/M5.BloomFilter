# Runtime and dependency upgrade evidence

Verified on 2026-11-03 against the official release feeds. The .NET 11 update
below was verified on 2026-11-11.

## .NET 11 update

.NET 11.0.0 shipped on 2026-11-10 with SDK `11.0.100`, per the
[official .NET 11 release metadata](https://builds.dotnet.microsoft.com/dotnet/release-metadata/11.0/releases.json).
It is a standard-term (STS) release supported until 2028-11-14; .NET 10 remains
the LTS channel.

Decision: add .NET 11 without dropping .NET 10. The six library projects and
four test projects target `net10.0;net11.0`; the five packages ship a `lib`
folder and a dependency group for each. `global.json` pins SDK `11.0.100` and
still excludes prerelease SDKs. CI installs the .NET 10 runtime (`10.0.x`)
beside the pinned SDK so the `net10.0` tests run. The package version is
**2.2.0**: a new target framework with no API, hashing, key encoding or
persisted format change. Retargeting to `net11.0` alone would have removed LTS
consumers for no gain, because no source file needs a .NET 11 API.

.NET 11 adds `BitArray.PopCount()` and span constructors for `BitArray`. The
library counts bits over its own word storage and builds exports through
`CollectionsMarshal.AsBytes`, so neither is adopted and both targets compile
the same source. None of the
[documented .NET 11 breaking changes](https://learn.microsoft.com/dotnet/core/compatibility/11)
touches an API the library calls.

`tools/SiteFilter`, `tools/V201Oracle` and
`benchmarks/M5.BloomFilter.Benchmarks` target `net11.0`. The benchmark project
uses BenchmarkDotNet `0.16.0-preview.2`: 0.15.8, the latest stable release,
does not recognise `net11.0` as a target and fails validation. It is the only
prerelease package in the repository and is not referenced by any shipped
package; see [the benchmark record](../benchmarks.md).

Package changes, each the last non-prerelease entry in NuGet's flat-container
index on 2026-11-11:

| Package | Previous | Updated |
| --- | --- | --- |
| Microsoft.NET.Test.Sdk | 18.11.0 | 18.12.0 |
| MSTest.TestAdapter | 4.5.2 | 4.5.3 |
| MSTest.TestFramework | 4.5.2 | 4.5.3 |
| BenchmarkDotNet (benchmarks only) | 0.15.8 | 0.16.0-preview.2 |
| Pineapple | 1.0.41 | 2.0.0 |

Pineapple 2.0.0 targets .NET 8 to 11 and adds `Microsoft.Extensions.Http` (and,
on .NET 10, `Microsoft.Extensions.Logging.Abstractions`) to the packages'
transitive dependencies. The library uses only `Pineapple.Common.Preconditions`;
the build raises none of the PINE001 to PINE008 migration diagnostics.

Verification with SDK 11.0.100, runtimes 10.0.14 and 11.0.0, macOS ARM64:
audited restore passed; Release build passed with zero warnings and zero
errors; **152 tests passed on each target framework** (117 core, 22 BitArray
JSON, 3 BitArray protobuf, 10 filter serialization) in Debug and Release,
including the fixtures written by the published 2.0.1 packages. No direct
package is outdated and no direct or transitive package has a known
vulnerability. Release packaging produced the five `.nupkg` and `.snupkg`
files at 2.2.0, each with `lib/net10.0` and `lib/net11.0`.

The sections below record the 2026-11-03 move to .NET 10 and are unchanged.

## Runtime

All ten projects target `net10.0`. `global.json` pins stable SDK `10.0.403`, permits servicing patches in that feature band, and excludes prerelease SDKs. The [official .NET 10 release metadata](https://dotnetcli.blob.core.windows.net/dotnet/release-metadata/10.0/releases.json) reports SDK `10.0.403` and runtime `10.0.14` as the latest stable releases. .NET 10 is the current LTS channel; .NET 11 remains prerelease at verification time.

For this workspace, the SDK was installed using the [official installer](https://dot.net/v1/dotnet-install.sh) into `/tmp/m5-bloomfilter-dotnet-10.0.403`, then installed side by side in the user's standard .dotnet directory using the official installer. No shell profiles were changed; ordinary `dotnet --version` now resolves 10.0.403 in this repository.

## Direct packages

Each version is the last non-prerelease entry in NuGet's official flat-container index at verification time.

| Package | Previous | Updated | Official version feed |
| --- | --- | --- | --- |
| Microsoft.NET.Test.Sdk | 16.11.0 | 18.11.0 | [NuGet](https://api.nuget.org/v3-flatcontainer/microsoft.net.test.sdk/index.json) |
| MSTest.TestAdapter | 2.2.6 | 4.5.2 | [NuGet](https://api.nuget.org/v3-flatcontainer/mstest.testadapter/index.json) |
| MSTest.TestFramework | 2.2.6 | 4.5.2 | [NuGet](https://api.nuget.org/v3-flatcontainer/mstest.testframework/index.json) |
| coverlet.collector | 3.1.0 | 10.1.0 | [NuGet](https://api.nuget.org/v3-flatcontainer/coverlet.collector/index.json) |
| Newtonsoft.Json | 13.0.1 | 13.0.4 | [NuGet](https://api.nuget.org/v3-flatcontainer/newtonsoft.json/index.json) |
| Pineapple | 1.0.35 | 1.0.41 | [NuGet](https://api.nuget.org/v3-flatcontainer/pineapple/index.json) |
| Shouldly | 4.0.3 | 4.3.0 | [NuGet](https://api.nuget.org/v3-flatcontainer/shouldly/index.json) |
| protobuf-net | protobuf-net.Grpc 1.0.152 | 3.4.40 | [NuGet](https://api.nuget.org/v3-flatcontainer/protobuf-net/index.json) |

The code uses protobuf serialization APIs only. Replacing `protobuf-net.Grpc` with `protobuf-net` directly removes the unused gRPC package tree. The BitArray protobuf test project was already listed in the solution; it now references its implementation project so real serializer tests can compile.

## Verification

```sh
dotnet restore M5.BloomFilter.slnx -p:NuGetAuditMode=all -warnaserror:NU1901,NU1902,NU1903,NU1904
dotnet test M5.BloomFilter.slnx
dotnet build M5.BloomFilter.slnx -c Release --no-restore
dotnet test M5.BloomFilter.slnx -c Release --no-build --no-restore --blame-hang-timeout 60s
dotnet pack M5.BloomFilter.slnx -c Release --no-build --no-restore -o artifacts/packages
for project in */*.csproj; do
  dotnet list "$project" package --outdated
  dotnet list "$project" package --vulnerable --include-transitive
done
```

Restore succeeded with SDK 10.0.403. All ten projects report no outdated direct packages and no known vulnerable direct or transitive packages. Package inventory runs per project because the solution contains a legacy Visual Studio shared project whose `CodeSharing` import is unsupported by the CLI inventory command.

MSTest 4 removes `ExpectedException`: the two converter-negative tests now assert the exact deserialization exceptions. `DataTestMethod` attributes use `TestMethod` instead.

Final integrated verification with SDK 10.0.403 and runtime 10.0.14: audited restore passed; Release build passed with zero warnings and zero errors; all **117 tests passed** (86 core, 22 BitArray JSON, 3 BitArray protobuf, 6 filter serialization). Release packaging produced the five active libraries' `.nupkg` and `.snupkg` files at version **2.0.0**. NuGet emitted readme recommendations during pack; packages were created successfully. The unused serialization skeleton also declares 2.0.0 but is excluded from the solution and release pack. The major version reflects the .NET 10 target and intentionally incompatible version 2 hashing/encoding contract; persisted filters must be rebuilt from original keys.

The legacy ARM64 target-platform override and major runtime roll-forward are no longer needed. Local verification ran on macOS ARM64; Linux and Windows execution will be established by CI.

## Continuous integration

`.github/workflows/ci.yml` restores with transitive NuGet auditing, fails on known vulnerability warnings, builds and tests Debug and Release on Ubuntu and Windows, and verifies Release packages. `.github/workflows/pages.yml` verifies Release tests before deployment. Both workflows use `global.json` and immutable action commit pins. `actionlint` passed for both workflows.

Latest stable action releases and their commit SHAs were verified through the official repositories' GitHub release and tag APIs on 2026-11-03:

| Action | Release | Pinned commit |
| --- | --- | --- |
| [actions/checkout](https://github.com/actions/checkout/releases/tag/v7.0.1) | v7.0.1 | `3d3c42e5aac5ba805825da76410c181273ba90b1` |
| [actions/setup-dotnet](https://github.com/actions/setup-dotnet/releases/tag/v6.1.0) | v6.1.0 | `0f4a354dd5547a28c70b7853dbd95d5b67886e73` |
| [actions/configure-pages](https://github.com/actions/configure-pages/releases/tag/v6.0.0) | v6.0.0 | `45bfe0192ca1faeb007ade9deae92b16b8254a0d` |
| [actions/upload-pages-artifact](https://github.com/actions/upload-pages-artifact/releases/tag/v5.0.0) | v5.0.0 | `fc324d3547104276b827a68afc52ff2a11cc49c9` |
| [actions/deploy-pages](https://github.com/actions/deploy-pages/releases/tag/v5.0.1) | v5.0.1 | `368f82528645a54fb793d4d04e342629a3f51346` |
