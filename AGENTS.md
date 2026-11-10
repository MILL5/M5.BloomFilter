# M5.BloomFilter

- .NET Bloom filter library with JSON/protobuf bit-array and filter serialization.
- Use the SDK pinned in `global.json`; run `dotnet test M5.BloomFilter.slnx` and `dotnet build M5.BloomFilter.slnx -c Release`.
- Preserve Bloom invariants: inserted keys never become absent, hashes are deterministic and in range, duplicate insertion is idempotent, snapshots own their bits.
- Test all hash methods, empty keys, sizing boundaries, concurrency, and populated serialization. False positives are permitted; false negatives are defects.
- Hash or encoding changes require an explicit persisted format version and migration documentation. Never silently reinterpret old bitsets.
- `Fixtures/v2.0.1/` in both test projects were written by the published 2.0.1 packages through `tools/V201Oracle`. Never regenerate them from current source; a fast path must reproduce them. `tools/` and `benchmarks/` projects stay outside `M5.BloomFilter.slnx` and are not packable.
- EGM runtime is project-local in `.agents/`; author designs in `docs/designs/`, use fresh-context Goldfish reviewers, resolve material findings before implementation.
- EGM artifact directory: docs/designs
- Branch flow: create feature branches from `develop`, open PRs to `develop`, then promote `develop` to `main` by merge PR. Never push directly to either protected branch; retain `develop` after promotion. GitHub Copilot reviews PRs to `develop` automatically.
- Never wait for nuget.org to index a release. After the publish workflow succeeds, check the version indexes once, report what is listed, and carry on.
- Keep project agent guidance in AGENTS.md only. Never rebase, add AI commit attribution, or manually request PR reviews.
- GitHub Pages must use verified MILL5 and Richard Crane MVP branding, accurate technical copy, accessible HTML, and maintained search/answer metadata.
- License: PolyForm Strict 1.0.0 for MILL5-owned work; preserve third-party licenses and historical release terms. Describe this release as source-available.
