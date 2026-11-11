---
version: 1
slug: "docs-site-index-html"
primary_target: "docs/site/index.html"
related_targets: ["docs/site/style.css","docs/site/app.js"]
---

# M5.BloomFilter GitHub Page

- Target: `docs/site/index.html`, deployed at https://mill5.github.io/M5.BloomFilter/.
- Mode: Persuade. Reference sections below the demonstrations are Read.
- Audience and job: .NET developers deciding whether a Bloom filter fits and whether to install this one; curious visitors following a shared link.
- Action: type a word, then copy the install command. Second action: share the answer.
- Proof: a filter built by the C# library over the ENABLE word list, queried in the browser by a port of the library's default hash; measured false positives; recorded benchmarks only.
- Constraints: static files, no third-party requests, verified MILL5 and Richard Crane assets, source-available wording, FAQ text equal to JSON-LD, `llms.txt`, sitemap and a 1200x630 link preview maintained.
- Owner's brief, 10 November 2026: keep the headline "This page knows 172,823 English words and stores none of them"; bring back the earlier page's bit grid and its animation because it teaches; the punched cards were confusing; restore the three plain rules and the compact API reference; clean and simple is liked but not required; "you figure out everything else"; simplify; make it viral.
- Replaces the punched-card page. That metaphor is retired, not refined. The white and lilac page before it is evidence, not a look to restore.

## Direction contract

THESIS: No metaphor. A Bloom filter is a row of bits, so the page draws bits: a square is one bit, filled is 1, empty is 0, at every scale. It refuses both the library landing page of headline, snippet and feature cards, and the costume that needs explaining before the filter does.

OWN-WORLD: White ground, navy ink, one cobalt. A cell is a 4px-radius square holding a monospace 0 or 1: pale when off, solid cobalt when on; the 0 that decides an answer is white with a heavy ink ring. The real filter is one square image of all 1,657,884 bits. A yellow disc with a navy edge marks a bit a key points at that is on (a white disc with a double ink ring when it is off), and yellow rings and flashes the cells a key points at in the 64-bit grid; the only other yellow is the text-selection colour. Labels such as FALSE POSITIVE are an ink block on the page and a white block on the band. One full-width cobalt band holds the 64-bit grid, with white cells for 1. The install section sits on a pale tint lighter than an off bit; code blocks and the footer are navy; every other section is white. Barlow Condensed for headings and verdicts, Barlow for sentences, the platform monospace for bits, addresses, typed keys and code. Flat: no shadows, no cards.

STORY: The page knows 172,823 words and keeps none. Seven bits answer: one 0 is proof of absence, seven 1s are only probable. Build a 64-bit one and cause a false positive yourself. Then install.

FIRST VIEWPORT: Masthead line with the MILL5 mark. The headline across the full column at up to 5.2rem. Beneath it, left: one sentence, the word field with its Check key, seven cells that fill in order, the verdict in heavy condensed type and its reason. Right: the square of every bit in a pale frame, with numbered yellow marks where the seven fell. On a phone the picture is short and wide and sits directly under the cells and verdict. The primary action is the word field.

SIGNATURE INTERACTION: Checking a word fills the seven cells one at a time and drops a numbered mark on the field for each; it stops at the first 0 and leaves the rest unchecked, as `Contains` does. Pointing at the picture, or at one of the seven cells, opens a loupe of the real bits around that position as cells. In the 64-bit grid, adding a key flips its three cells on, and "Find a false positive" names the keys that switched on a stranger's bits. The 10,000-word run draws one square per made-up word and fills the ones that got through.

FORM: the bit field and the cell grid, pinned by the owner's brief, which beats the roll; the roll's assigned candidate was not built. seed:a6448a9da5c66699

FINISH: unreviewed and undocumented is unfinished; this build ends with the finish review, the verdict, DESIGN.md, and every shipping raster carrying its provenance.

## Unresolved

- None blocking.
