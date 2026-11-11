---
name: M5.BloomFilter
description: A Bloom filter for .NET 10 and 11, drawn as what it is. A square is one bit; filled is 1, empty is 0.
colors:
  ground: "#ffffff"
  tint: "#f1f3fc"
  off: "#e4e9fb"
  off-edge: "#c5cef2"
  on: "#2340e0"
  on-deep: "#1a31b4"
  on-soft: "#c9d3ff"
  ink: "#0d1433"
  ink-soft: "#414a70"
  mark: "#ffd23f"
  mute: "#6a7296"
  code-text: "#eef1ff"
  code-comment: "#a9b4e8"
typography:
  display: { fontFamily: "Barlow Condensed, Arial Narrow, Helvetica Neue, Arial, sans-serif", fontSize: "clamp(2.5rem, 6.1vw, 5.2rem)", fontWeight: 800, lineHeight: 0.97, letterSpacing: "-0.012em" }
  verdict: { fontFamily: "Barlow Condensed, Arial Narrow, Helvetica Neue, Arial, sans-serif", fontSize: "clamp(2.2rem, 4.7vw, 3.75rem)", fontWeight: 800, lineHeight: 0.98, letterSpacing: "-0.01em" }
  headline: { fontFamily: "Barlow Condensed, Arial Narrow, Helvetica Neue, Arial, sans-serif", fontSize: "clamp(2rem, 3.8vw, 2.9rem)", fontWeight: 600, lineHeight: 1.04, letterSpacing: "-0.005em" }
  rule-title: { fontFamily: "Barlow Condensed, Arial Narrow, Helvetica Neue, Arial, sans-serif", fontSize: "clamp(1.625rem, 2.6vw, 2rem)", fontWeight: 600, lineHeight: 1.08 }
  title: { fontFamily: "Barlow Condensed, Arial Narrow, Helvetica Neue, Arial, sans-serif", fontSize: "1.375rem", fontWeight: 600, lineHeight: 1.2, letterSpacing: "0.005em" }
  question: { fontFamily: "Barlow Condensed, Arial Narrow, Helvetica Neue, Arial, sans-serif", fontSize: "1.25rem", fontWeight: 600, lineHeight: 1.3 }
  standfirst: { fontFamily: "Barlow, Segoe UI, Helvetica Neue, Arial, sans-serif", fontSize: "clamp(1.0625rem, 1.5vw, 1.25rem)", fontWeight: 400, lineHeight: 1.5 }
  body: { fontFamily: "Barlow, Segoe UI, Helvetica Neue, Arial, sans-serif", fontSize: "1.125rem", fontWeight: 400, lineHeight: 1.6, fontFeature: "tnum" }
  fine: { fontFamily: "Barlow, Segoe UI, Helvetica Neue, Arial, sans-serif", fontSize: "0.9375rem", fontWeight: 400, lineHeight: 1.5 }
  nav: { fontFamily: "Barlow, Segoe UI, Helvetica Neue, Arial, sans-serif", fontSize: "1rem", fontWeight: 600, lineHeight: 1 }
  legend: { fontFamily: "Barlow Condensed, Arial Narrow, Helvetica Neue, Arial, sans-serif", fontSize: "0.875rem", fontWeight: 500, lineHeight: 1.2, letterSpacing: "0.06em" }
  key: { fontFamily: "Barlow Condensed, Arial Narrow, Helvetica Neue, Arial, sans-serif", fontSize: "1.125rem", fontWeight: 600, lineHeight: 1, letterSpacing: "0.07em" }
  tag: { fontFamily: "Barlow Condensed, Arial Narrow, Helvetica Neue, Arial, sans-serif", fontSize: "0.9375rem", fontWeight: 600, lineHeight: 1.2, letterSpacing: "0.06em" }
  typed: { fontFamily: "ui-monospace, Cascadia Mono, Cascadia Code, Consolas, SF Mono, Menlo, DejaVu Sans Mono, monospace", fontSize: "1.375rem", fontWeight: 400, lineHeight: 1.2 }
  bit: { fontFamily: "ui-monospace, Cascadia Mono, Cascadia Code, Consolas, SF Mono, Menlo, DejaVu Sans Mono, monospace", fontSize: "calc(var(--cell) * 0.44)", fontWeight: 600, lineHeight: 1 }
  chip: { fontFamily: "ui-monospace, Cascadia Mono, Cascadia Code, Consolas, SF Mono, Menlo, DejaVu Sans Mono, monospace", fontSize: "0.9375rem", fontWeight: 400 }
  address: { fontFamily: "ui-monospace, Cascadia Mono, Cascadia Code, Consolas, SF Mono, Menlo, DejaVu Sans Mono, monospace", fontSize: "0.6875rem", fontWeight: 400, lineHeight: 1.2 }
  code: { fontFamily: "ui-monospace, Cascadia Mono, Cascadia Code, Consolas, SF Mono, Menlo, DejaVu Sans Mono, monospace", fontSize: "0.875rem", fontWeight: 400, lineHeight: 1.65 }
