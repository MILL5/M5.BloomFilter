# Releasing version 2.0.0

## Package family

Publish these five active packages together at the same version:

- M5.BloomFilter
- M5.BitArraySerialization.Json
- M5.BitArraySerialization.ProtoBuf
- M5.BloomFilter.Serialization.Json
- M5.BloomFilter.Serialization.ProtoBuf

The unused M5.BloomFilter.Serialization skeleton is not a release package.
All active packages include a README, Apache-2.0 license metadata, and symbols.

## Verification

Use SDK 10.0.403 from global.json. Run:

```sh
dotnet restore M5.BloomFilter.slnx -p:NuGetAuditMode=all -warnaserror:NU1901,NU1902,NU1903,NU1904
dotnet test M5.BloomFilter.slnx
dotnet build M5.BloomFilter.slnx -c Release --no-restore
dotnet test M5.BloomFilter.slnx -c Release --no-build --no-restore
dotnet pack M5.BloomFilter.slnx -c Release --no-build --no-restore -o artifacts/packages
```

CI must pass on Linux and Windows. Inspect the exact PR head and automatic review
findings, resolve material issues, merge, then publish from the merged main commit
or its release tag. Do not manually request reviews or use no-op pushes.

## Authentication

The release workflow supports either a scoped `NUGET_API_KEY` repository secret
or [NuGet Trusted Publishing](https://learn.microsoft.com/en-us/nuget/nuget-org/trusted-publishing)
with repository variable `NUGET_USER` (the NuGet user profile, not an organization
or email). For trusted publishing, configure owner MILL5, repository M5.BloomFilter,
workflow release.yml, no environment. Configure permission for all five package
IDs. No publishing credentials belong in this repository or chat.

Trusted publishing is configured as `NUGET_BLOOMFILTER`, owned by MILL5 and
restricted to this repository, `release.yml`, and the five package IDs above.
Repository variable `NUGET_USER` selects the `richcrane` NuGet profile. The workflow
exchanges GitHub OIDC for a short-lived publishing key; no permanent key is stored.

## Publication

The `Publish NuGet` workflow runs when a GitHub release is published or is manually
dispatched on main. It repeats auditing/tests/pack, uploads reviewable package
artifacts, obtains credentials, then publishes the packages and matching symbols.
`--skip-duplicate` permits retrying a partly completed family release.

After publishing, verify all five version indexes and inspect each .nuspec for
net10.0, version 2.0.0, and matching internal dependencies. NuGet indexing can lag
an accepted push. Confirm the package gallery and a clean restore before claiming
the release is available. Version 2.0.0 is incompatible with old persisted filters;
include [the rebuild instructions](serialization.md) in the GitHub release notes.

GitHub Pages is deployed separately from docs/site and can publish before NuGet.
Its installation copy must accurately distinguish source and published packages.
