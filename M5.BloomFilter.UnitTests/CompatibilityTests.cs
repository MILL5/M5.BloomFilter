using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace M5.BloomFilter.UnitTests
{
    /// <summary>
    /// Pins 2.1 behaviour to fixtures written by the published 2.0.1 packages
    /// (tools/V201Oracle), so the comparison never depends on the code under test.
    /// </summary>
    [TestClass]
    public class CompatibilityTests
    {
        private static string Fixture(string name) => Path.Combine(AppContext.BaseDirectory, "Fixtures", "v2.0.1", name);

        private static int[] Parse(string positions) => positions.Split(',').Select(int.Parse).ToArray();

        // Must match Oracle.Input in tools/V201Oracle/Program.cs.
        private static byte[] Input(int length)
        {
            var data = new byte[length];
            for (int i = 0; i < length; i++) data[i] = (byte)((i * 131 + length * 17 + 7) & 0xFF);
            return data;
        }

        // Must match Oracle.Strings in tools/V201Oracle/Program.cs.
        private static readonly string[] Strings =
        {
            "", "a", "customer:42", "Bloom v2 ☃", "東京", "\U0001F600", "\uD800", "a\uDC00b",
            new string('x', 300), string.Concat(Enumerable.Repeat("é\U0001F600", 90)),
        };

        private static IBloomFilter NewFilter() => FilterBuilder.Build(1000, 0.01);

        private static void AssertExactly(int[] expected, IImmutableBloomFilter filter, string label)
        {
            var bits = filter.HashBits;
            var distinct = expected.Distinct().ToArray();
            foreach (int position in distinct) Assert.IsTrue(bits[position], label);
            Assert.AreEqual(distinct.Length, bits.Cast<bool>().Count(bit => bit), label);
        }

        [TestMethod]
        public void EveryStrategyReproducesPublishedPositions()
        {
            var rows = File.ReadAllLines(Fixture("positions.txt"));
            Assert.AreEqual(960, rows.Length);
            int spanRows = 0;
            Span<int> buffer = stackalloc int[8];
            foreach (var row in rows)
            {
                var fields = row.Split(';');
                var function = HashFunction.Functions[(HashMethod)int.Parse(fields[0])];
                var data = Input(int.Parse(fields[1]));
                int m = int.Parse(fields[2]);
                var expected = Parse(fields[3]);
                CollectionAssert.AreEqual(expected, function.ComputeHash(data, m, 8), row);
                if (function.TryComputeHash(data, m, buffer))
                {
                    spanRows++;
                    CollectionAssert.AreEqual(expected, buffer.ToArray(), "span " + row);
                }
            }

            // Murmur3 and Murmur3KirschMitzenmacher: 20 lengths x 3 sizes each.
            Assert.AreEqual(120, spanRows);
        }

        [TestMethod]
        public void OnlyTheTwoMurmur3StrategiesHaveASpanCore()
        {
            Span<int> one = stackalloc int[1];
            foreach (var pair in HashFunction.Functions)
            {
                bool expected = pair.Key is HashMethod.Murmur3 or HashMethod.Murmur3KirschMitzenmacher;
                Assert.AreEqual(expected, pair.Value.TryComputeHash(ReadOnlySpan<byte>.Empty, 1, one), pair.Key.ToString());
            }
        }

        [TestMethod]
        public void StringOverloadsSetThePublishedBits()
        {
            var rows = File.ReadAllLines(Fixture("keys.txt")).Where(row => row.StartsWith("string;")).ToArray();
            Assert.AreEqual(Strings.Length, rows.Length);
            foreach (var row in rows)
            {
                var fields = row.Split(';');
                string key = Strings[int.Parse(fields[1])];
                var expected = Parse(fields[2]);
                byte[] utf8 = Encoding.UTF8.GetBytes(key);

                CollectionAssert.AreEqual(expected, NewFilter().ComputeHash(key), row);

                var viaString = NewFilter(); viaString.Add(key); AssertExactly(expected, viaString, "Add(string) " + row);
                var viaBytes = NewFilter(); viaBytes.Add(utf8); AssertExactly(expected, viaBytes, "Add(byte[]) " + row);
                var viaKeyString = NewFilter(); viaKeyString.AddKey(key); AssertExactly(expected, viaKeyString, "AddKey(string) " + row);
                var viaKeyChars = NewFilter(); viaKeyChars.AddKey(key.AsSpan()); AssertExactly(expected, viaKeyChars, "AddKey(chars) " + row);
                var viaKeyBytes = NewFilter(); viaKeyBytes.AddKey(utf8.AsSpan()); AssertExactly(expected, viaKeyBytes, "AddKey(bytes) " + row);

                AssertLookups(expected, filter => filter.Contains(key), row);
                AssertLookups(expected, filter => filter.ContainsKey(key), row);
                AssertLookups(expected, filter => filter.ContainsKey(key.AsSpan()), row);
                AssertLookups(expected, filter => filter.ContainsKey(utf8.AsSpan()), row);
                AssertLookups(expected, filter => filter.Contains(utf8), row);
            }
        }

        [TestMethod]
        public void ScalarOverloadsSetThePublishedBits()
        {
            var rows = File.ReadAllLines(Fixture("keys.txt")).Where(row => !row.StartsWith("string;")).ToArray();
            Assert.AreEqual(13, rows.Length);
            foreach (var row in rows)
            {
                var fields = row.Split(';');
                var expected = Parse(fields[2]);
                var filter = NewFilter();
                Func<IImmutableBloomFilter, bool> lookup;
                Func<IBloomFilter, bool> mutableLookup;
                object boxed;
                switch (fields[0] + ";" + fields[1])
                {
                    case "byte;7": { byte v = 7; filter.Add(v); lookup = f => f.Contains(v); mutableLookup = f => f.Contains(v); boxed = v; break; }
                    case "short;-2": { short v = -2; filter.Add(v); lookup = f => f.Contains(v); mutableLookup = f => f.Contains(v); boxed = v; break; }
                    case "ushort;65535": { ushort v = 65535; filter.Add(v); lookup = f => f.Contains(v); mutableLookup = f => f.Contains(v); boxed = v; break; }
                    case "int;42": { int v = 42; filter.Add(v); lookup = f => f.Contains(v); mutableLookup = f => f.Contains(v); boxed = v; break; }
                    case "int;-1": { int v = -1; filter.Add(v); lookup = f => f.Contains(v); mutableLookup = f => f.Contains(v); boxed = v; break; }
                    case "uint;4294967295": { uint v = uint.MaxValue; filter.Add(v); lookup = f => f.Contains(v); mutableLookup = f => f.Contains(v); boxed = v; break; }
                    case "long;-9223372036854775808": { long v = long.MinValue; filter.Add(v); lookup = f => f.Contains(v); mutableLookup = f => f.Contains(v); boxed = v; break; }
                    case "ulong;18446744073709551615": { ulong v = ulong.MaxValue; filter.Add(v); lookup = f => f.Contains(v); mutableLookup = f => f.Contains(v); boxed = v; break; }
                    case "float;1.5": { float v = 1.5f; filter.Add(v); lookup = f => f.Contains(v); mutableLookup = f => f.Contains(v); boxed = v; break; }
                    case "float;NaN": { float v = float.NaN; filter.Add(v); lookup = f => f.Contains(v); mutableLookup = f => f.Contains(v); boxed = v; break; }
                    case "double;-0": { double v = -0.0; filter.Add(v); lookup = f => f.Contains(v); mutableLookup = f => f.Contains(v); boxed = v; break; }
                    case "double;epsilon": { double v = double.Epsilon; filter.Add(v); lookup = f => f.Contains(v); mutableLookup = f => f.Contains(v); boxed = v; break; }
                    case "datetime;638447616000000000": { var v = new DateTime(638447616000000000L); filter.Add(v); lookup = f => f.Contains(v); mutableLookup = f => f.Contains(v); boxed = v; break; }
                    default: throw new InvalidOperationException("Unknown fixture row " + row);
                }

                AssertExactly(expected, filter, row);
                Assert.IsTrue(mutableLookup(filter), row);
                AssertLookups(expected, lookup, row);

                // The boxed overload keeps using the shared encoder and must agree.
                var viaObject = NewFilter();
                viaObject.Add(boxed);
                AssertExactly(expected, viaObject, "object " + row);
            }
        }

        [TestMethod]
        public void RandomStringsEncodeExactlyAsEncodingUtf8Does()
        {
            // Includes unpaired surrogates, which must become U+FFFD as they do
            // in Encoding.UTF8.GetBytes(string), and strings past the stack buffer.
            var random = new Random(20261110);
            char[] alphabet = { 'a', 'Z', '0', ' ', '\u00E9', '\u6771', '\uFFFD', '\uD83D', '\uDE00', '\uD800', '\uDFFF', '\u0000', '\u007F', '\u0080', '\u07FF', '\u0800' };
            for (int trial = 0; trial < 3000; trial++)
            {
                int length = trial % 7 == 0 ? random.Next(80, 400) : random.Next(0, 24);
                var chars = new char[length];
                for (int i = 0; i < length; i++) chars[i] = alphabet[random.Next(alphabet.Length)];
                string key = new string(chars);
                byte[] reference = Encoding.UTF8.GetBytes(key);

                var viaReference = NewFilter(); viaReference.Add(reference);
                var viaString = NewFilter(); viaString.Add(key);
                var viaChars = NewFilter(); viaChars.AddKey(chars.AsSpan());
                string expected = Bits(viaReference);
                Assert.AreEqual(expected, Bits(viaString), "Add(string) trial " + trial);
                Assert.AreEqual(expected, Bits(viaChars), "AddKey(chars) trial " + trial);
                var snapshot = viaReference.ToImmutable();
                Assert.IsTrue(snapshot.Contains(key) && snapshot.ContainsKey(key) && snapshot.ContainsKey(chars.AsSpan()), "lookup trial " + trial);
            }
        }

        private static string Bits(IImmutableBloomFilter filter)
        {
            var bits = filter.HashBits;
            var bytes = new byte[(bits.Length + 7) / 8];
            bits.CopyTo(bytes, 0);
            return Convert.ToHexString(bytes);
        }

        [TestMethod]
        public void SpanLookupsAgreeWithArrayPositionsForEverySizeAndStrategy()
        {
            var random = new Random(8675309);
            foreach (var method in new[] { HashMethod.Murmur3KirschMitzenmacher, HashMethod.Murmur3, HashMethod.SHA1, HashMethod.CRC32 })
            {
                var hash = HashFunction.Functions[method];
                foreach (int m in new[] { 1, 2, 31, 32, 33, 97, 9593, 1 << 20 })
                {
                    foreach (short k in new short[] { 1, 3, 7, 130 })
                    {
                        var filter = new BloomFilter(m, k, hash);
                        var keys = new List<byte[]>();
                        for (int i = 0; i < 60; i++)
                        {
                            var key = new byte[random.Next(0, 40)];
                            random.NextBytes(key);
                            keys.Add(key);
                            if (i % 3 == 0) filter.Add(key);
                            if (i % 3 == 1) ((IBloomFilter)filter).AddKey(key.AsSpan());
                        }

                        var bits = filter.HashBits;
                        var snapshot = filter.ToImmutable();
                        foreach (var key in keys)
                        {
                            bool expected = hash.ComputeHash(key, m, k).All(position => bits[position]);
                            string label = $"{method} m={m} k={k}";
                            Assert.AreEqual(expected, filter.Contains(key), label);
                            Assert.AreEqual(expected, ((IBloomFilter)filter).ContainsKey(key.AsSpan()), label);
                            Assert.AreEqual(expected, snapshot.Contains(key), label);
                            Assert.AreEqual(expected, snapshot.ContainsKey(key.AsSpan()), label);
                        }
                    }
                }
            }
        }

        // A lookup is true with every expected bit set and false with any one cleared.
        private static void AssertLookups(int[] expected, Func<IImmutableBloomFilter, bool> lookup, string label)
        {
            var template = NewFilter();
            var statistics = template.Statistics;
            var bits = new BitArray(statistics.M);
            foreach (int position in expected) bits[position] = true;
            var full = new ReadOnlyFilter(bits, HashMethod.Murmur3KirschMitzenmacher, statistics.M, statistics.N, statistics.K, statistics.P);
            Assert.IsTrue(lookup(full), label);
            foreach (int cleared in expected.Distinct())
            {
                var partial = new BitArray(bits);
                partial[cleared] = false;
                var filter = new ReadOnlyFilter(partial, HashMethod.Murmur3KirschMitzenmacher, statistics.M, statistics.N, statistics.K, statistics.P);
                Assert.IsFalse(lookup(filter), label + " without bit " + cleared);
            }
        }

        private sealed class MirroredMurmur : Murmur3KirschMitzenmacher
        {
            public override int[] ComputeHash(byte[] data, int m, int k)
                => base.ComputeHash(data, m, k).Select(position => m - 1 - position).ToArray();
        }

        private sealed class MirroredPlainMurmur : Murmur3
        {
            public override int[] ComputeHash(byte[] data, int m, int k)
                => base.ComputeHash(data, m, k).Select(position => m - 1 - position).ToArray();
        }

        private sealed class ConstantFnv : LCGWithFNV1a
        {
            public override uint Hash(byte[] data) => 12345;
        }

        [TestMethod]
        public void SubclassesOfBuiltInStrategiesAreStillCalled()
        {
            Span<int> one = stackalloc int[1];
            foreach (HashFunction custom in new HashFunction[] { new MirroredMurmur(), new MirroredPlainMurmur(), new ConstantFnv() })
            {
                Assert.IsFalse(custom.TryComputeHash(ReadOnlySpan<byte>.Empty, 1, one), custom.GetType().Name);

                var probe = new BloomFilter(1000L, 0.01, custom);
                var expected = custom.ComputeHash(Encoding.UTF8.GetBytes("subclass"), probe.Statistics.M, probe.Statistics.K);

                var viaString = new BloomFilter(1000L, 0.01, custom); ((IBloomFilter)viaString).Add("subclass");
                AssertExactly(expected, viaString, custom.GetType().Name + " Add(string)");
                var viaKey = new BloomFilter(1000L, 0.01, custom); ((IBloomFilter)viaKey).AddKey("subclass".AsSpan());
                AssertExactly(expected, viaKey, custom.GetType().Name + " AddKey(chars)");
                var viaBytes = new BloomFilter(1000L, 0.01, custom); viaBytes.Add(Encoding.UTF8.GetBytes("subclass"));
                AssertExactly(expected, viaBytes, custom.GetType().Name + " Add(byte[])");

                IBloomFilter filter = viaString;
                Assert.IsTrue(filter.Contains("subclass"));
                Assert.IsTrue(filter.ContainsKey("subclass"));
                Assert.IsTrue(filter.ContainsKey(Encoding.UTF8.GetBytes("subclass").AsSpan()));
                var snapshot = filter.ToImmutable();
                Assert.IsTrue(snapshot.Contains("subclass"));
                Assert.IsTrue(snapshot.ContainsKey("subclass".AsSpan()));
                AssertExactly(expected, snapshot, custom.GetType().Name + " snapshot");
            }

            // The constant FNV hash maps every key to the same positions.
            IBloomFilter constant = new BloomFilter(1000L, 0.01, new ConstantFnv());
            constant.Add(1);
            Assert.IsTrue(constant.Contains("anything at all"));
            Assert.IsTrue(constant.Contains(987654321L));
        }

        private sealed class RecordingFilter : IBloomFilter
        {
            public List<byte[]> Added { get; } = new();
            public List<byte[]> Queried { get; } = new();
            public int BatchCalls { get; private set; }
            public BitArray HashBits => new BitArray(64);
            public HashFunction Hash => HashFunction.Functions[HashMethod.SHA256];
            public Statistics Statistics { get; } = new Statistics(64, (short)3);
            public void Add(byte[] element) => Added.Add(element);
            public System.Threading.Tasks.Task AddAsync(byte[] element) { Add(element); return System.Threading.Tasks.Task.CompletedTask; }
            public void Add(IEnumerable<byte[]> elements) => BatchCalls++;
            public System.Threading.Tasks.Task AddAsync(IEnumerable<byte[]> elements) { BatchCalls++; return System.Threading.Tasks.Task.CompletedTask; }
            public bool Contains(byte[] element) { Queried.Add(element); return true; }
            public System.Threading.Tasks.Task<bool> ContainsAsync(byte[] element) => System.Threading.Tasks.Task.FromResult(Contains(element));
            public ICollection<bool> Contains(IEnumerable<byte[]> elements) { BatchCalls++; return Array.Empty<bool>(); }
            public System.Threading.Tasks.Task<ICollection<bool>> ContainsAsync(IEnumerable<byte[]> elements) => System.Threading.Tasks.Task.FromResult(Contains(elements));
            public bool All(IEnumerable<byte[]> elements) { BatchCalls++; return true; }
            public System.Threading.Tasks.Task<bool> AllAsync(IEnumerable<byte[]> elements) => System.Threading.Tasks.Task.FromResult(All(elements));
            public int[] ComputeHash(byte[] data) => new[] { 0, 1, 2 };
        }

        [TestMethod]
        public void OtherImplementationsReceiveOneByteArrayPerKey()
        {
            const string astral = "\U0001F600a";
            var filter = new RecordingFilter();
            IImmutableBloomFilter immutable = filter;

            filter.Add("café");
            filter.Add(42);
            filter.Add(long.MinValue);
            filter.Add(1.5f);
            filter.Add((byte)7);
            filter.AddKey(new byte[] { 1, 2, 3 }.AsSpan());
            filter.AddKey(astral.AsSpan());
            filter.AddKey(astral);
            filter.AddKey(astral.ToCharArray());

            var expectedAdds = new[]
            {
                Encoding.UTF8.GetBytes("café"), BitConverter.GetBytes(42), BitConverter.GetBytes(long.MinValue),
                BitConverter.GetBytes(1.5f), new byte[] { 7 }, new byte[] { 1, 2, 3 },
                Encoding.UTF8.GetBytes(astral), Encoding.UTF8.GetBytes(astral), Encoding.UTF8.GetBytes(astral),
            };
            Assert.IsTrue(BitConverter.IsLittleEndian, "expected values assume a little-endian test host");
            Assert.AreEqual(expectedAdds.Length, filter.Added.Count);
            for (int i = 0; i < expectedAdds.Length; i++) CollectionAssert.AreEqual(expectedAdds[i], filter.Added[i], "add " + i);

            Assert.IsTrue(filter.Contains("café"));
            Assert.IsTrue(immutable.Contains("café"));
            Assert.IsTrue(filter.Contains(42));
            Assert.IsTrue(immutable.Contains(42));
            Assert.IsTrue(immutable.ContainsKey(new byte[] { 1, 2, 3 }.AsSpan()));
            Assert.IsTrue(immutable.ContainsKey(astral.AsSpan()));
            Assert.IsTrue(immutable.ContainsKey(astral));
            var expectedQueries = new[]
            {
                Encoding.UTF8.GetBytes("café"), Encoding.UTF8.GetBytes("café"), BitConverter.GetBytes(42), BitConverter.GetBytes(42),
                new byte[] { 1, 2, 3 }, Encoding.UTF8.GetBytes(astral), Encoding.UTF8.GetBytes(astral),
            };
            Assert.AreEqual(expectedQueries.Length, filter.Queried.Count);
            for (int i = 0; i < expectedQueries.Length; i++) CollectionAssert.AreEqual(expectedQueries[i], filter.Queried[i], "query " + i);

            Assert.AreEqual(0, filter.BatchCalls, "a single key must never reach a batch overload");
        }

        [TestMethod]
        public void NullKeysAreRejectedExactlyAsBefore()
        {
            var filter = NewFilter();
            var snapshot = filter.ToImmutable();
            Assert.ThrowsExactly<ArgumentNullException>(() => filter.Add((string)null));
            Assert.ThrowsExactly<ArgumentNullException>(() => filter.Contains((string)null));
            Assert.ThrowsExactly<ArgumentNullException>(() => snapshot.Contains((string)null));
            Assert.ThrowsExactly<ArgumentNullException>(() => filter.AddKey((string)null));
            Assert.ThrowsExactly<ArgumentNullException>(() => filter.ContainsKey((string)null));
            Assert.ThrowsExactly<ArgumentNullException>(() => snapshot.ContainsKey((string)null));
            Assert.ThrowsExactly<ArgumentNullException>(() => filter.Add((byte[])null));
            Assert.ThrowsExactly<ArgumentNullException>(() => snapshot.Contains((byte[])null));
            Assert.ThrowsExactly<ArgumentNullException>(() => ((IBloomFilter)null).AddKey("x"));
            Assert.ThrowsExactly<ArgumentNullException>(() => ((IImmutableBloomFilter)null).ContainsKey("x"));
            Assert.AreEqual(0, filter.GetOccupancy().SetBits);

            // A null string converted to a span is the empty key, as documented.
            filter.AddKey(default(ReadOnlySpan<char>));
            Assert.IsTrue(filter.Contains(""));
            Assert.IsTrue(filter.Contains(Array.Empty<byte>()));
        }
    }
}
