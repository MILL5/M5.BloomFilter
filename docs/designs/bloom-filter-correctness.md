# Bloom filter correctness

- Author: Richard Crane
- Date: 2026-11-03
- Status: Reviewed; implementing with Goldfish findings resolved

## Overview and problem

This repository supplies a mutable Bloom filter, immutable snapshots, sixteen hash
strategies, typed extensions, JSON and protobuf serializers, and four test suites.
Existing tests pass (54 tests), but most exercise happy paths or only print false
positive counts. They do not establish the probabilistic or persistence contract.
The Elephant review covers all tracked source, project, solution, tests and docs;
image assets are packaging inputs and contain no algorithm logic.

## Goals and non-goals

For every supported hash strategy, insertion sets exactly the positions that
lookup tests. Successful insertion must never later return absent. Empty bytes
are a valid element. All positions are deterministic, terminate, and lie in
[0,m). Duplicate insertion is idempotent. Batch and asynchronous APIs agree with
single-element operations. Snapshots cannot be changed through exported bitsets.

The default strategy must have a measured false positive rate near its requested
rate at capacity. Statistics describe the usual ideal independent-hash estimate,
not a guarantee for adversarial inputs or historical weak checksum strategies.
Reject invalid dimensions, NaN probabilities, malformed payloads, and truncated
bitsets. Upgrade all projects and direct dependencies to the latest stable .NET release and packages. Publish a branded GitHub Pages site; no NuGet publication.

## Critical user journeys

1. Given an empty filter, add byte arrays including empty and short values;
   every inserted value remains present in single, batch, async and snapshot reads.
2. Given a snapshot, mutate the original filter and every exported bit array;
   the snapshot retains its original bits and membership results.
3. Given a populated filter, round-trip all supported serializers; all inserted
   values remain present. A malformed or unsupported old payload fails explicitly.
4. Given n=10000 and requested p=0.01, insert a deterministic corpus and query
   50000 disjoint values; observed false positives must fall below p plus six
   binomial standard deviations. No test assumes absent keys always return false.

## Technical plan

Keep BitArray storage and the existing public interfaces. Hide owned bits behind
snapshot getters, use private storage inside operations, and capture mutable
snapshots under the same lock used by insertion. The existing synchronous Task
wrappers stay synchronous. Validate null byte inputs before hashing.

Sizing uses m=ceil(-n*ln(p)/ln(2)^2), at least one hash, and the better of adjacent
integer k values around m/n*ln(2). Increase m if integer rounding would put the
calculated error above the requested rate. Reject requests exceeding Int32 bit
capacity instead of silently clamping. Compute probability with floating point
multiplication before k*n to avoid integer overflow. All probabilities must be
finite and strictly between zero and one on input.

Hash fixes preserve algorithm families: unsigned logical shifts; little-endian
Murmur3 block reads; safe handling of Int32.MinValue; bounded hash generation for
Murmur2 short inputs; counter-fed checksum and crypto expansion; parameter guards.
LCG uses its full 48-bit state. Every public built-in ComputeHash rejects null,
m<=0 and k<=0. Counter expansion must not repeat a fixed digest forever.

### Persistence and compatibility

Hash corrections change bit positions. Introduce explicit hash format version 2
on ReadOnlyFilter and serialized JSON field v / protobuf tag 7. New writers emit
2. Readers reject missing or unsupported versions with a clear rebuild message,
rather than interpreting old bitsets with changed hashes and returning false
negatives. This is an intentional persistence compatibility break: rebuild from
original keys before upgrading persisted data. Keep existing hash enum numbers
and other field names/tags. Old software must not consume newly generated filters.
Document this migration and require a major package version before publication;
this work does not publish packages. The direct bits constructor is an explicit
current-format construction API, not an automatic legacy migration.

Bit-array readers require nonnegative logical length and exact ceil(length/8)
decoded bytes; reject truncated, surplus, or invalid compressed data. Empty arrays
use zero bytes. The shared byte-count helper avoids overflow at Int32.MaxValue.
ReadOnlyFilter validates M equals bit length and clones constructor input.
The incomplete core Newtonsoft converter must round-trip the same logical fields
with its own documented raw base64 bits representation and version 2.

## Alternatives and risks

Keeping legacy broken hash algorithms would retain hangs and weak distributions.
Adding sixteen new enum IDs and retaining all old implementations would preserve
legacy reads but doubles maintenance and leaves unsafe selectable methods.
Explicit format rejection requires rebuilding existing persisted filters but
prevents silent false negatives without duplicating algorithms. Replacing the
entire library or all algorithms with SHA256 would break existing API expectations.

Cloning exported bits costs O(m) per export; hashing and lookups must use private
bits to avoid O(m) per lookup. Batch APIs retain current materialization semantics.
Very large valid requests can still exhaust memory; arithmetic validation is not
an allocator guarantee. Untrusted bit-array dimensions must be validated before
allocation; no arbitrary new product size cap is imposed.

## Additional requested scope

Create root AGENTS.md with repo build, Bloom invariants, persistence and EGM guidance.
Upgrade every project to .NET 10 (latest stable on November 3; .NET 11 remains
a release candidate), update every direct NuGet dependency, pin a stable SDK,
and add CI for restore/build/test/pack. Examine all sibling projects for Pages
patterns, use their verified MILL5 and Richard Crane MVP branding, and deploy a
static site through GitHub Actions. Include canonical/Open Graph/Twitter metadata,
sitemap, robots.txt, accessible semantic HTML, factual FAQ and SoftwareSourceCode,
Person, Organization structured data, plus llms.txt for discoverable source links.
SEO/AEO/GEO content must describe measured library behavior without unsupported
performance promises or guaranteed rankings. Update README and public docs with
examples, migration, APIs, limits and EGM workflow.

