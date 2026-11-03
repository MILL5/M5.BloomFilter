# Runtime and dependency upgrade evidence

Verified on 2026-11-03 against the official release feeds.

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
