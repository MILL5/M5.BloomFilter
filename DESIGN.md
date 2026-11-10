---
name: M5.BloomFilter
description: A Bloom filter for .NET 10, shown as a 1970 deck of punched cards and its fan-fold printout.
colors:
  tray: "#0b4c8c"
  tray-deep: "#083a6d"
  tray-text: "#ffffff"
  tray-soft: "#cfe0f3"
  manila: "#e7d6a3"
  manila-light: "#f3e8c4"
  manila-edge: "#c2ad72"
  form-ink: "#8b7a4a"
  paper: "#f4f6f0"
  paper-edge: "#cfd6c8"
  bar: "#dbe9d2"
  salmon: "#f1b79e"
  salmon-edge: "#c98a70"
  salmon-ink: "#4a2a1e"
  ink: "#14212b"
  ink-soft: "#3b4a56"
  slate: "#5d6770"
  comment-ink: "#5b4d27"
  pencil: "#ab1f17"
  key: "#1b2b3a"
  key-hover: "#26394b"
  key-text: "#f6f0dc"
typography:
  display: { fontFamily: "Barlow Condensed, Arial Narrow, Helvetica Neue, Arial, sans-serif", fontSize: "clamp(2.5rem, 6.1vw, 5.2rem)", fontWeight: 800, lineHeight: 0.97, letterSpacing: "-0.012em" }
  verdict: { fontFamily: "Barlow Condensed, Arial Narrow, Helvetica Neue, Arial, sans-serif", fontSize: "clamp(2.4rem, 6.4vw, 5rem)", fontWeight: 800, lineHeight: 0.98, letterSpacing: "-0.01em" }
  headline: { fontFamily: "Barlow Condensed, Arial Narrow, Helvetica Neue, Arial, sans-serif", fontSize: "clamp(2rem, 3.8vw, 2.9rem)", fontWeight: 600, lineHeight: 1.04, letterSpacing: "-0.005em" }
  title: { fontFamily: "Barlow Condensed, Arial Narrow, Helvetica Neue, Arial, sans-serif", fontSize: "1.375rem", fontWeight: 600, lineHeight: 1.2, letterSpacing: "0.005em" }
  standfirst: { fontFamily: "Barlow, Segoe UI, Helvetica Neue, Arial, sans-serif", fontSize: "clamp(1.0625rem, 1.5vw, 1.25rem)", fontWeight: 400, lineHeight: 1.5 }
  body: { fontFamily: "Barlow, Segoe UI, Helvetica Neue, Arial, sans-serif", fontSize: "1.125rem", fontWeight: 400, lineHeight: 1.6 }
  fine: { fontFamily: "Barlow, Segoe UI, Helvetica Neue, Arial, sans-serif", fontSize: "0.9375rem", fontWeight: 400, lineHeight: 1.5 }
  label: { fontFamily: "Barlow Condensed, Arial Narrow, Helvetica Neue, Arial, sans-serif", fontSize: "0.8125rem", fontWeight: 500, lineHeight: 1.2, letterSpacing: "0.06em" }
  key: { fontFamily: "Barlow Condensed, Arial Narrow, Helvetica Neue, Arial, sans-serif", fontSize: "1.125rem", fontWeight: 600, lineHeight: 1, letterSpacing: "0.07em" }
  typed: { fontFamily: "ui-monospace, Cascadia Mono, Cascadia Code, Consolas, SF Mono, Menlo, DejaVu Sans Mono, monospace", fontSize: "1.375rem", fontWeight: 400, lineHeight: 1.2 }
  listing: { fontFamily: "ui-monospace, Cascadia Mono, Cascadia Code, Consolas, SF Mono, Menlo, DejaVu Sans Mono, monospace", fontSize: "0.9375rem", fontWeight: 400, lineHeight: "1.6rem", fontFeature: "tnum" }
  code: { fontFamily: "ui-monospace, Cascadia Mono, Cascadia Code, Consolas, SF Mono, Menlo, DejaVu Sans Mono, monospace", fontSize: "0.875rem", fontWeight: 400, lineHeight: 1.65 }
rounded:
  none: "0"
  print: "2px"
  keycap: "5px"