rounded:
  cell: "4px"
  loupe-cell: "2px"
  mark: "50%"
spacing:
  column: "1180px"
  gutter: "clamp(14px, 3.2vw, 32px)"
  section: "clamp(56px, 8.5vw, 112px)"
  spread-gap: "clamp(28px, 5vw, 76px)"
  cell-gap: "clamp(4px, 1vw, 10px)"
  measure: "34rem"
  target: "56px"
  target-min: "44px"
components:
  key: { backgroundColor: "{colors.on}", textColor: "{colors.ground}", typography: "{typography.key}", rounded: "{rounded.cell}", padding: "0 26px", height: "56px" }
  key-hover: { backgroundColor: "{colors.on-deep}" }
  key-disabled: { backgroundColor: "{colors.mute}", textColor: "{colors.ground}" }
  key-small: { backgroundColor: "{colors.on}", textColor: "{colors.ground}", rounded: "{rounded.cell}", padding: "0 18px", height: "48px" }
  key-quiet: { backgroundColor: "{colors.ground}", textColor: "{colors.ink}", rounded: "{rounded.cell}", padding: "0 16px", height: "44px" }
  key-quiet-hover: { backgroundColor: "{colors.tint}" }
  key-light: { backgroundColor: "{colors.ground}", textColor: "{colors.on}", rounded: "{rounded.cell}", padding: "0 26px", height: "56px" }
  key-light-hover: { backgroundColor: "{colors.on-soft}" }
  key-ghost: { backgroundColor: "{colors.on}", textColor: "{colors.ground}", rounded: "{rounded.cell}", padding: "0 26px", height: "56px" }
  key-ghost-hover: { backgroundColor: "{colors.on-deep}" }
  field: { backgroundColor: "{colors.ground}", textColor: "{colors.ink}", typography: "{typography.typed}", rounded: "{rounded.cell}", padding: "0 16px", height: "56px" }
  field-sizer: { backgroundColor: "{colors.ground}", textColor: "{colors.ink}", rounded: "{rounded.cell}", padding: "0 16px", height: "50px" }
  cell-off: { backgroundColor: "{colors.off}", textColor: "{colors.ink-soft}", typography: "{typography.bit}", rounded: "{rounded.cell}" }
  cell-on: { backgroundColor: "{colors.on}", textColor: "{colors.ground}", typography: "{typography.bit}", rounded: "{rounded.cell}" }
  cell-deciding-zero: { backgroundColor: "{colors.ground}", textColor: "{colors.ink}", typography: "{typography.bit}", rounded: "{rounded.cell}" }
  band-cell-off: { backgroundColor: "{colors.on-deep}", textColor: "{colors.on-soft}", rounded: "{rounded.cell}" }
  band-cell-on: { backgroundColor: "{colors.ground}", textColor: "{colors.on}", rounded: "{rounded.cell}" }
  mark: { backgroundColor: "{colors.mark}", textColor: "{colors.ink}", rounded: "{rounded.mark}", size: "26px" }
  mark-zero: { backgroundColor: "{colors.ground}", textColor: "{colors.ink}", rounded: "{rounded.mark}", size: "26px" }
  bitfield-frame: { backgroundColor: "{colors.off}", rounded: "{rounded.cell}", padding: "14px" }
  loupe: { backgroundColor: "{colors.ground}", textColor: "{colors.ink-soft}", typography: "{typography.address}", rounded: "{rounded.cell}", padding: "6px" }
  chip: { backgroundColor: "{colors.ground}", textColor: "{colors.ink}", typography: "{typography.chip}", rounded: "{rounded.cell}", padding: "0 14px", height: "44px" }
  chip-hover: { backgroundColor: "{colors.off}" }
  chip-band: { backgroundColor: "{colors.on}", textColor: "{colors.ground}", rounded: "{rounded.cell}", padding: "0 14px", height: "44px" }
  chip-band-hover: { backgroundColor: "{colors.on-deep}" }
  tag: { backgroundColor: "{colors.ink}", textColor: "{colors.ground}", typography: "{typography.tag}", rounded: "{rounded.cell}", padding: "2px 8px 3px" }
  tag-band: { backgroundColor: "{colors.ground}", textColor: "{colors.ink}", typography: "{typography.tag}", rounded: "{rounded.cell}", padding: "2px 8px 3px" }
  code-block: { backgroundColor: "{colors.ink}", textColor: "{colors.code-text}", typography: "{typography.code}", rounded: "{rounded.cell}", padding: "20px 22px" }
  install-command: { backgroundColor: "{colors.ground}", textColor: "{colors.ink}", rounded: "{rounded.cell}", padding: "8px 14px", height: "48px" }
  inline-code: { backgroundColor: "{colors.tint}", textColor: "{colors.ink}", rounded: "{rounded.cell}", padding: "1px 5px" }
  band: { backgroundColor: "{colors.on}", textColor: "{colors.ground}" }
  tint-section: { backgroundColor: "{colors.tint}", textColor: "{colors.ink}" }
  footer: { backgroundColor: "{colors.ink}", textColor: "{colors.on-soft}", typography: "{typography.fine}", padding: "32px 0 40px" }
