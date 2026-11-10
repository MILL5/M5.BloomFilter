// Proves the page's nonsense generator cannot produce a real list entry, so a
// string that passes the filter is a false positive and not a word.
// Usage: node tools/SiteFilter/check-nonsense.mjs <enable1.txt> [samples]
import { readFileSync } from 'node:fs';
import { createRequire } from 'node:module';

const require = createRequire(import.meta.url);
const { make, joins } = require('../../docs/site/nonsense.js');

const [, , listPath, samplesArg] = process.argv;
if (!listPath) {
  console.error('usage: node check-nonsense.mjs <enable1.txt> [samples]');
  process.exit(2);
}

const entries = readFileSync(listPath, 'ascii').split('\n').map(w => w.trim()).filter(Boolean);
const words = new Set(entries);
const samples = Number(samplesArg ?? 5_000_000);
let failed = false;

// 1. No list entry contains any of the generator's letter pairs.
for (const join of joins) {
  const example = entries.find(word => word.includes(join));
  if (example) { console.error(`FAIL "${join}" occurs in the list, for example in "${example}"`); failed = true; }
}

// 2. Every generated string contains one of those pairs (and, redundantly, is not an entry).
let state = 0x2545f491;
const random = () => {
  state ^= state << 13; state ^= state >>> 17; state ^= state << 5;
  return (state >>> 0) / 4294967296;
};

let shortest = Infinity, longest = 0, withoutJoin = 0, real = 0;
const seen = [];
for (let i = 0; i < samples; i++) {
  const word = make(random);
  shortest = Math.min(shortest, word.length);
  longest = Math.max(longest, word.length);
  if (!joins.some(join => word.includes(join))) withoutJoin++;
  if (words.has(word)) real++;
  if (i < 8) seen.push(word);
}
if (withoutJoin || real || shortest < 9 || longest > 12) failed = true;

console.log(`${joins.length} letter pairs, none in ${entries.length.toLocaleString('en-US')} entries: ${failed ? 'see failures' : 'confirmed'}`);
console.log(`${samples.toLocaleString('en-US')} generated, lengths ${shortest}-${longest}, without a pair: ${withoutJoin}, real entries: ${real}`);
console.log('for example: ' + seen.join(' '));
process.exit(failed ? 1 : 0);
