using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace M5.BloomFilter.UnitTests
{
    /// <summary>
    /// The GitHub Page answers with a JavaScript port of the default hash, checked
    /// at start-up against docs/site/vectors.js. These tests hold that file and the
    /// page's snapshot, docs/site/words.json, to what the library itself computes.
    /// </summary>
    [TestClass]
    public class SiteVectorTests
    {
        private static string Site(string name) => Path.Combine(AppContext.BaseDirectory, "Fixtures", "site", name);

        private static IImmutableBloomFilter Snapshot()
            => JsonConvert.DeserializeObject<IImmutableBloomFilter>(File.ReadAllText(Site("words.json")), new BloomFilterJsonConverter());

        // { word: 'bloom', present: true, positions: [1, 2, 3] }
        private static IEnumerable<(string Word, bool Present, int[] Positions)> Vectors()
        {
            var pattern = new Regex(@"\{ word: '((?:[^'\\]|\\u[0-9A-Fa-f]{4})*)', present: (true|false), positions: \[([0-9, ]+)\] \}");
            foreach (Match match in pattern.Matches(File.ReadAllText(Site("vectors.js"))))
            {
                string word = Regex.Replace(match.Groups[1].Value, @"\\u([0-9A-Fa-f]{4})",
                    escape => ((char)int.Parse(escape.Groups[1].Value, NumberStyles.HexNumber)).ToString());
                yield return (word, match.Groups[2].Value == "true",
                    match.Groups[3].Value.Split(',').Select(text => int.Parse(text.Trim(), CultureInfo.InvariantCulture)).ToArray());
            }
        }

        // { n: 1000, p: 0.01, m: 9593, k: 7 }  or  { n: ..., p: ..., m: null, k: null }
        private static IEnumerable<(long N, double P, int? M, int? K)> Sizes()
        {
            var pattern = new Regex(@"\{ n: (\d+), p: ([0-9.]+), m: (\d+|null), k: (\d+|null) \}");
            foreach (Match match in pattern.Matches(File.ReadAllText(Site("vectors.js"))))
            {
                yield return (long.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture),
                    double.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture),
                    match.Groups[3].Value == "null" ? null : int.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture),
                    match.Groups[4].Value == "null" ? null : int.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture));
            }
        }

        [TestMethod]
        public void PageSnapshotIsWhatThePageSaysItIs()
        {
            var snapshot = Snapshot();
            Assert.AreEqual(1_657_884, snapshot.Statistics.M);
            Assert.AreEqual(7, snapshot.Statistics.K);
            Assert.AreEqual(172_823, snapshot.Statistics.N);
            Assert.AreEqual(HashMethod.Murmur3KirschMitzenmacher, snapshot.Hash.ToHashMethod());

            var built = FilterBuilder.Build(172_823, 0.01).Statistics;
            Assert.AreEqual(snapshot.Statistics.M, built.M);
            Assert.AreEqual(snapshot.Statistics.K, built.K);

            // Figures quoted on the page: 207 kB, 51.8% of bits on, 1.00%.
            var occupancy = snapshot.GetOccupancy();
            Assert.AreEqual(859_023, occupancy.SetBits);
            Assert.AreEqual(207_236, (snapshot.Statistics.M + 7) / 8);
            Assert.AreEqual("51.8", (occupancy.FillRatio * 100).ToString("0.0", CultureInfo.InvariantCulture));
            Assert.AreEqual("1.00", (occupancy.EstimatedFalsePositiveRate * 100).ToString("0.00", CultureInfo.InvariantCulture));
        }

        [TestMethod]
        public void PageVectorsAreTheLibrarysPositionsAndAnswers()
        {
            var snapshot = Snapshot();
            var vectors = Vectors().ToArray();
            Assert.AreEqual(8, vectors.Length);
            Assert.IsTrue(vectors.Any(vector => vector.Word == "naïve") && vectors.Any(vector => vector.Word == "\U0001F600"));
            foreach (var (word, present, positions) in vectors)
            {
                CollectionAssert.AreEqual(positions, snapshot.ComputeHash(word), word);
                Assert.AreEqual(present, snapshot.Contains(word), word);
                Assert.AreEqual(present, snapshot.ContainsKey(word.AsSpan()), word);
            }
        }

        [TestMethod]
        public void PageSizingVectorsAreTheLibrarysStatistics()
        {
            var sizes = Sizes().ToArray();
            Assert.AreEqual(14, sizes.Length);
            foreach (var (n, p, m, k) in sizes)
            {
                if (m == null)
                {
                    Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Statistics(n, p), $"n={n} p={p}");
                    continue;
                }

                var statistics = new Statistics(n, p);
                Assert.AreEqual(m.Value, statistics.M, $"n={n} p={p}");
                Assert.AreEqual(k.Value, (int)statistics.K, $"n={n} p={p}");
            }
        }
    }
}