---

# Design System: M5.BloomFilter

## Overview

**Creative North Star: "Bits, Plainly"**

A Bloom filter is a row of bits, so the site draws bits and nothing that needs explaining first. The unit is one square cell holding a monospace 0 or 1: pale when it is 0, solid cobalt when it is 1. The same unit appears at every scale: three to five cells above a rule, seven cells for an answer, an eight by eight grid, a seven by seven loupe, a hundred by hundred run of squares, and one picture of all 1,657,884 bits where each cell has shrunk to a dot.

The page is white with navy ink and one cobalt. A single full-width cobalt band holds the 64-bit grid, the install section sits on a pale tint, and the footer is navy. Everything is flat. Type is a heavy condensed sans for headings and verdicts, its regular-width sibling for sentences, and the platform monospace for anything a machine holds or a visitor types. Recorded from the shipped `docs/site/style.css`, `app.js`, `field.js` and `assets/social-card.html`. This replaces the punched-card system, which is retired.

**Key Characteristics:**
- White ground, navy ink, one cobalt. Cobalt means a 1.
- One unit, the cell: a square with a 4px radius holding a monospace 0 or 1.
- The real filter is one picture of every bit in a pale frame, painted from the real snapshot.
- A yellow disc with a navy edge marks a bit a key points at.
- One cobalt band per page; inside it the cell inverts and a 1 is white.
- Flat: no cast shadows, no gradients, no textures, no card containers.
- Brand marks are the real files. Nothing is drawn or generated to stand in for one.

## Colors

White and navy with a single saturated cobalt, a family of pale cobalt tints for the off state, and one yellow reserved for pointing.

### Primary
- **Cobalt** (`on`): a bit that is 1. Also the band, the primary key, links, the headline figure, the "probably" verdict, the field caret, the scrollbar thumb and the `theme-color`. White on it measures 7.29:1; it measures 7.29:1 on white and 6.58:1 on `tint`.
- **Deep Cobalt** (`on-deep`): a 0 inside the band, the hover of a cobalt key, the hover of a band chip, inline code on the band. White on it is 9.80:1.
- **Soft Cobalt** (`on-soft`): secondary text on the band and on the footer, the 0 numeral in a band cell, the band chip's ring, the hover of the light key. 4.94:1 on `on`, 6.65:1 on `on-deep`, 12.23:1 on `ink`.