spacing:
  gutter: "clamp(14px, 3.2vw, 32px)"
  column: "1180px"
  line: "1.6rem"
  corner: "clamp(18px, 2.4vw, 30px)"
  feed: "clamp(14px, 3.4vw, 40px)"
  measure: "66ch"
  target: "56px"
  target-min: "44px"
components:
  key: { backgroundColor: "{colors.key}", textColor: "{colors.key-text}", typography: "{typography.key}", rounded: "{rounded.keycap}", padding: "0 22px", height: "56px" }
  key-hover: { backgroundColor: "{colors.key-hover}" }
  key-disabled: { backgroundColor: "{colors.slate}", textColor: "{colors.key-text}" }
  key-small: { backgroundColor: "{colors.key}", textColor: "{colors.key-text}", rounded: "{rounded.keycap}", padding: "0 16px", height: "44px" }
  field: { backgroundColor: "{colors.manila-light}", textColor: "{colors.ink}", typography: "{typography.typed}", rounded: "{rounded.print}", padding: "8px 14px", height: "56px" }
  field-on-paper: { backgroundColor: "{colors.tray-text}", textColor: "{colors.ink}", rounded: "{rounded.print}", padding: "8px 14px", height: "50px" }
  chip: { backgroundColor: "{colors.manila}", textColor: "{colors.ink}", rounded: "{rounded.print}", padding: "0 12px", height: "44px" }
  chip-hover: { backgroundColor: "{colors.manila-light}" }
  card-stock: { backgroundColor: "{colors.manila}", textColor: "{colors.ink}", rounded: "{rounded.none}" }
  card-limits: { backgroundColor: "{colors.salmon}", textColor: "{colors.ink}", rounded: "{rounded.none}" }
  fanfold-sheet: { backgroundColor: "{colors.paper}", textColor: "{colors.ink}", rounded: "{rounded.none}" }
  listing: { backgroundColor: "{colors.paper}", textColor: "{colors.ink}", typography: "{typography.listing}", padding: "0 14px" }
  code-block: { backgroundColor: "{colors.manila}", textColor: "{colors.ink}", typography: "{typography.code}", padding: "14px 16px" }
  reproduce-panel: { backgroundColor: "{colors.tray-deep}", textColor: "{colors.tray-soft}", padding: "16px 18px" }
---

# Design System: M5.BloomFilter

## Overview

**Creative North Star: "The Deck and the Printout"**

The site is a 1970 machine room seen from above. The ground is enamel blue from edge to edge. Manila punched cards lie on it, and long content runs on fan-fold printout paper with sprocket margins. A hole in a card is not drawn as a dark mark; it is the blue ground showing through. Every surface is one of three stocks (card, paper, tray) and every piece of type is one of three faces.

Density is that of a printed form: full-width cards, tables set one printout line per row, short measures of reading text beside the listing they explain. Controls are flat ink keys and printed fields. The only moving part is the reader feeding a card. Recorded from the shipped `docs/site/style.css`, `app.js` and `assets/social-card.html`.

**Key Characteristics:**
- Blue ground, manila cards, pale fan-fold paper. No white page.
- Cards keep a real card's proportion (7.375 by 3.25) and a top-left corner cut at sixty degrees.
- Holes are filled with the ground colour.
- Condensed sans for headings, legends and keys; humanist sans for reading; platform monospace for code, typed input and printout data.
- Red grease pencil marks a blank hole and nothing else, and always with a word beside it.
- Brand marks are the real files. Nothing is drawn or generated to stand in for one.

## Colors

Three paper stocks on one saturated ground, with dark blue-black ink and one red.

### Primary
- **Tray Blue** (`tray`): the page ground, the `theme-color`, the fill of every punched hole and sprocket hole. Taken from the blue of the package icon and the MILL5 mark.
- **Deep Tray** (`tray-deep`): the recessed panel on the ground that holds the C# reproduction snippet, with a 1px white border at 22% alpha. Also the scrollbar track.
- **Tray Text** and **Tray Soft** (`tray-text`, `tray-soft`): white for the verdict and emphasis on the ground; pale blue for secondary text on the ground.

