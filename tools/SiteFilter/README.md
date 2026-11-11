# SiteFilter

Builds `docs/site/words.json`, the snapshot the GitHub Page queries. It is a
Bloom filter over the ENABLE word list, built with `FilterBuilder.Build(count, 0.01)`
and written with `BloomFilterJsonConverter`, the core package's JSON format.

The project is not in the solution and is never packed.

## Word list

| | |
|---|---|
| List | ENABLE (Enhanced North American Benchmark Lexicon), `enable1.txt` |
| Source | <https://raw.githubusercontent.com/dolph/dictionary/694bf9529e986f9175dcbf5fe5875d111941b708/enable1.txt> |
| SHA-256 | `3f16130220645692ed49c7134e24a18504c2ca55b3c012f7290e3e77c63b1a89` |
| Entries | 172,823 distinct lower-case words, 1,743,363 bytes |
| Licence | Public domain. The ENABLE release notice reads: "The ENABLE master word list, WORD.LST, is herewith formally released into the Public Domain. Anyone is free to use it or distribute it in any manner they see fit." It is quoted in [Debian's SCOWL copyright file](https://sources.debian.org/src/scowl/7.1-1/Copyright/). |

The list is not committed. `words.json` holds bits only; no word can be read
back out of it.

## Regenerate

```sh
curl -L -o /tmp/enable1.txt https://raw.githubusercontent.com/dolph/dictionary/694bf9529e986f9175dcbf5fe5875d111941b708/enable1.txt
dotnet run -c Release --project tools/SiteFilter -- /tmp/enable1.txt docs/site/words.json
node tools/SiteFilter/check-nonsense.mjs /tmp/enable1.txt
```

The tool refuses to run if the file's SHA-256 or entry count differs, and
refuses to write a snapshot in which any list entry tests absent. It prints the
filter's sizes and occupancy, a false-positive count over one million fixed
non-words, and the positions of a few fixed words.

Those printed vectors live in three places that must agree:

- `docs/site/vectors.js`, checked by the page when it starts;
- `M5.BloomFilter.UnitTests/SiteVectorTests.cs`, checked against the library;
- the numbers quoted in `docs/site/index.html` and `docs/site/llms.txt`.

If the vectors in the page and the library ever disagree, the page shows "The
filter is switched off" instead of answering.

`vectors.js` also carries the size of the page's 64-bit teaching filter
(`{ n: 14, p: 0.113, m: 64, k: 3 }`) and `loxzo`, a string that is not in the
list and that the filter accepts: the false positive the page offers as an
example. `SiteVectorTests` asserts both against the library.

`check-nonsense.mjs` runs the page's made-up-word generator
(`docs/site/nonsense.js`) five million times against the list and fails if it
ever produces a real entry. That is what lets the page call a string that
passes the filter a false positive.
