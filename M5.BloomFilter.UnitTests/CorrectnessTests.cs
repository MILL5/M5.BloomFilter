using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M5.BloomFilter.UnitTests
{
    [TestClass]
    public class CorrectnessTests
    {
        [DataRow(HashMethod.LCGWithFNV1)]
        [DataRow(HashMethod.LCGWithFNV1a)]
        [DataRow(HashMethod.LCGModifiedFNV1)]
        [DataRow(HashMethod.RNGWithFNV1)]
        [DataRow(HashMethod.RNGWithFNV1a)]
        [DataRow(HashMethod.RNGModifiedFNV1)]
        [DataRow(HashMethod.CRC32)]
        [DataRow(HashMethod.CRC32u)]
        [DataRow(HashMethod.Adler32)]
        [DataRow(HashMethod.Murmur2)]
        [DataRow(HashMethod.Murmur3)]
        [DataRow(HashMethod.Murmur3KirschMitzenmacher)]
        [DataRow(HashMethod.SHA1)]
        [DataRow(HashMethod.SHA256)]
        [DataRow(HashMethod.SHA384)]
        [DataRow(HashMethod.SHA512)]
        [TestMethod]
        [Timeout(30000)]
        public async Task EveryHashPreservesMembership(HashMethod method)
        {
            var filter = FilterBuilder.Build(300, 0.01, method);
            var values = Enumerable.Range(0, 256).Select(i => new[] { (byte)i })
                .Concat(new[] { Array.Empty<byte>(), Encoding.UTF8.GetBytes("東京🙂") }).ToArray();
            Assert.IsFalse(filter.Contains(values[0]));
            await filter.AddAsync(values);
            var before = filter.HashBits.Cast<bool>().ToArray();
            filter.Add(values);
            CollectionAssert.AreEqual(before, filter.HashBits.Cast<bool>().ToArray());
            Assert.IsTrue(filter.All(values));
            Assert.IsTrue(await filter.AllAsync(values));
            Assert.AreEqual(values.Length, filter.Contains(values).Count);
            var frozen = filter.ToImmutable();
            Assert.IsTrue(await frozen.AllAsync(values));
            foreach (var value in values)
            {
                Assert.IsTrue(filter.Contains(value));
                var positions = filter.ComputeHash(value);
                Assert.AreEqual(filter.Statistics.K, positions.Length);
                Assert.IsTrue(positions.All(p => p >= 0 && p < filter.Statistics.M));
                CollectionAssert.AreEqual(positions, frozen.ComputeHash(value));
            }
            var function = HashFunction.GetHashFunction(method);
            Assert.AreEqual(1000, function.ComputeHash(Array.Empty<byte>(), 1, 1000).Length);
            Assert.IsTrue(function.ComputeHash(new byte[] { 1 }, int.MaxValue, 200).All(p => p >= 0 && p < int.MaxValue));
            Assert.ThrowsExactly<ArgumentNullException>(() => function.ComputeHash(null, 1, 1));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => function.ComputeHash(Array.Empty<byte>(), 0, 1));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => function.ComputeHash(Array.Empty<byte>(), 1, 0));
        }

        [TestMethod]
        public void SizingRejectsInvalidAndUnrepresentableRequests()
        {
            foreach (double p in new[] { double.NaN, double.NegativeInfinity, double.PositiveInfinity, 0, -1, 1 })
                Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Statistics(10L, p));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new Statistics(long.MaxValue, 0.01));
            Assert.AreEqual((short)1, new Statistics(1, long.MaxValue).K);
            Assert.AreEqual((short)short.MaxValue, new Statistics(int.MaxValue, 1L).K);
            double probability = new Statistics(100, long.MaxValue).P;
            Assert.IsTrue(double.IsFinite(probability) && probability >= 0 && probability <= 1);
            foreach (long n in new long[] { 1, 2, 100, 10000, 7000000 })
            foreach (double p in new[] { 0.9, 0.1, 0.01, 0.000001 })
            {
                var s = new Statistics(n, p);
                Assert.IsTrue(s.M > 0 && s.K > 0);
                Assert.IsTrue(s.P <= p + 1e-14, $"n={n} p={p} calculated={s.P}");
            }
        }

        [TestMethod]
        public void SnapshotsOwnAllExportedAndSuppliedBits()
        {
            var filter = FilterBuilder.Build(100);
            filter.Add("present");
            var snapshot = filter.ToImmutable();
            filter.HashBits.SetAll(false);
            snapshot.HashBits.SetAll(false);
            Assert.IsTrue(filter.Contains("present"));
            Assert.IsTrue(snapshot.Contains("present"));
            var source = snapshot.HashBits;
            var imported = new ReadOnlyFilter(source, snapshot.Hash.ToHashMethod(), snapshot.Statistics.M,
                snapshot.Statistics.N, snapshot.Statistics.K, snapshot.Statistics.P);
            source.SetAll(false);
            Assert.IsTrue(imported.Contains("present"));
            var before = snapshot.HashBits.Cast<bool>().ToArray();
            filter.Add("later");
            CollectionAssert.AreEqual(before, snapshot.HashBits.Cast<bool>().ToArray());
            Assert.ThrowsExactly<ArgumentException>(() => new ReadOnlyFilter(new BitArray(1),
                HashMethod.SHA256, 2, 1, 1, 0.1));
            Assert.ThrowsExactly<NotSupportedException>(() => new ReadOnlyFilter(new BitArray(1),
                HashMethod.SHA256, 1, 1, 1, 0.1, 1));
        }

        [TestMethod]
        public async Task CollectionsAndBoxedScalarsUseTheSameEncoding()
        {
            var filter = FilterBuilder.Build(100);
            var array = new[] { 12, 34, 56 };
            var list = array.ToList();
            filter.Add(array);
            Assert.AreEqual(3, filter.Contains(array).Count);
            Assert.IsTrue(filter.All(list));
            Assert.IsTrue(filter.Contains((object)12));
            CollectionAssert.AreEqual(filter.ComputeHash(12), filter.ComputeHash((object)12));
            Assert.IsTrue((await filter.ContainsAsync(list)).All(x => x));
            filter.Add<int>((IEnumerable<int>)list);
            Assert.IsTrue(filter.ToImmutable().Contains(list).All(x => x));
            Assert.IsTrue((await filter.ToImmutable().ContainsAsync(array)).All(x => x));
            var prior = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
                filter.Add((object)12.5m);
                var hashes = filter.ComputeHash((object)12.5m);
                CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
                Assert.IsTrue(filter.Contains((object)12.5m));
                CollectionAssert.AreEqual(hashes, filter.ComputeHash((object)12.5m));
            }
            finally { CultureInfo.CurrentCulture = prior; }
            Assert.ThrowsExactly<ArgumentNullException>(() => filter.Add((byte[])null));
            Assert.ThrowsExactly<ArgumentNullException>(() => filter.Contains((byte[])null));
            Assert.ThrowsExactly<ArgumentNullException>(() => filter.Add((IEnumerable<byte[]>)null));
            Assert.ThrowsExactly<ArgumentNullException>(() => filter.Add(new byte[][] { null }));
            Assert.AreEqual(0, filter.Contains(Array.Empty<byte[]>()).Count);
            Assert.IsTrue(filter.All(Array.Empty<byte[]>()));
        }

        [TestMethod]
        public void ParallelWritersAndSnapshotsDoNotLoseCompletedInsertions()
        {
            var filter = FilterBuilder.Build(2000);
            Parallel.For(0, 1000, i =>
            {
                filter.Add(i);
                Assert.IsTrue(filter.ToImmutable().Contains(i));
            });
            Assert.IsTrue(filter.All(Enumerable.Range(0, 1000)));
        }

        [TestMethod]
        [DataRow(HashMethod.LCGWithFNV1)]
        [DataRow(HashMethod.LCGWithFNV1a)]
        [DataRow(HashMethod.LCGModifiedFNV1)]
        [DataRow(HashMethod.RNGWithFNV1)]
        [DataRow(HashMethod.RNGWithFNV1a)]
        [DataRow(HashMethod.RNGModifiedFNV1)]
        [DataRow(HashMethod.CRC32)]
        [DataRow(HashMethod.CRC32u)]
        [DataRow(HashMethod.Adler32)]
        [DataRow(HashMethod.Murmur2)]
        [DataRow(HashMethod.Murmur3)]
        [DataRow(HashMethod.Murmur3KirschMitzenmacher)]
        [DataRow(HashMethod.SHA1)]
        [DataRow(HashMethod.SHA256)]
        [DataRow(HashMethod.SHA384)]
        [DataRow(HashMethod.SHA512)]
        public void FalsePositiveRateMatchesDesignAtCapacity(HashMethod method)
        {
            const int n = 10000, queries = 50000;
            const double requested = 0.01;
            var filter = FilterBuilder.Build(n, requested, method);
            filter.Add(Enumerable.Range(0, n).Select(i => Encoding.UTF8.GetBytes("member:" + i)));
            Assert.IsTrue(filter.All(Enumerable.Range(0, n).Select(i => Encoding.UTF8.GetBytes("member:" + i))));
            int positives = Enumerable.Range(0, queries).Count(i => filter.Contains(Encoding.UTF8.GetBytes("absent:" + i)));
            double upper = requested + 6 * Math.Sqrt(requested * (1 - requested) / queries);
            Assert.IsTrue((double)positives / queries <= upper, $"False positives {positives}/{queries}, upper={upper}");
        }
    }
}
