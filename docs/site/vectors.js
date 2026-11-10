// Positions for fixed words in words.json (m = 1,657,884, k = 7), printed by
// tools/SiteFilter. SiteVectorTests.cs asserts the same values against the C#
// library; the page checks them against bloom.js before it answers anything.
window.SITE_VECTORS = [
  { word: 'bloom', present: true, positions: [1110165, 768828, 427491, 86154, 1402701, 1061364, 720027] },
  { word: 'filter', present: true, positions: [21372, 908830, 138404, 1025862, 255436, 1142894, 372468] },
  { word: 'zyzzyva', present: true, positions: [498505, 81862, 1323103, 906460, 489817, 73174, 1314415] },
  { word: '', present: false, positions: [1446201, 1531357, 1616513, 43785, 128941, 214097, 299253] },
  { word: 'naïve', present: false, positions: [1165087, 484652, 1462101, 781666, 101231, 1078680, 398245] },
  { word: '😀', present: false, positions: [1371407, 215759, 717995, 1220231, 64583, 566819, 1069055] },
  { word: 'definitelynotaword', present: false, positions: [234789, 332233, 429677, 527121, 624565, 722009, 819453] }
];

// Statistics(n, p) from the C# library, for the sizing calculator. A null m
// means the library throws because the filter would exceed Int32 bits.
window.SITE_SIZES = [
  { n: 1, p: 0.5, m: 2, k: 1 },
  { n: 1000, p: 0.01, m: 9593, k: 7 },
  { n: 12345, p: 0.05, m: 77119, k: 4 },
  { n: 172823, p: 0.01, m: 1657884, k: 7 },
  { n: 1000000, p: 0.1, m: 4808328, k: 3 },
  { n: 1000000, p: 0.01, m: 9592955, k: 7 },
  { n: 1000000, p: 0.001, m: 14377640, k: 10 },
  { n: 1000000, p: 0.0001, m: 19172955, k: 13 },
  { n: 1000000, p: 0.000001, m: 28755279, k: 20 },
  { n: 50000000, p: 0.0001, m: 958647740, k: 13 },
  { n: 223000000, p: 0.01, m: 2139228902, k: 7 },
  { n: 2147483647, p: 0.1, m: null, k: null },
  { n: 250000000, p: 0.000001, m: null, k: null }
];
