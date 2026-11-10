// Checks that the GitHub Page says what the snapshot and the library say.
// Usage: node tools/SiteFilter/check-page.mjs
import { readFileSync } from 'node:fs';
import { createRequire } from 'node:module';
import { dirname, join } from 'node:path';
import { fileURLToPath } from 'node:url';

const require = createRequire(import.meta.url);
const site = join(dirname(fileURLToPath(import.meta.url)), '..', '..', 'docs', 'site');
const Bloom = require(join(site, 'bloom.js'));

const failures = [];
const check = (ok, message) => { if (!ok) failures.push(message); };
const text = html => html.replace(/<[^>]+>/g, '').replace(/&lt;/g, '<').replace(/&gt;/g, '>').replace(/&amp;/g, '&')
  .replace(/&quot;/g, '"').replace(/\s+/g, ' ').trim();

const html = readFileSync(join(site, 'index.html'), 'utf8');
const snapshot = JSON.parse(readFileSync(join(site, 'words.json'), 'utf8'));
const filter = Bloom.load(snapshot);

// 1. The JavaScript port reproduces the vectors the C# tests assert.
globalThis.window = {};
new Function('window', readFileSync(join(site, 'vectors.js'), 'utf8'))(globalThis.window);
const vectors = globalThis.window.SITE_VECTORS;
check(Array.isArray(vectors) && vectors.length >= 5, 'vectors.js must define SITE_VECTORS');
for (const vector of vectors ?? []) {
  const got = Bloom.positions(vector.word, filter.m, filter.k).positions;
  check(JSON.stringify(got) === JSON.stringify(vector.positions), `positions differ for "${vector.word}"`);
  check(filter.contains(vector.word) === vector.present, `membership differs for "${vector.word}"`);
}

// 1b. The sizing calculator reproduces Statistics(n, p) from the C# library.
const sizes = globalThis.window.SITE_SIZES;
check(Array.isArray(sizes) && sizes.length >= 10, 'vectors.js must define SITE_SIZES');
for (const expected of sizes ?? []) {
  const got = Bloom.size(expected.n, expected.p);
  const same = expected.m === null ? got === null : got !== null && got.m === expected.m && got.k === expected.k;
  check(same, `size(${expected.n}, ${expected.p}) gives ${JSON.stringify(got)}, C# gives m=${expected.m} k=${expected.k}`);
}

// 2. Every quoted figure matches the snapshot.
const fmt = n => Math.round(n).toLocaleString('en-US');
const cards = Math.ceil(filter.m / 960);
const fill = filter.countSetBits() / filter.m;
const stats = {
  words: fmt(filter.n),
  bits: fmt(filter.m),
  cards: fmt(cards),
  kilobytes: fmt(Math.ceil(filter.m / 8) / 1000),
  fill: (fill * 100).toFixed(1) + '%',
  estimate: (Math.pow(fill, filter.k) * 100).toFixed(2) + '%',
};
for (const [name, expected] of Object.entries(stats)) {
  const found = [...html.matchAll(new RegExp(`data-stat="${name}">([^<]*)<`, 'g'))].map(m => m[1]);
  check(found.length > 0, `no data-stat="${name}" on the page`);
  for (const value of found) check(value === expected, `data-stat="${name}" says ${value}, snapshot says ${expected}`);
}
check(!/\{\{[A-Z_]+\}\}/.test(html), 'unfilled {{PLACEHOLDER}} left in index.html');
check(filter.k === 7, 'the page copy says seven holes; the snapshot has k = ' + filter.k);
check(snapshot.v === 2 && snapshot.hm === 12, 'snapshot must be format 2, hash method 12');

// 3. FAQ answers on the page equal the FAQPage structured data, word for word.
const jsonLd = JSON.parse(html.match(/<script type="application\/ld\+json">([\s\S]*?)<\/script>/)[1]);
const faqPage = jsonLd['@graph'].find(node => node['@type'] === 'FAQPage');
const visible = [...html.matchAll(/<details>\s*<summary>([\s\S]*?)<\/summary>\s*<p>([\s\S]*?)<\/p>\s*<\/details>/g)]
  .map(m => ({ question: text(m[1]), answer: text(m[2]) }));
check(visible.length === faqPage.mainEntity.length, `page has ${visible.length} questions, JSON-LD has ${faqPage.mainEntity.length}`);
faqPage.mainEntity.forEach((entry, index) => {
  check(visible[index]?.question === entry.name, `question ${index + 1} differs`);
  check(visible[index]?.answer === entry.acceptedAnswer.text, `answer ${index + 1} differs`);
});
const software = jsonLd['@graph'].find(node => node['@type'] === 'SoftwareSourceCode');
check(html.includes(`Version ${software.softwareVersion} for .NET 10`), 'softwareVersion differs from the version on the page');

// 4. Every local file the page references exists.
for (const match of html.matchAll(/(?:src|href)="([^"#:]+)"/g)) {
  try { readFileSync(join(site, match[1])); } catch { failures.push(`missing file ${match[1]}`); }
}

if (failures.length) {
  console.error(failures.map(f => 'FAIL ' + f).join('\n'));
  process.exit(1);
}
console.log(`ok: ${vectors.length} vectors, ${sizes.length} sizes, ${Object.keys(stats).length} figures, ${visible.length} FAQ entries, m=${filter.m} k=${filter.k}`);
