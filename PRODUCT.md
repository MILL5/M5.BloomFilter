# Product

<!-- impeccable:product-schema 1 -->

Recorded from the repository and the owner's brief ("do what you want to make it
viral and awesome; it cannot look like it was done with AI"). No interview was
held; items marked *(inferred)* are assumptions drawn from the repository.

## Platform

web

## Stack

Static HTML, CSS and JavaScript in `docs/site`, deployed unchanged by the pinned
GitHub Pages workflow. No build step, framework, analytics or third-party runtime
requests.

## Users

.NET developers deciding whether a Bloom filter fits a problem and whether this
package is the one to install *(inferred)*. They arrive from NuGet, GitHub,
search results, an assistant's answer, or a link someone shared. Many know the
term but have never watched one work. A second audience is anyone curious enough
to follow a shared link and play with the demonstration *(inferred)*.

## Product Purpose

M5.BloomFilter is a C# Bloom filter library for .NET 10 with JSON and protobuf
snapshot packages. The page exists to make the data structure's tradeoff
something a visitor experiences first-hand, then to get the package installed
and used correctly.

## Positioning

The page runs a real filter built by the C# library: a browser port of the
library's default hash reads a snapshot the library wrote, so the bit positions
on screen are the ones `ComputeHash` returns. The library versions its persisted
hash format and refuses to reinterpret old bits.

## Operating Context

Membership pre-checks in front of a slower exact lookup (database, cache, API,
disk). Filters are sized from an expected key count and a target false-positive
probability, filled once or incrementally, shared as immutable snapshots, and
persisted as JSON or protobuf.

## Capabilities and Constraints

- A negative answer is certain for an intact, consistently used filter; a
  positive answer is probable and must be confirmed against the source of truth.
- No delete operation. Overfilling raises the false-positive rate.
- Sixteen hash strategies; the default is Murmur3KirschMitzenmacher.
- Version 2.1.0: lookups take no lock; string, numeric and span keys are hashed
  without allocating for the Murmur3 strategies; `GetOccupancy()` measures how
  full a filter is; `Union` and `UnionWith` combine compatible filters.
- Persisted hash format version 2; older payloads must be rebuilt from keys.
- The requested error rate is a sizing target, not a measured guarantee or an
  adversarial-input guarantee.
- Requires .NET 10. Source-available under PolyForm Strict 1.0.0; describe it as
  source-available, never as open source.

## Brand Commitments

- Verified assets only: `docs/site/assets/mill5-logo.png`,
  `docs/site/assets/richard-crane.jpg`, and the NuGet package icon
  `M5.BloomFilter/bitarray_400-min.png`. Never draw or generate a substitute
  logo or mark.
- Attribution: MILL5 and Richard Crane, Microsoft MVP, founder of MILL5.
- Family links: FastSearch, Pineapple, yocto.
- The previous page's violet palette is not a brand commitment; the MILL5 mark
  and the package icon are both blue.

## Evidence on Hand

- Frozen hash vectors and statistical false-positive tests in
  `M5.BloomFilter.UnitTests`, and fixtures written by the published 2.0.1
  packages (`tools/V201Oracle`).
- `docs/site/words.json`: a filter over the 172,823-word public-domain ENABLE
  list, built by `tools/SiteFilter` (1,657,884 bits, 7 hashes, 10,006 false
  positives in one million fixed non-words). `SiteVectorTests` and
  `tools/SiteFilter/check-page.mjs` hold the page to it.
- `docs/benchmarks.md`: BenchmarkDotNet results for 2.0.1 against 2.1.0 on one
  named machine. These are the only performance figures that may be quoted.
- Sizing formulas in `M5.BloomFilter/Statistics.cs`.
- Burton H. Bloom, "Space/Time Trade-offs in Hash Coding with Allowable Errors",
  CACM 13(7), 1970; Kirsch and Mitzenmacher, "Less Hashing, Same Performance:
  Building a Better Bloom Filter", 2006.
- No customer names, testimonials, download counts or third-party comparisons
  are available. Do not invent any.

## Product Principles

1. Show the real thing working before describing it.
2. State what an answer means in the same breath as the answer.
3. Every number on the page is computed or measured, and says which.
4. Limits are part of the pitch: say when not to use a Bloom filter.

## Accessibility & Inclusion

Keyboard and touch operable throughout, visible focus, state never carried by
colour alone, reduced-motion respected, no overflow from 320px to 1440px, and
the reference content readable without JavaScript.
