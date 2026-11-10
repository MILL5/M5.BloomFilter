using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace M5.BloomFilter.UnitTests
{
    [TestClass]
    public class StorageTests
    {
        private static readonly HashFunction Default = HashFunction.Functions[HashMethod.Murmur3KirschMitzenmacher];

        [TestMethod]
        public void BatchAddWithANullElementLeavesTheFilterUnchanged()
        {
            foreach (var method in new[] { HashMethod.Murmur3KirschMitzenmacher, HashMethod.SHA256 })
            {
                var filter = FilterBuilder.Build(100, 0.01, method);
                Assert.ThrowsExactly<ArgumentNullException>(() => filter.Add(new[] { new byte[] { 1 }, new byte[] { 2 }, null }));
                Assert.AreEqual(0, filter.GetOccupancy().SetBits, method.ToString());
                Assert.IsFalse(filter.HashBits.Cast<bool>().Any(bit => bit), method.ToString());
            }
        }

        [TestMethod]
        public void BatchLookupFinishesEnumeratingBeforeItReadsBits()
        {
            var filter = FilterBuilder.Build(100);
            byte[] key = { 9, 9, 9 };

            IEnumerable<byte[]> YieldThenInsert()
            {
                yield return key;
                filter.Add(key);
            }

            // 2.0 computed every position first, so the key inserted during
            // enumeration is visible when the bits are read. That must not change.
            CollectionAssert.AreEqual(new[] { true }, filter.Contains(YieldThenInsert()).ToArray());
        }

        [TestMethod]
        public void BatchResultsAreOnePerKeyInOrderForEveryHashCount()
        {
            foreach (short k in new short[] { 1, 2, 7, 129 })
            {
                var filter = new BloomFilter(4096, k, Default);
                var present = Enumerable.Range(0, 20).Select(i => BitConverter.GetBytes(i)).ToArray();
                filter.Add(present.Where((_, index) => index % 2 == 0));
                var results = filter.Contains(present).ToArray();
                Assert.AreEqual(20, results.Length);
                for (int i = 0; i < 20; i += 2) Assert.IsTrue(results[i], $"k={k} key {i}");
                var snapshot = filter.ToImmutable();
                CollectionAssert.AreEqual(results, snapshot.Contains(present).ToArray(), $"k={k}");
                Assert.IsTrue(filter.All(present.Where((_, index) => index % 2 == 0)));
                for (int i = 0; i < 20; i++)
                {
                    Assert.AreEqual(results[i], filter.Contains(present[i]), $"k={k} single {i}");
                    Assert.AreEqual(results[i], snapshot.ContainsKey(present[i].AsSpan()), $"k={k} span {i}");
                }
            }
        }

        [TestMethod]
        public void ExportIsBitExactWhenLengthIsNotAMultipleOf32()
        {
            foreach (int m in new[] { 1, 7, 8, 9, 31, 32, 33, 63, 64, 65, 9593 })
            {
                var filter = new BloomFilter(m, (short)3, Default);
                var expected = new BitArray(m);
                for (int i = 0; i < 40; i++)
                {
                    var key = BitConverter.GetBytes(i);
                    filter.Add(key);
                    foreach (int position in filter.ComputeHash(key)) expected[position] = true;
                }

                var exported = filter.HashBits;
                Assert.AreEqual(m, exported.Length);
                CollectionAssert.AreEqual(expected.Cast<bool>().ToArray(), exported.Cast<bool>().ToArray(), "m=" + m);

                var snapshot = (ReadOnlyFilter)filter.ToImmutable();
                CollectionAssert.AreEqual(expected.Cast<bool>().ToArray(), snapshot.HashBits.Cast<bool>().ToArray(), "snapshot m=" + m);
                Assert.AreEqual(expected.Cast<bool>().Count(bit => bit), snapshot.GetOccupancy().SetBits, "m=" + m);

                var statistics = filter.Statistics;
                var imported = new ReadOnlyFilter(exported, HashMethod.Murmur3KirschMitzenmacher, statistics.M, statistics.N, statistics.K, statistics.P);
                CollectionAssert.AreEqual(expected.Cast<bool>().ToArray(), imported.HashBits.Cast<bool>().ToArray(), "import m=" + m);
            }
        }

        [TestMethod]
        public void MaximumCapacityFilterExportsAndReimports()
        {
            // 2,147,483,647 bits needs 67,108,864 words, one more than
            // new BitArray(int[]) accepts on .NET 10.
            const int m = int.MaxValue;
            var filter = new BloomFilter(m, (short)3, Default);
            byte[] key = { 1, 2, 3, 4, 5 };
            filter.Add(key);
            var positions = filter.ComputeHash(key);

            var exported = filter.HashBits;
            Assert.AreEqual(m, exported.Length);
            foreach (int position in positions) Assert.IsTrue(exported[position]);
            Assert.AreEqual(positions.Distinct().Count(), filter.GetOccupancy().SetBits);

            var statistics = filter.Statistics;
            var imported = new ReadOnlyFilter(exported, HashMethod.Murmur3KirschMitzenmacher, statistics.M, statistics.N, statistics.K, statistics.P);
            Assert.IsTrue(imported.Contains(key));
            Assert.AreEqual(positions.Distinct().Count(), imported.GetOccupancy().SetBits);
            Assert.IsFalse(imported.Contains(new byte[] { 5, 4, 3, 2, 1 }));
        }

        [TestMethod]
        public void LookupsStayCorrectWhileAWriterRuns()
        {
            var filter = FilterBuilder.Build(1000);
            filter.Add("before");
            using var release = new ManualResetEventSlim();
            using var started = new ManualResetEventSlim();
            var readers = Task.Run(() =>
            {
                started.Set();
                bool all = true;
                while (!release.IsSet) all &= filter.Contains("before");
                return all;
            });
            Assert.IsTrue(started.Wait(TimeSpan.FromSeconds(10)));
            for (int i = 0; i < 20000; i++) filter.Add(i);
            release.Set();
            Assert.IsTrue(readers.Wait(TimeSpan.FromSeconds(30)));
            Assert.IsTrue(readers.Result, "a completed insertion must never test absent");
        }

        [TestMethod]
        public void ParallelReadersAndWritersNeverLoseACompletedInsertion()
        {
            foreach (var method in new[] { HashMethod.Murmur3KirschMitzenmacher, HashMethod.Murmur3, HashMethod.SHA1 })
            {
                var filter = FilterBuilder.Build(20000, 0.01, method);
                int completed = -1;
                using var stop = new CancellationTokenSource();
                var failures = new System.Collections.Concurrent.ConcurrentQueue<string>();

                var readers = Enumerable.Range(0, 4).Select(reader => Task.Run(() =>
                {
                    var random = new Random(reader);
                    while (!stop.IsCancellationRequested)
                    {
                        int upTo = Volatile.Read(ref completed);
                        if (upTo < 0) continue;
                        int key = random.Next(upTo + 1);
                        if (!filter.Contains(key)) failures.Enqueue($"{method}: lookup missed {key}");
                        if (reader == 0 && !filter.ToImmutable().Contains(key)) failures.Enqueue($"{method}: snapshot missed {key}");
                        if (reader == 1 && !new ReadOnlyFilter(filter).ContainsKey(BitConverter.GetBytes(key).AsSpan())) failures.Enqueue($"{method}: copy missed {key}");
                    }
                })).ToArray();

                Parallel.For(0, 4, writer =>
                {
                    for (int i = writer; i < 8000; i += 4) filter.Add(i);
                });
                Volatile.Write(ref completed, 7999);
                for (int i = 8000; i < 12000; i++)
                {
                    filter.Add(i);
                    Volatile.Write(ref completed, i);
                }

                Thread.Sleep(50);
                stop.Cancel();
                Task.WaitAll(readers);
                Assert.IsTrue(failures.IsEmpty, string.Join("; ", failures.Take(5)));
                Assert.IsTrue(filter.All(Enumerable.Range(0, 12000)), method.ToString());
            }
        }

        [TestMethod]
        public void SnapshotsContainAPrefixOfSequentialInsertions()
        {
            // Sized so that a false positive among the probes is negligible: any
            // gap in a snapshot would be a torn (inconsistent) copy.
            var filter = FilterBuilder.Build(20000, 0.000001);
            const int count = 3000;
            const int maximumSnapshots = 48;
            var snapshots = new List<IImmutableBloomFilter>();
            int done = 0;
            var taker = Task.Run(() =>
            {
                while (Volatile.Read(ref done) == 0 && snapshots.Count < maximumSnapshots)
                {
                    snapshots.Add(filter.ToImmutable());
                    Thread.Yield();
                }
            });
            for (int i = 0; i < count; i++)
            {
                filter.Add("sequence:" + i);
                if ((i & 63) == 0) Thread.Yield();
            }

            Volatile.Write(ref done, 1);
            Assert.IsTrue(taker.Wait(TimeSpan.FromSeconds(60)));
            snapshots.Add(filter.ToImmutable());

            foreach (var snapshot in snapshots)
            {
                int present = 0;
                while (present < count && snapshot.Contains("sequence:" + present)) present++;
                for (int later = present; later < count; later++)
                    Assert.IsFalse(snapshot.Contains("sequence:" + later), $"snapshot holds {later} without {present}");
            }

            Assert.IsTrue(snapshots[^1].All(Enumerable.Range(0, count).Select(i => "sequence:" + i)));
        }

        private sealed class OutOfRangeHash : HashFunction
        {
            public override int[] ComputeHash(byte[] data, int m, int k) => new[] { m };
        }

        private sealed class SecondPositionOutOfRangeHash : HashFunction
        {
            public override int[] ComputeHash(byte[] data, int m, int k) => new[] { 0, m };
        }

        [TestMethod]
        public void BatchLookupRejectsAnInvalidPositionEvenAfterAClearBit()
        {
            // Bit 0 is clear, so a lookup could answer false without reading the
            // second position. A batch must still reject it, as 2.0 did.
            var filter = new BloomFilter(40, (short)2, new SecondPositionOutOfRangeHash());
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => filter.Contains(new[] { new byte[] { 1 } }));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => filter.ToImmutable().Contains(new[] { new byte[] { 1 } }));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => filter.Add(new byte[] { 1 }));
        }

        [TestMethod]
        public void KeysLongerThanTheStackBufferEncodeExactly()
        {
            foreach (int length in new[] { 85, 86, 255, 256, 257, 4096, 70000 })
            {
                string ascii = new string('k', length);
                string mixed = string.Concat(Enumerable.Repeat("\u00E9\U0001F600\uD800z", length / 5 + 1));
                foreach (string key in new[] { ascii, mixed })
                {
                    var expected = FilterBuilder.Build(1000); expected.Add(System.Text.Encoding.UTF8.GetBytes(key));
                    var viaString = FilterBuilder.Build(1000); viaString.Add(key);
                    var viaChars = FilterBuilder.Build(1000); viaChars.AddKey(key.AsSpan());
                    CollectionAssert.AreEqual(expected.HashBits.Cast<bool>().ToArray(), viaString.HashBits.Cast<bool>().ToArray(), "length " + key.Length);
                    CollectionAssert.AreEqual(expected.HashBits.Cast<bool>().ToArray(), viaChars.HashBits.Cast<bool>().ToArray(), "length " + key.Length);
                    Assert.IsTrue(expected.Contains(key) && expected.ToImmutable().ContainsKey(key.AsSpan()));
                }
            }
        }

        [TestMethod]
        public void PositionsOutsideTheFilterAreRejected()
        {
            var filter = new BloomFilter(40, (short)1, new OutOfRangeHash());
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => filter.Add(new byte[] { 1 }));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => filter.Contains(new byte[] { 1 }));
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => filter.Add(new[] { new byte[] { 1 } }));
            Assert.AreEqual(0, filter.GetOccupancy().SetBits);
            Assert.IsFalse(filter.HashBits.Cast<bool>().Any(bit => bit));
        }
    }
}
