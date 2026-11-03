# M5.BloomFilter

- .NET Bloom filter library with JSON/protobuf bit-array and filter serialization.
- Use the SDK pinned in `global.json`; run `dotnet test M5.BloomFilter.slnx` and `dotnet build M5.BloomFilter.slnx -c Release`.
- Preserve Bloom invariants: inserted keys never become absent, hashes are deterministic and in range, duplicate insertion is idempotent, snapshots own their bits.
- Test all hash methods, empty keys, sizing boundaries, concurrency, and populated serialization. False positives are permitted; false negatives are defects.
- Hash or encoding changes require an explicit persisted format version and migration documentation. Never silently reinterpret old bitsets.
- EGM runtime is project-local in `.agents/`; author designs in `docs/designs/`, use fresh-context Goldfish reviewers, resolve material findings before implementation.
- EGM artifact directory: docs/designs
- Keep project agent guidance in AGENTS.md only. Never rebase, add AI commit attribution, or manually request PR reviews.
- GitHub Pages must use verified MILL5 and Richard Crane MVP branding, accurate technical copy, accessible HTML, and maintained search/answer metadata.
