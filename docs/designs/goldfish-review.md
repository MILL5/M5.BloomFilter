# Goldfish design review and resolutions

Fresh reviewer: goldfish_design; no parent conversation supplied. Reviewed four
personas (zoom-out, comprehension, design critic, implementation readiness),
three standards, the draft and listed source/serializer files.

- Initial zoom out: direction passes; reuse existing storage/helpers.
- Comprehension: proposed invariants, intentional format break, .NET and Pages
  scope were understood; no comprehension blocker.
- Design critique: majors for hash byte contracts, short K limits, ambiguous
  scalar generic overloads, and complete Brotli input consumption.
- Intermediate zoom out: shared encoder and existing families are sufficient.
- Readiness: required concrete SDK/packages/Pages scope, exact file boundaries,
  stable checkpoints and collection/boxed scalar regression assertions.

Resolutions: draft now records object scalar overloads, canonical encoder,
constrained K, exact schema, strict compressed decoding, explicit file paths and
dependency evidence. Hash rules and vectors are frozen in hash-format-v2.md.
Hash/encoding/serialization changes are one atomic buildable checkpoint; existing
tests and regressions must pass together before release. Pages source is only
docs/site, preventing review artifacts from becoming site content.

Review is design evidence, not proof of implementation. Final source review and
Debug/Release tests are required after implementation.

## Final source review and regression resolutions

Fresh reviewer goldfish_source read the current sources, tests, standards, SDK,
project declarations and solution mapping without parent conversation history.
It found two material gaps: optional JSON converters did not support declared
IImmutableBloomFilter values. Both now handle interface and concrete snapshots;
populated all16-strategy regression tests explicitly round-trip both declarations.
No other material source defect was identified by that review.

An independent all16 error-rate test caught Adler32 at 1500/50000 false positives
(3%) for a 1% target. Adding a full-key FNV1a discriminator to checksum expansion
reduced Adler32 to 466/50000 (0.932%) without relaxing the six-sigma threshold.
Golden checksum vectors were independently recomputed before freezing v2.
