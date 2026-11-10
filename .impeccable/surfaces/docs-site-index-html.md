---
version: 1
slug: "docs-site-index-html"
primary_target: "docs/site/index.html"
related_targets: ["docs/site/style.css","docs/site/app.js"]
---

# M5.BloomFilter GitHub Page

- Target: `docs/site/index.html`, deployed at https://mill5.github.io/M5.BloomFilter/.
- Mode: Persuade. Reference sections below the demonstration are Read.
- Audience and job: .NET developers deciding whether a Bloom filter fits and whether to install this one; curious visitors following a shared link.
- Action: try a word, then copy the install command.
- Proof: a filter built by the C# library over the ENABLE word list, queried in the browser by a port of the library's default hash; measured false positives; recorded benchmarks only.
- Constraints: static files, no third-party requests, verified MILL5 and Richard Crane assets, source-available wording, FAQ text equal to JSON-LD, `llms.txt` and sitemap maintained.
- Replaces the earlier white and lilac page. That look is retired, not refined.
- Decided without an interview on the owner's instruction to proceed on judgment.

## Direction contract

THESIS: The filter is shown as the object it would have been in 1970, the year Bloom published: a deck of punched cards, 960 bits a card, read by pulling seven cards and looking for seven holes. The page refuses the library landing page of headline, install snippet and feature cards; the first thing a visitor does is make a real filter answer.

OWN-WORLD: A 1970 machine room. Saturated tray-blue ground from the package icon's blue. Manila 80-column cards at true proportion with a cut top-left corner, printed digit rows and column numbers, rectangular holes that show the blue behind them. Long content on fan-fold printout paper with sprocket margins, perforations between sections and pale green bars under listings. One salmon card for limits. Barlow Condensed for card legends and headings, Barlow for reading, the platform monospace only for code, typed keys and printout data. Controls are flat ink keys that sink when pressed; inputs are printed card fields. A red grease-pencil ring marks a blank hole.

STORY: The page knows 172,823 words and holds none. A missing hole is proof of absence; seven punched holes are only probable. Nonsense gets through about once in a hundred. Then: size a deck, install, read the limits.

FIRST VIEWPORT: Blue ground edge to edge. A manila header card across the content width carries the MILL5 imprint, navigation on its print line, the headline at up to 5.2rem, one sentence, and the word field with its key. Directly below, the verdict in white heavy condensed type on the blue, then the upper half of the live probe card with its real holes. The primary action is the word field; the install command sits on the same card.

SIGNATURE INTERACTION: Submitting a word feeds the seven probed cards through the reader in order, rings each hole, and stops on the card that decides the answer. Every probe is also a table row that recalls its card.

FORM: punched-card deck with fan-fold printout, candidate 3 of 7 on the ordered list; seed:e6252ddc74bafe3e.

FINISH: unreviewed and undocumented is unfinished; this build ends with the finish review, the verdict, DESIGN.md, and every shipping raster carrying its provenance.

## Unresolved

- None blocking. Benchmark figures appear only after `docs/benchmarks.md` records them.