### Secondary
- **Manila** (`manila`): card stock. Header card, imprint card, code blocks, chips, the probe card. Always carries the grain texture.
- **Manila Light** (`manila-light`): a field printed on a card, the install command box, chip hover.
- **Manila Edge** (`manila-edge`): rules printed on a card, the portrait border, the edge lines of the deck strip, the scrollbar thumb.
- **Form Ink** (`form-ink`): preprinted digits and column numbers on the canvas card. It is 2.9:1 on manila by intent, like a real form. Never set readable text in it.

### Tertiary
- **Salmon** (`salmon`, `salmon-edge`, `salmon-ink`): the limits card, its item rules, and its edge legend.
- **Grease Pencil** (`pencil`): the ring, crosshair and BLANK tag on the probe card, the BLANK cell in a listing, the "no" lamp, the cap on a blank card in the deck strip, and the field caret.

### Neutral
- **Fan-fold Paper** (`paper`, `paper-edge`): the printout sheets; dashed perforations and hairlines.
- **Green Bar** (`bar`): the bands under listing rows.
- **Ink** and **Soft Ink** (`ink`, `ink-soft`): all text on card and paper; legends, notes and fine print.
- **Slate** (`slate`): placeholder text and the disabled key. **Comment Ink** (`comment-ink`): code comments on manila.
- **Keycap** (`key`, `key-hover`, `key-text`): the keys.

`slate`, `comment-ink`, `salmon-ink`, `key-hover` and white are literals in the stylesheet, not custom properties. Measured contrast: ink on manila 11.3:1, soft ink on manila 6.3:1, pencil on manila 5.0:1, tray-soft on tray 6.4:1, white on tray 8.7:1, key text on keycap 12.7:1.

### Named Rules
**The Hole Shows Blue Rule.** A hole is painted in `tray`, the same colour as the ground behind the card. Never draw a hole in black, grey or a shadow.

**The One Salmon Card Rule.** Salmon is the limits card. One per page.