### Secondary
- **Mark Yellow** (`mark`): the disc on the bit picture, the aim ring around the 64-bit cells a key points at, the colour a grid cell flips from, and the text selection. Ink on it is 12.48:1. Against the bit picture's average tone it is only about 2.0:1 and against white 1.44:1, which is why it always carries a navy edge or navy text and is never text itself.

### Neutral
- **Ground** (`ground`): the page, fields, the loupe, the quiet key, a 1 inside the band.
- **Tint** (`tint`): the install section's ground, inline code on white, the scrollbar track, the quiet key's hover. It is lighter than `off`; the two are separate tokens.
- **Off** (`off`): a bit that is 0, the frame around the bit picture, the unlit squares of the 10,000 run, chip hover. Cobalt on it is 6.02:1.
- **Off Edge** (`off-edge`): 1px hairlines (FAQ rules, the benchmark note's rule, the masthead divider) and the 1.5px ring of an unread cell. Decorative only: 1.56:1 on white.
- **Ink** (`ink`): text, 2px rules and borders, the tag, the code block and footer grounds, the focus outline. 18.03:1 on white, 16.28:1 on `tint`, 14.89:1 on `off`.
- **Soft Ink** (`ink-soft`): legends, fine print, captions, bit addresses, the 0 numeral in an off cell. 8.61:1 on white, 7.78:1 on `tint`, 7.11:1 on `off`.
- **Mute** (`mute`): placeholder text and the disabled key's ground. 4.71:1 with white either way.
- **Code Text** and **Code Comment** (`code-text`, `code-comment`): code on the ink block, 16.02:1 and 8.91:1.

`mute`, `code-text` and `code-comment` are literals in the stylesheet, not custom properties; the rest are `:root` custom properties of the same name.

### Named Rules
**The One Cobalt Rule.** There is one accent hue. Cobalt means a 1, and by extension the thing to press and the answer "probably". No second accent, no status green or red: a "no" is navy ink, not a colour.

**The Pointing Rule.** `mark` means "a key points here". It appears as the numbered disc, the aim ring, the flip, and the text selection, and nowhere else. It always sits beside navy and is always backed by text (the numeral on the disc, the status sentence, the address under the cell).

**The Inverted Band Rule.** Inside the cobalt band the cell inverts: a 1 is white with a cobalt numeral, a 0 is `on-deep` with an `on-soft` numeral. Secondary text is `on-soft`, focus outlines are white, tags are white with ink text.

## Typography

**Display Font:** Barlow Condensed 500, 600, 800 (with Arial Narrow, Helvetica Neue, Arial)
**Body Font:** Barlow 400, 600 (with Segoe UI, Helvetica Neue, Arial)
**Label/Mono Font:** the platform monospace (`ui-monospace`, Cascadia Mono, Cascadia Code, Consolas, SF Mono, Menlo, DejaVu Sans Mono)

**Character:** A heavy narrow sans that states a result, beside its regular-width sibling for explaining it. Five woff2 files are self-hosted in `docs/site/assets/fonts/` under the OFL, with `font-display: swap`; the 800 condensed and the 400 text are preloaded. The page requests nothing from another origin. Body figures are tabular.

### Hierarchy
- **Display** (800, clamp 2.5rem to 5.2rem, 0.97, -0.012em): the one `h1`, across the full column, balanced wrap. Its figure is cobalt and does not break. On a short desktop window it tops out at 4.1rem.
- **Verdict** (800, clamp 2.2rem to 3.75rem, 0.98, -0.01em): the filter's answer. Cobalt for "probably", ink for "definitely not".
- **Headline** (600, clamp 2rem to 2.9rem, 1.04): section `h2`, balanced wrap.
- **Rule title** (600, clamp 1.625rem to 2rem, 1.08): the `h2` of each of the three rules.
- **Title** (600, 1.375rem, 1.2): `h3`. FAQ questions use the same face at 1.25rem, the product name in the masthead at 1.3125rem.
- **Standfirst** (400, clamp 1.0625rem to 1.25rem, 1.5): the sentence under the headline; the reason under the verdict runs at clamp 1.0625rem to 1.1875rem.
- **Body** (400, 1.125rem, 1.6; 1.0625rem at 620px and below): reading text, 34rem wide in a spread, 66ch in the FAQ.
- **Fine** (400, 0.9375rem, 1.5, soft ink): notes, captions, status lines, benchmark conditions, footer.
- **Legend** (condensed 500, 0.875rem, 0.06em, uppercase, soft ink): the label of a field or select.
- **Key** (condensed 600, 1.125rem, 0.07em, uppercase): lettering on a key; 1rem on the small and quiet keys.
- **Tag** (condensed 600, 0.9375rem, 0.06em, uppercase): the classification at the start of a status sentence.
- **Typed / Chip / Code** (mono 400 at 1.375rem, 0.9375rem and 0.875rem): what the visitor types, a key that can be checked again, and C#.
- **Bit** (mono 600, 0.44 of the cell's side): the 0 or 1 in a cell. In the 64-bit grid it is clamp 0.9375rem to 1.5rem.
- **Address** (mono 400, 0.6875rem): the bit number under one of the seven cells, and the numerals in the loupe.

### Named Rules
**The Three Faces Rule.** Condensed is for headings, verdicts, keys, legends and tags. Barlow is for sentences. Monospace is for bits, bit addresses, typed keys, chips and code, and never for a sentence.

**The Legend Rule.** An uppercase legend labels a control. It never sits above a heading as a category line.

## Layout

One centred column, `min(1180px, 100% - 2 × gutter)`, gutter `clamp(14px, 3.2vw, 32px)`. Full-width grounds (the band, the tint, the footer) carry the gutter themselves and hold the same column. Sections are separated by `clamp(56px, 8.5vw, 112px)`; consecutive white sections share one gap rather than doubling it.

The recurring arrangement is a two-column spread, gap `clamp(28px, 5vw, 76px)`, top-aligned: text and controls on one side, the picture or code on the other. The hero gives the picture a fixed 29rem column, the band gives the grid 31rem, and the later spreads split evenly. The three rules and the three reference articles are three equal columns. Reading text holds a 34rem measure. Primary targets are 56px tall and nothing interactive is under 44px; the small inline sample words extend their hit area with a pseudo-element.

The page runs: masthead, hero (headline, word field, seven cells, verdict, the bit picture), three rules, the cobalt band with the 64-bit grid, the 10,000-word run, install and sizing on the tint, reference, FAQ, imprint, navy footer.

Responsive changes, as built:
- **Wider than 1000px and no taller than 800px:** the headline, standfirst, cells (3.25rem) and verdict tighten and addresses hide, so the verdict and its reason stay above the fold.
- **1000px and below:** spreads stack. In the band, the grid moves between the status sentence and the list of added keys.
- **621px to 1000px:** the bit picture sits beside its caption (24rem and the remainder).
- **860px and below:** the three rules and the three reference articles become one column.
- **720px and below:** bit addresses under the seven cells hide.
- **620px and below:** body 1.0625rem. The bit picture moves directly under the seven cells and the verdict and becomes twice as wide as it is tall; the same bits are redrawn row by row to that shape. Navigation takes its own row, the band's field takes a full row with its two keys sharing the next, the grid's index numerals hide, code wraps at 0.75rem, the imprint and the sizer stack.
- **380px and below:** navigation tightens to 0.9375rem.

## Elevation & Depth

Flat. Nothing casts a shadow, nothing blurs, nothing is layered by tone except the three grounds (white, tint, cobalt band) and the navy footer and code block. The one floating object, the loupe, is separated by a 2px ink border, not a shadow. `box-shadow` appears only as a hard zero-blur ring: an inset 2px ring on the quiet and ghost keys, an inset 3px ink ring on the deciding 0, an inset 1.5px ring on chips and unread cells, and a white-then-ink double ring on the loupe's centre cell. A key's press is a 2px move down.

### Named Rules
**The Flat Rule.** No offset, no blur, no gradient, no texture. Separate with a ground change, a 2px ink rule, or a 1px `off-edge` hairline. A ring drawn with `box-shadow` must have zero offset and zero blur.

## Shapes

One radius. Cells, keys, fields, chips, tags, the bit picture's frame, the loupe, code blocks, inline code and the portrait all use 4px (`--cell-radius`). The loupe's 20px cells drop to 2px, as does the focus outline. The mark is the one circle. Cells are always square (`aspect-ratio: 1`).

Borders are navy and 2px where they bound something (fields, the install command, the loupe, the mark) or head something (each reference article, the sizing block). Hairlines are 1px `off-edge`. Sections are not boxed: nothing on the page is a card with its own ground, border and padding. The FAQ chevron is two 2px ink borders rotated 45 degrees and the select arrow is two ink triangles; neither is a glyph or an icon file.

## Components

### Cell
The unit. A square with a 4px radius and a centred monospace numeral at 0.44 of its side.
- **0:** `off` ground, soft-ink numeral. **1:** cobalt ground, white numeral.
- **Seven cells (the answer):** side `clamp(2.25rem, 11.6vw, 4rem)`, gap `clamp(4px, 1vw, 10px)`, the bit's address beneath. They fill one at a time, 150ms apart, each arriving from 55% scale in 420ms, and stop at the first 0. That deciding 0 is white with an inset 3px ink ring and an ink numeral. Cells never reached are empty with a 1.5px `off-edge` ring.
- **Rule strip:** three to five 30px cells, gap 5px, above each rule's heading.
- **64-bit grid:** eight columns, gap `clamp(4px, 1vw, 8px)`, inverted for the band, each cell carrying its index at the top left. Cells a key points at get a 3px `mark` outline at 2px offset. A cell that switches on scales up from 40% and from yellow in 480ms, 70ms after the previous one.

### Bit picture and mark
- **Picture:** every bit of the real snapshot, bit 0 at the top left, row by row, in a square (2:1 at 620px and below). `field.js` paints one canvas pixel per device pixel, each the area-weighted mix of `on` and `off` for the bits it covers; the browser never scales it. It sits in an `off` frame with 14px padding (13px on a phone), so a mark on an edge bit stays whole. Crosshair cursor.
- **Mark:** a 26px disc centred on the bit, `mark` yellow with a 2px ink border and a condensed 600 numeral, one per cell checked. A bit that is 0 gets a white disc with a second ink ring 3px out. Drops in from 3.2× scale in 520ms; grows to 1.35× while its cell is pointed at.
- **Loupe:** pointing at the picture or at one of the seven cells opens a seven by seven block of 20px cells (gap 2px, padding 6px) on white with a 2px ink border, offset 16px from the bit toward whichever side stays inside the picture and covers the fewest marks. The centre cell has the double ring. The caption states the same bit in words.
- **Run:** a hundred by hundred canvas of squares in `off`, filled `on` for each made-up word that got through, 400 per frame, with a 1px gap once a square is over 3 device pixels.

### Keys
- **Shape:** 4px radius, no border, 56px tall, 26px side padding (18px at 620px and below), uppercase condensed lettering.
- **Primary:** white on cobalt. Hover `on-deep` in 120ms; active sinks 2px in 80ms; disabled is `mute`.
- **Small:** 48px tall, 18px padding, 1rem. Copy beside the install command.
- **Quiet:** 44px tall, white with an inset 2px ink ring and ink lettering; hover `tint`. Share.
- **Light and ghost (band only):** white with cobalt lettering, hover `on-soft`; transparent with an inset 2px white ring, hover `on-deep`.
- **Text button:** underlined Barlow 600 at 1rem with a 44px target, white on the band; the underline thickens from 1.5px to 3px on hover. The sample words under the word field use it in ink monospace.

### Fields
- **Word field:** white, 2px ink border (white on the band), 4px radius, 56px tall, monospace 1.375rem, cobalt caret, `mute` placeholder, legend above.
- **Sizer fields:** the same at 50px and 1.125rem. The select draws its arrow from two ink triangles.
- **Focus, everywhere:** a 3px ink outline at 3px offset (2px on fields and code blocks); white inside the band and the footer.

### Chips, tags, code
- **Chip:** a key that can be checked again. Monospace 0.9375rem, 44px tall, 14px padding, white with an inset 1.5px ink ring, hover `off`. On the band it is transparent with an `on-soft` ring; hover and focus fill `on-deep` with a 2px white ring.
- **Tag:** FALSE POSITIVE, DEFINITELY ABSENT, POSSIBLY PRESENT at the start of a status sentence. White on ink; ink on white inside the band.
- **Code block:** `code-text` on ink, 4px radius, 20px by 22px padding, comments in `code-comment`, focusable so it scrolls by keyboard. Inline code has a `tint` ground on white, white on the tint, `on-deep` on the band.
- **Install command:** monospace in a white box with a 2px ink border, 48px minimum height, beside the small key.

### Navigation and footer
- **Masthead:** the MILL5 logo file at 124px, the product name in condensed 600 behind a 1px `off-edge` divider, then Barlow 600 links in ink with 44px targets; hover underlines them with a 2px cobalt line. No bar, no ground, no rule beneath.
- **FAQ:** native `details` between 1px `off-edge` hairlines, 60px rows, condensed 1.25rem questions that turn cobalt on hover, a chevron that turns in 140ms.
- **Imprint:** the portrait file (110 to 168px, 4px radius) beside a headline, a sentence and plain Barlow 600 links.
- **Footer:** navy, `on-soft` fine print, white underlined links.

### Motion
Every animation shows a bit being read or written: a cell landing, a mark dropping, a grid cell flipping. All use `cubic-bezier(0.16, 1, 0.3, 1)`. Under `prefers-reduced-motion` all animation and transition is off, the answer appears complete, and the 10,000 run is drawn in one step.

### Social card
`assets/social-card.html` loads the site stylesheet at 1200 by 630: the logo at 150px, the headline at 82px, seven 56px cells for "bloom", and the bit picture at 446px with its seven marks. Render `social-card.png` from it.

## Do's and Don'ts

### Do:
- **Do** draw any new state as cells: `off` for 0, `on` for 1, square, 4px radius, monospace numeral.
- **Do** keep text on white, `tint`, the band or the navy footer, with the pairs measured above.
- **Do** give every yellow mark a navy edge and a text equivalent.
- **Do** invert cells, tags, keys and focus outlines inside the band.
- **Do** separate with a ground change, a 2px ink rule or a 1px `off-edge` hairline.
- **Do** paint pictures of bits from the real snapshot, one canvas pixel per device pixel, with colours read from the custom properties.
- **Do** use the real brand files: `docs/site/assets/mill5-logo.png`, `docs/site/assets/richard-crane.jpg`, and the NuGet package icon `M5.BloomFilter/bitarray_400-min.png` (shipped as `icon-64.png` and `icon-180.png`).
- **Do** render `social-card.png` from `assets/social-card.html` whenever the headline or figures change.

### Don't:
- **Don't** draw, generate, trace or approximate a logo, mark, portrait or icon. If an asset is missing, ask for the file.
- **Don't** bring back the punched cards, the fan-fold paper, or any other metaphor that has to be explained before the filter is.
- **Don't** add a second accent hue, or use `mark` for emphasis, links, buttons, backgrounds or text.
- **Don't** add a second cobalt band, or a second tinted section, to a page.
- **Don't** add a cast shadow, blur, gradient or texture, or box a section as a card.
- **Don't** introduce another radius, or a cell that is not square.
- **Don't** set sentences in monospace or in uppercase condensed, and don't put a legend above a heading.
- **Don't** set text in `off-edge`, `off` or `mark`, or soft ink on the band.
- **Don't** load fonts, scripts or images from another origin.