## Detailed implementation plan

### Phase 1: arithmetic and owned storage

Modify M5.BloomFilter/Statistics.cs, BloomFilter.cs, ReadOnlyFilter.cs and
HashFunction/HashFunction.cs. Add guarded sizing and floating point probability;
clone owned storage and synchronize snapshot exports. Add format version property
and explicit version-checked deserialization constructor overload. Keep public
constructor signatures source compatible. Validate null elements and batches.
Fix generic encoding in both BloomFilterExtensions files: scalar forwarding
uses the same primitive byte encoding as typed overloads, invariant text for
other values; collection arguments must enumerate rather than hash type names.
Use canonical little-endian primitive bytes.

### Phase 2: hash correctness

Modify HashAlgorithms/LCGWithFNV.cs, RNGWithFNV.cs, HashChecksum.cs, HashCrypto.cs,
Murmur2.cs, Murmur3.cs and checksum empty-range validation in Adler32.cs,
Crc32.cs and Crc32u.cs. Centralize parameter validation in HashFunction.cs.
Keep all sixteen enum IDs. Check deterministic range, empty input, repeated
calls, m=1 and near-Int32-max m; include k larger than one digest can supply.

### Phase 3: serialization

Modify M5.BitArraySerialization.Shared/BitArrayExtensions.cs,
M5.BitArraySerialization.Json/BitArrayDTO.cs,
M5.BitArraySerialization.ProtoBuf/BitArray.cs,
M5.BloomFilter.Serialization.Json/BloomFilterDTO.cs,
BloomFilterConverter.System.Text.Json.cs and BloomFilterConverter.Newtonsoft.Json.cs,
M5.BloomFilter.Serialization.ProtoBuf/BloomFilter.cs, and
M5.BloomFilter/BloomFilterJsonConverter.cs. Persist version and enforce exact
lengths. Preserve Brotli threshold 257 and protobuf field tags 1 through 6.

### Phase 4: regression evidence and documentation

Add focused correctness tests to the four existing test projects covering all
sixteen strategies, duplicate and batch semantics, null inputs, finite sizing,
overflow, empty inputs, snapshots, parallel writers, populated serialization,
compression boundaries 0/1/7/8/9/256/257/258 and malformed payloads. Preserve
existing tests except assertions that incorrectly prohibit all false positives.
Update README.md with usage, probability limits, thread and snapshot behavior,
format migration, and the runnable command. Record actual test totals and
Goldfish findings/resolutions in review artifacts under docs/designs.

## Verification and rollback

Run DOTNET_ROLL_FORWARD=Major dotnet test M5.BloomFilter.slnx --nologo
--verbosity quiet --blame-hang-timeout 60s -- RunConfiguration.TargetPlatform=ARM64.
Baseline command only; after upgrade use the pinned .NET 10 SDK with dotnet test
without runtime roll-forward.
Run Debug and Release builds/tests, git diff --check, and packaging checks for the
installed EGM runtime. Fresh Goldfish reviewers read this draft and standards
without parent conversation history before implementation and inspect final code
afterward. Commit and push the reviewed implementation through a PR, monitor automatic
Copilot reviews without requesting them, then merge when required checks permit
and verify the published GitHub Pages deployment and public response. Rollback restores source and requires
rebuilding stored filters with the corresponding old or new library version.

## Open questions

NuGet publishing is now explicitly requested. Authentication is missing; the
prepared release workflow supports an API key secret or trusted publishing.
Operational migration of user-owned persisted filters remains outside this change.

## Goldfish decisions resolved before implementation

Remove ambiguous generic scalar overloads; keep non-generic object scalar overloads
and generic IEnumerable<T> batch overloads. This intentional major-version source
break fixes collection dispatch. A shared encoder dispatches primitives, strings,
byte arrays and DateTime first, uses little-endian bytes, and invariant text for
other scalar objects. Collections passed as object are rejected rather than
silently hashed as their type name. K is optimized within 1..Int16.MaxValue.

The hash worker freezes v2 counter/seed/mapping rules and golden vectors in
hash-format-v2.md. Crypto expansion uses data followed by a little-endian UInt32
counter starting at zero, reads little-endian UInt32 chunks, and maps modulo m.
Checksum expansion uses a full little-endian counter and an avalanche mixer.
Murmur2 uses UInt32 arithmetic; Murmur variants use explicit little-endian reads
and deterministic nonzero domain seeds. No rejection loop may cycle forever.

The core converter uses hb={b:base64 raw bytes,l:bit length}, hm,m,k,n,p,v.
External bit-array converters keep threshold-based Brotli. Readers require exact
output bytes and full compressed input consumption. New serialization is v=2;
missing versions fail with a rebuild message. Tests freeze all changed contracts.

Additional concrete files: AGENTS.md, global.json, all ten existing .csproj files,
.github/workflows/ci.yml, .github/workflows/pages.yml, docs/site/index.html,
docs/site/styles.css, docs/site/app.js, docs/site/robots.txt, sitemap.xml, llms.txt,
.nojekyll and branding assets, README.md, docs/usage.md, docs/serialization.md,
docs/designs/dependencies.md and regression test files in the four existing test
projects. Dependency exact versions and SDK evidence are in dependencies.md.