**The Grease Pencil Rule.** `pencil` means a blank hole, which means "definitely not". It always appears with text (BLANK, the lamp sentence, the ring's tag). It is 1.2:1 against `tray`, so it is only read on card or paper.

## Typography

**Display Font:** Barlow Condensed 500, 600, 800 (with Arial Narrow, Helvetica Neue, Arial)
**Body Font:** Barlow 400, 600 (with Segoe UI, Helvetica Neue, Arial)
**Label/Mono Font:** the platform monospace (`ui-monospace`, Cascadia Mono, Consolas, SF Mono, Menlo)

**Character:** A narrow sans that reads as form printing and key lettering, beside its regular-width sibling for sentences. Five woff2 files are self-hosted in `docs/site/assets/fonts/` under the OFL; two are preloaded. The page requests nothing from another origin.

### Hierarchy
- **Display** (800, clamp 2.5rem to 5.2rem, 0.97): the one `h1`, in ink on the header card. Balanced wrap; figures do not break.
- **Verdict** (800, clamp 2.4rem to 5rem, 0.98): the filter's answer, in white on the tray.
- **Headline** (600, clamp 2rem to 2.9rem, 1.04): section `h2`, capped at 24ch on a sheet and 16ch on the limits card.
- **Title** (600, 1.375rem, 1.2): `h3`, the reproduce panel heading. FAQ questions and the product name use the same face at 1.25rem.
- **Standfirst** (400, clamp 1.0625rem to 1.25rem, 1.5): the sentence under the headline and the reason under the verdict.
- **Body** (400, 1.125rem, 1.6; 1.0625rem at 620px and below): reading text, 34rem wide beside a listing, 66ch when stacked.
- **Fine** (400, 0.9375rem, 1.5, soft ink): notes, citations, benchmark conditions, footer.
- **Label** (500, 0.8125rem, 0.06em, uppercase): card legends, field labels, listing column heads. Listing captions use 600 at 1rem, 0.05em; print-line navigation uses 500 at 1rem, 0.05em.
- **Typed / Listing / Code** (mono 400 at 1.375rem, 0.9375rem and 0.875rem): the word field, table data with tabular figures, and code.

### Named Rules
**The Three Faces Rule.** Condensed is for what a form prints or a key says. Barlow is for sentences. Monospace is only for code, what the visitor types, and data a machine printed.

**The Legend Rule.** An uppercase legend labels a field or signs the edge of a card. It never sits above a heading as a category line.

## Layout

One centred column, `min(1180px, 100% - 2 × gutter)`, with a gutter of `clamp(14px, 3.2vw, 32px)`. The page is a vertical run of objects on the tray: header card, the reader (verdict, probe card, deck strip, listing), a fan-fold run of sheets, the salmon card, a second fan-fold run for the FAQ, the imprint card, then a footer set directly on the tray.

Inside a sheet, a **spread** puts commentary on the left (up to 32rem) and its listing or code on the right, 28 to 52px apart. Vertical rhythm comes from the printout line (`1.6rem`): every listing row is exactly one line tall. Section padding uses clamps from about 34px to 68px. Interactive targets are 56px tall in the header card and never under 44px. Wide tables scroll inside their own listing; the page never scrolls sideways.

Responsive changes, as built:
- **1020px and below:** verdict and reason stack. **1000px and below:** spreads stack and prose widens to 66ch.
- **860px and below:** field and install stack; listing and reproduce panel stack; limits go to one column; the one-line lamp appears under the word field because the verdict may be below the fold.
- **620px and below:** body 1.0625rem; the header card's install block moves below the deck strip onto the tray; navigation wraps to its own row; the key goes full width; listings drop to 0.8125rem with 8px cell padding; code wraps at 0.75rem; the imprint card stacks; sprocket holes repeat every 30px instead of 44px; the deck strip is 76px tall instead of 84px.
- **380px and below:** listings 0.75rem with 5px cell padding.
- **The Thirty-Column Window.** Below 620px the probe card is not shrunk. The frame changes to 2.766 by 3.25 (thirty of eighty columns), loses its corner cut, and the canvas shows thirty columns at reading size, centred on the hole in question and clamped to the card's ends. The frame proportion and the windowed drawing must switch together.

## Elevation & Depth

Paper lying on a surface. **The Lying Flat Rule.** Only a whole card or a whole run of fan-fold paper lifts off the tray; nothing inside one has a shadow. Surfaces within a card or sheet are flat and separated by 1px printed rules or dashed perforations. Controls have no shadow: a key's press is a 2px move down.

### Shadow Vocabulary
- **Lift** (`filter: drop-shadow(0 1px 0 rgb(3 18 36 / 0.45)) drop-shadow(0 10px 14px rgb(3 18 36 / 0.3))`): a 1px edge for the thickness of the stock and a soft cast below. Applied to a wrapper, because the card inside is clipped. The social card uses a heavier version at its 1200px size.
- **Focus** (`outline: 3px solid #fff; outline-offset: 2px; box-shadow: 0 0 0 8px ink`): a white ring inside an ink halo, visible on tray, card and paper alike. Inside listings and code blocks it is a 3px ink outline.
- **Current row** (`inset 0 -2px 0 ink, inset 0 2px 0 ink`): two ruled lines around the listing row whose card is showing.

## Shapes

Square paper. Cards and sheets have no radius. A card's top-left corner is cut with `clip-path` at sixty degrees: `corner` across and `corner × 1.73` down, where `corner` is `clamp(18px, 2.4vw, 30px)`; the probe card uses the true 0.25in by 0.433in cut as percentages. Printed things (fields, chips, the install box) have a 2px radius. Keys have 5px. The lamp is the one circle.

Card stock carries an inline SVG noise texture (`--grain`) on every manila and salmon surface; on the canvas card it is multiplied over the drawing. Fan-fold paper has a feed margin each side (`clamp(14px, 3.4vw, 40px)`) with round sprocket holes of radius 0.2 × feed in tray blue, a 1px dashed inner edge, and a 2px dashed perforation between sheets.

## Components

### Keys
Flat ink keycaps with cream uppercase condensed lettering.
- **Shape:** 5px radius, no border, no shadow, 56px tall, 22px side padding. Small key: 44px, 16px padding, 1rem.
- **Hover / Active / Disabled:** background to `key-hover` in 120ms; sinks 2px in 80ms; `slate` background when disabled.

### Fields
- **Card field:** `manila-light`, 2px ink border, 2px radius, 56px tall, monospace 1.375rem, pencil-red caret, slate placeholder.
- **Paper field:** the same on white, 50px tall, 1.125rem, with a label legend above. Selects draw their arrow from two ink triangles.

### Cards
- **Stock:** manila with grain, cut corner, inside a lift wrapper. Padding clamps from about 18px to 60px.
- **Print line:** the header card's top row: the MILL5 logo at 118px, the product name, uppercase navigation with a 2px ink underline on hover, closed by a 1px `manila-edge` rule. Indented past the corner cut.
- **Limits card:** salmon stock, a two-column ordered list with a 1px `salmon-edge` rule over each item, and an edge legend in `salmon-ink`.
- **Imprint card:** the portrait (110 to 176px, 1px `manila-edge` border) beside a heading, a sentence and plain links.

### Probe card (canvas)
Drawn in `app.js` at up to 3× device pixels, with colours read from the custom properties. Eighty columns by twelve rows at 0.087in and 0.25in pitch; holes 0.055in by 0.125in in tray blue. Digits 0 to 9 are preprinted in `form-ink` when a column is at least 5.5px wide, column numbers at 11px; below that the digit rows become faint rules. The keypunch line along the top is monospace capitals: the typed word at the left, `CARD 0001 OF n` at the right. The hole in question gets a slightly tilted pencil ellipse, thin crosshair lines from the top and left edges, and a tag reading PUNCHED (ink) or BLANK (pencil) in white condensed capitals. The caption and the listing state the same in text.

**Feed motion:** a new card arrives in 200ms on `cubic-bezier(0.16, 1, 0.3, 1)` from 4% right, 3% up, 0.5° rotation and 25% opacity. A submitted word feeds its cards 210ms apart and stops on the one that decides the answer. With reduced motion the deciding card appears at once.

**Deck strip:** a second canvas shows the whole deck edge-on: a 28px manila band with a `manila-edge` line every 3px. Each probed card stands 14px or 28px above it (staggered when two are close), 6px wide, numbered in condensed 600. The card on show is white; a blank one has a pencil cap.

### Listing
Fan-fold table. Paper ground, green bars three lines on and three off starting under the caption, monospace with tabular figures, every row one printout line, numbers right-aligned, the first column left. Caption and column heads are condensed uppercase. A BLANK state is pencil red, bold and capitalised. The row number is an underlined button that recalls its card.

### Code block, chips, FAQ
- **Code block:** manila with grain under a 3px ink top rule; comments in `comment-ink`; focusable so it can be scrolled by keyboard.
- **Chips:** manila with grain, 1px ink border, 2px radius, 44px tall, monospace. Each is a made-up word that got through and can be read again.
- **FAQ:** native `details` between 1px `paper-edge` rules, condensed 1.25rem questions, a chevron made from two ink borders that turns in 140ms.

## Do's and Don'ts

### Do:
- **Do** put new content on one of the three stocks: a manila card on the tray, a fan-fold sheet, or directly on the tray in white and `tray-soft`.
- **Do** fill holes with `tray` and keep any card at 7.375 by 3.25 with eighty columns and twelve rows.
- **Do** set tabular data in a listing with rows one `1.6rem` line tall and green bars every three lines.
- **Do** pair every use of `pencil` with a word, and every state with text.
- **Do** use the real brand files: `docs/site/assets/mill5-logo.png`, `docs/site/assets/richard-crane.jpg`, and the NuGet package icon `M5.BloomFilter/bitarray_400-min.png` (shipped as `icon-64.png` and `icon-180.png`).
- **Do** render `social-card.png` from `assets/social-card.html`, which loads the site stylesheet and punches a real card from the deck.

### Don't:
- **Don't** draw, generate, trace or approximate a logo, mark, portrait or icon. If an asset is missing, ask for the file.
- **Don't** add a white or grey page background, or bring back the earlier white and lilac look.
- **Don't** add a second salmon card or use salmon as an accent.
- **Don't** use `pencil` for emphasis, links, errors in general or decoration, and don't place it on the tray.
- **Don't** set readable text in `form-ink`.
- **Don't** give controls, tables, code blocks or anything inside a card its own shadow, or round a card or sheet.
- **Don't** set sentences in monospace or in uppercase condensed, and don't put a legend above a heading.
- **Don't** load fonts, scripts or images from another origin.
