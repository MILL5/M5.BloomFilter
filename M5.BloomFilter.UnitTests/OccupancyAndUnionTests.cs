using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace M5.BloomFilter.UnitTests
{
    [TestClass]
    public class OccupancyAndUnionTests
    {
        private static IEnumerable<HashMethod> Methods => Enum.GetValues<HashMethod>().Where(method => method != HashMethod.Unknown);

        private sealed class ForeignFilter : IBloomFilter
        {
            private readonly BitArray _bits;
            public ForeignFilter(BitArray bits, HashFunction hash, short k) { _bits = bits; Hash = hash; Statistics = new Statistics(bits.Length, k); }
            public BitArray HashBits => new BitArray(_bits);
            public HashFunction Hash { get; }
            public Statistics Statistics { get; }
            public void Add(byte[] element) { foreach (int p in ComputeHash(element)) _bits[p] = true; }
            public Task AddAsync(byte[] element) { Add(element); return Task.CompletedTask; }
            public void Add(IEnumerable<byte[]> elements) { foreach (var e in elements) Add(e); }
            public Task AddAsync(IEnumerable<byte[]> elements) { Add(elements); return Task.CompletedTask; }
            public bool Contains(byte[] element) => ComputeHash(element).All(p => _bits[p]);
            public Task<bool> ContainsAsync(byte[] element) => Task.FromResult(Contains(element));
            public ICollection<bool> Contains(IEnumerable<byte[]> elements) => elements.Select(Contains).ToList();
            public Task<ICollection<bool>> ContainsAsync(IEnumerable<byte[]> elements) => Task.FromResult(Contains(elements));
            public bool All(IEnumerable<byte[]> elements) => elements.All(Contains);
            public Task<bool> AllAsync(IEnumerable<byte[]> elements) => Task.FromResult(All(elements));
            public int[] ComputeHash(byte[] data) => Hash.ComputeHash(data, Statistics.M, Statistics.K);
        }

        [TestMethod]
        public void OccupancyOfEmptyPartialAndSaturatedFilters()
        {
            var filter = FilterBuilder.Build(1000, 0.01);
            var empty = filter.GetOccupancy();
            Assert.AreEqual(0, empty.SetBits);
            Assert.AreEqual(filter.Statistics.M, empty.TotalBits);
            Assert.AreEqual(filter.Statistics.K, empty.HashCount);
            Assert.AreEqual(0d, empty.FillRatio);
            Assert.AreEqual(0d, empty.EstimatedFalsePositiveRate);
            Assert.AreEqual(0d, empty.EstimatedCount);

            // Hand-computed: 96 of 128 bits with k = 2.
            var bits = new BitArray(128);
            for (int i = 0; i < 96; i++) bits[i] = true;
            var partial = new ReadOnlyFilter(bits, HashMethod.SHA256, 128, 40, 2, 0.5).GetOccupancy();
            Assert.AreEqual(96, partial.SetBits);
            Assert.AreEqual(0.75, partial.FillRatio, 1e-12);
            Assert.AreEqual(0.5625, partial.EstimatedFalsePositiveRate, 1e-12);
            Assert.AreEqual(-64 * Math.Log(0.25), partial.EstimatedCount, 1e-9);

            var saturated = new ReadOnlyFilter(new BitArray(77, true), HashMethod.SHA256, 77, 10, 3, 0.5).GetOccupancy();
            Assert.AreEqual(77, saturated.SetBits);
            Assert.AreEqual(1d, saturated.FillRatio);
            Assert.AreEqual(1d, saturated.EstimatedFalsePositiveRate);
            Assert.IsTrue(double.IsPositiveInfinity(saturated.EstimatedCount));

            Assert.AreEqual(default, new BloomFilterOccupancy());
            Assert.AreEqual(0d, default(BloomFilterOccupancy).EstimatedCount);
            Assert.AreEqual(partial, new BloomFilterOccupancy(96, 128, 2));
            Assert.IsTrue(partial != empty);
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new BloomFilterOccupancy(5, 4, 1));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new BloomFilterOccupancy(-1, 4, 1));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new BloomFilterOccupancy(0, 0, 1));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new BloomFilterOccupancy(0, 4, 0));
            Assert.ThrowsExactly<ArgumentNullException>(() => ((IImmutableBloomFilter)null).GetOccupancy());
        }

        [TestMethod]
        public void OccupancyTracksWhatWasAdded()
        {
            foreach (var method in Methods)
            {
                var filter = FilterBuilder.Build(2000, 0.01, method);
                for (int i = 0; i < 1000; i++) filter.Add("key:" + i);
                var occupancy = filter.GetOccupancy();
                Assert.AreEqual(filter.HashBits.Cast<bool>().Count(bit => bit), occupancy.SetBits, method.ToString());
                Assert.AreEqual(occupancy, filter.ToImmutable().GetOccupancy(), method.ToString());
                Assert.AreEqual(occupancy, new ForeignFilter(filter.HashBits, filter.Hash, filter.Statistics.K).GetOccupancy(), method.ToString());
                // Half full by design, so well under the 1% target and close to 1000 keys.
                Assert.IsTrue(occupancy.EstimatedFalsePositiveRate < filter.Statistics.P, method.ToString());
                Assert.AreEqual(1000d, occupancy.EstimatedCount, 60d, method.ToString());
            }
        }

        [TestMethod]
        public void UnionHoldsEveryKeyOfBothFiltersForEveryStrategy()
        {
            foreach (var method in Methods)
            {
                var left = FilterBuilder.Build(500, 0.01, method);
                var right = FilterBuilder.Build(500, 0.01, method);
                var leftKeys = Enumerable.Range(0, 200).Select(i => "left:" + i).ToArray();
                var rightKeys = Enumerable.Range(0, 200).Select(i => "right:" + i).ToArray();
                left.Add(leftKeys);
                right.Add(rightKeys);
                Assert.IsTrue(left.IsCompatibleWith(right));

                var expected = left.HashBits.Or(right.HashBits).Cast<bool>().ToArray();

                var merged = left.ToImmutable().Union(right);
                CollectionAssert.AreEqual(expected, merged.HashBits.Cast<bool>().ToArray(), method.ToString());
                Assert.IsTrue(merged.All(leftKeys) && merged.All(rightKeys), method.ToString());
                Assert.AreEqual(left.Statistics.N, merged.Statistics.N);
                Assert.AreEqual(method, merged.Hash.ToHashMethod());

                var beforeRight = right.HashBits.Cast<bool>().ToArray();
                left.UnionWith(right.ToImmutable());
                CollectionAssert.AreEqual(expected, left.HashBits.Cast<bool>().ToArray(), method.ToString());
                CollectionAssert.AreEqual(beforeRight, right.HashBits.Cast<bool>().ToArray(), "source must not change");
                Assert.IsTrue(left.All(leftKeys) && left.All(rightKeys), method.ToString());

                left.UnionWith(left);
                CollectionAssert.AreEqual(expected, left.HashBits.Cast<bool>().ToArray(), "self-union");
                left.UnionWith(right);
                CollectionAssert.AreEqual(expected, left.HashBits.Cast<bool>().ToArray(), "repeat union");
            }
        }

        [TestMethod]
        public void UnionRejectsIncompatibleAndNullArguments()
        {
            var a = FilterBuilder.Build(500, 0.01);
            var differentSize = FilterBuilder.Build(501, 0.01);
            var differentHashCount = new BloomFilter(a.Statistics.M, (short)(a.Statistics.K + 1), a.Hash);
            var differentStrategy = FilterBuilder.Build(500, 0.01, HashMethod.Murmur3);
            Assert.AreEqual(a.Statistics.M, differentStrategy.Statistics.M);

            foreach (IImmutableBloomFilter other in new IImmutableBloomFilter[] { differentSize, differentHashCount, differentStrategy })
            {
                Assert.IsFalse(a.IsCompatibleWith(other));
                Assert.ThrowsExactly<ArgumentException>(() => a.UnionWith(other));
                Assert.ThrowsExactly<ArgumentException>(() => a.Union(other));
                Assert.ThrowsExactly<ArgumentException>(() => a.ToImmutable().Union(other));
            }

            Assert.AreEqual(0, a.GetOccupancy().SetBits);
            Assert.ThrowsExactly<ArgumentNullException>(() => a.UnionWith(null));
            Assert.ThrowsExactly<ArgumentNullException>(() => a.Union(null));
            Assert.ThrowsExactly<ArgumentNullException>(() => a.IsCompatibleWith(null));
            Assert.ThrowsExactly<ArgumentNullException>(() => ((IBloomFilter)null).UnionWith(a));
            Assert.ThrowsExactly<ArgumentNullException>(() => ((IImmutableBloomFilter)null).Union(a));
            Assert.ThrowsExactly<ArgumentNullException>(() => ((IImmutableBloomFilter)null).IsCompatibleWith(a));
        }

        private sealed class CustomHash : HashFunction
        {
            public override int[] ComputeHash(byte[] data, int m, int k)
                => Enumerable.Range(0, k).Select(i => (data.Length * 31 + i * 7) % m).ToArray();
        }

        [TestMethod]
        public void UnknownStrategiesCombineOnlyWhenTheyShareOneInstance()
        {
            var shared = new CustomHash();
            var a = new BloomFilter(256, (short)3, shared);
            var b = new BloomFilter(256, (short)3, shared);
            var c = new BloomFilter(256, (short)3, new CustomHash());
            a.Add(new byte[] { 1 });
            b.Add(new byte[] { 1, 2 });
            Assert.AreEqual(HashMethod.Unknown, a.Hash.ToHashMethod());
            Assert.IsTrue(a.IsCompatibleWith(b));
            Assert.IsFalse(a.IsCompatibleWith(c));
            Assert.ThrowsExactly<ArgumentException>(() => a.UnionWith(c));

            var merged = a.ToImmutable().Union(b);
            Assert.IsTrue(merged.Contains(new byte[] { 1 }) && merged.Contains(new byte[] { 1, 2 }));
            Assert.AreSame(shared, merged.Hash);
            a.UnionWith(b);
            Assert.IsTrue(a.Contains(new byte[] { 1, 2 }));
        }

        [TestMethod]
        public void UnionWorksAcrossImplementationsButOnlyIntoABuiltInTarget()
        {
            var builtIn = FilterBuilder.Build(300, 0.01, HashMethod.SHA256);
            builtIn.Add("built-in");
            var foreign = new ForeignFilter(new BitArray(builtIn.Statistics.M), builtIn.Hash, builtIn.Statistics.K);
            foreign.Add(System.Text.Encoding.UTF8.GetBytes("foreign"));

            Assert.ThrowsExactly<NotSupportedException>(() => foreign.UnionWith(builtIn));

            var merged = ((IImmutableBloomFilter)foreign).Union(builtIn);
            Assert.IsTrue(merged.Contains("built-in") && merged.Contains("foreign"));
            builtIn.UnionWith(foreign);
            Assert.IsTrue(builtIn.Contains("built-in") && builtIn.Contains("foreign"));
        }

        [TestMethod]
        public void ShardsFilledInParallelUnionIntoOneFilter()
        {
            const int shards = 8, perShard = 2000;
            var parts = Enumerable.Range(0, shards).Select(_ => FilterBuilder.Build(shards * perShard, 0.01)).ToArray();
            Parallel.For(0, shards, shard =>
            {
                for (int i = 0; i < perShard; i++) parts[shard].Add($"{shard}:{i}");
            });

            var whole = FilterBuilder.Build(shards * perShard, 0.01);
            Parallel.ForEach(parts, part => whole.UnionWith(part));

            var single = FilterBuilder.Build(shards * perShard, 0.01);
            for (int shard = 0; shard < shards; shard++)
                for (int i = 0; i < perShard; i++) single.Add($"{shard}:{i}");

            CollectionAssert.AreEqual(single.HashBits.Cast<bool>().ToArray(), whole.HashBits.Cast<bool>().ToArray());
            Assert.AreEqual(shards * perShard, whole.GetOccupancy().EstimatedCount, shards * perShard * 0.05);
        }

        [TestMethod]
        public void UnionWithASourceThatIsBeingWrittenKeepsEveryCompletedInsertion()
        {
            const int count = 6000;
            var source = FilterBuilder.Build(20000, 0.01);
            int completed = -1;
            var writer = Task.Run(() =>
            {
                for (int i = 0; i < count; i++)
                {
                    source.Add(i);
                    System.Threading.Volatile.Write(ref completed, i);
                    if ((i & 31) == 0) System.Threading.Thread.Yield();
                }
            });

            int unions = 0;
            while (!writer.IsCompleted)
            {
                // Everything finished before this union began must be in its result,
                // whether the copy is taken into a mutable filter or a snapshot.
                int before = System.Threading.Volatile.Read(ref completed);
                var target = FilterBuilder.Build(20000, 0.01);
                target.UnionWith(source);
                var merged = target.ToImmutable().Union(source);
                for (int key = 0; key <= before; key++)
                {
                    Assert.IsTrue(target.Contains(key), $"union {unions} lost {key} of {before}");
                    Assert.IsTrue(merged.Contains(key), $"snapshot union {unions} lost {key} of {before}");
                }

                unions++;
            }

            writer.Wait();
            Assert.IsTrue(unions > 0);
            var last = FilterBuilder.Build(20000, 0.01);
            last.UnionWith(source);
            CollectionAssert.AreEqual(source.HashBits.Cast<bool>().ToArray(), last.HashBits.Cast<bool>().ToArray());
        }
    }
}
