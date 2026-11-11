# Releasing version 2.2.0

## Package family

Publish these five active packages together at the same version:

- M5.BloomFilter
- M5.BitArraySerialization.Json
- M5.BitArraySerialization.ProtoBuf
- M5.BloomFilter.Serialization.Json
- M5.BloomFilter.Serialization.ProtoBuf

The unused M5.BloomFilter.Serialization skeleton is not a release package.
All active packages include a README, the complete PolyForm Strict 1.0.0 LICENSE file, and symbols.

## Branch workflow

`develop` is the default development branch. Create feature branches from its
latest commit and open pull requests back to `develop`. GitHub Copilot reviews
ready PRs to `develop` automatically, including subsequent pushes. Inspect and
resolve review findings without manually requesting another review.

Both `develop` and `main` require pull requests, all four Linux/Windows
Debug/Release CI checks, an up-to-date base, and resolved review conversations.
Direct pushes, force pushes, and branch deletion are blocked, including for
administrators. Human approval is optional for this solo-maintainer repository;
automatic review does not replace required CI checks.

Promote a tested `develop` to `main` through a merge PR. The required
`Promotion from develop` check rejects any other source branch or fork.
Use merge commits to preserve ancestry, retain `develop` after promotion, and
merge `main` back into `develop` through a PR when synchronization is needed.
Pages deployment and NuGet publication continue to use `main` and release tags.

## Verification

Use SDK 11.0.100 from global.json. The tests run on net10.0 and net11.0, so the
.NET 10 runtime must be installed beside it. Run:

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

Before publishing a version that touches hashing, storage or serialization, check
that the published 2.0.1 code can read its output:

```sh
M5_ORACLE_OUT=/tmp/m5-current dotnet test M5.BloomFilter.Serialization.UnitTests
dotnet run -c Release --project tools/V201Oracle -- --read /tmp/m5-current
```

Every line must report `missing=0`. The fixtures under `Fixtures/v2.0.1/` were
written by the published packages; never regenerate them from current source.

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

Create the release on the merged commit, for example
`gh release create v2.2.0 --target <main SHA>`; the tag must point at a commit on
`main`. After publishing, verify all five version indexes and inspect each .nuspec for
net10.0 and net11.0 dependency groups, version 2.2.0, an embedded LICENSE file matching the repository, and matching internal dependencies. NuGet indexing can lag
an accepted push. Confirm the package gallery and a clean restore before claiming
the release is available. Version 2.0.0 is incompatible with old persisted filters;
include [the rebuild instructions](serialization.md) in the GitHub release notes.

GitHub Pages is deployed separately from docs/site and can publish before NuGet.
Its installation copy must accurately distinguish source and published packages.
When a release changes what the page says about the package, merge the page
change after the packages are verified on nuget.org.

## Version 2.2.0

Adds a `net11.0` build beside `net10.0` in all five packages and moves the
Pineapple dependency from 1.0.41 to 2.0.0. No API, hashing, key encoding or
persisted format changes, so nothing needs rebuilding when moving between
2.0.x, 2.1.0 and 2.2.0 in either direction. .NET 10 remains supported: it is
the LTS release, and .NET 11 is a standard-term release. See
[the runtime and dependency record](designs/dependencies.md).

## Version 2.1.0

A feature release with no breaking changes. Hashing, key encoding and persisted
format version 2 are unchanged, so nothing needs rebuilding when moving between
2.0.x and 2.1.0 in either direction. Lookups no longer take the filter's lock;
insertions and snapshots still do. See
[the design and review record](designs/v2-1-lock-free-and-page.md) and
[the recorded benchmarks](benchmarks.md).

## License-only patch

Version 2.0.1 adopts PolyForm Strict 1.0.0 with MILL5 copyright. Its hash format
remains version 2 and is compatible with 2.0.0 payloads; no additional rebuild is
needed when moving from 2.0.0 to 2.0.1. Earlier published package files retain
their original license. NuGet package versions cannot be replaced, so the change
ships in a new patch release. Leave third-party Apache/OFL licenses unchanged.
