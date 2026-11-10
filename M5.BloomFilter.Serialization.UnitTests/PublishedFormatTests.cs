using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using ProtoBuf;
using STJ = System.Text.Json.JsonSerializer;

namespace M5.BloomFilter.Serialization.UnitTests
{
    /// <summary>
    /// Snapshots written by the published 2.0.1 packages (tools/V201Oracle) must
    /// load unchanged, and the current code must write what 2.0.1 can read.
    /// </summary>
    [TestClass]
    public class PublishedFormatTests
    {
        private static readonly (string Name, int Expected, int Keys, int Bits)[] Snapshots =
        {
            ("large", 1000, 500, 9593), // above the companion compression threshold; 9593 % 32 != 0
            ("small", 10, 5, 96),       // below it, so every format is byte-for-byte deterministic
        };

        private static string Read(string name) => File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Fixtures", "v2.0.1", name)).Trim();

        private static System.Text.Json.JsonSerializerOptions Options()
        {
            var options = new System.Text.Json.JsonSerializerOptions();
            options.Converters.Add(new System.Text.Json.Serialization.BloomFilterConverter());
            return options;
        }

        private static string Key(int index) => "key:" + index.ToString(CultureInfo.InvariantCulture);

        private static IImmutableBloomFilter Rebuild(int expected, int keys)
        {
            var filter = FilterBuilder.Build(expected, 0.01);
            for (int i = 0; i < keys; i++) filter.Add(Key(i));
            return filter.ToImmutable();
        }

        private static string Bits(BitArray bits)
        {
            var bytes = new byte[(bits.Length + 7) / 8];
            bits.CopyTo(bytes, 0);
            return bits.Length.ToString(CultureInfo.InvariantCulture) + ":" + Convert.ToHexString(bytes);
        }

        private static byte[] Protobuf(IImmutableBloomFilter snapshot)
        {
            using var stream = new MemoryStream();
            Serializer.Serialize(stream, new BloomFilter(snapshot));
            return stream.ToArray();
        }

        [TestMethod]
        public void PublishedSnapshotsLoadWithEveryKeyAndIdenticalBits()
        {
            foreach (var (name, _, keys, bitCount) in Snapshots)
            {
                string expectedBits = Read(name + ".bits.txt");
                var loaded = new (string Format, IImmutableBloomFilter Filter)[]
                {
                    ("core", JsonConvert.DeserializeObject<IImmutableBloomFilter>(Read(name + ".core.json"), new BloomFilterJsonConverter())),
                    ("newtonsoft", JsonConvert.DeserializeObject<IImmutableBloomFilter>(Read(name + ".newtonsoft.json"), new Newtonsoft.Json.BloomFilterConverter())),
                    ("stj", STJ.Deserialize<IImmutableBloomFilter>(Read(name + ".stj.json"), Options())),
                    ("protobuf", Serializer.Deserialize<BloomFilter>(new MemoryStream(Convert.FromBase64String(Read(name + ".protobuf.b64")))).AsBloomFilter()),
                };

                foreach (var (format, filter) in loaded)
                {
                    string label = name + "." + format;
                    Assert.AreEqual(bitCount, filter.Statistics.M, label);
                    Assert.AreEqual(7, filter.Statistics.K, label);
                    Assert.AreEqual(HashMethod.Murmur3KirschMitzenmacher, filter.Hash.ToHashMethod(), label);
                    Assert.AreEqual(expectedBits, Bits(filter.HashBits), label);
                    for (int i = 0; i < keys; i++)
                    {
                        Assert.IsTrue(filter.Contains(Key(i)), label + " " + Key(i));
                        Assert.IsTrue(filter.ContainsKey(Key(i).AsSpan()), label + " span " + Key(i));
                    }
                }
            }
        }

        [TestMethod]
        public void RebuiltFiltersHaveThePublishedBitsAndCoreJson()
        {
            foreach (var (name, expected, keys, _) in Snapshots)
            {
                var snapshot = Rebuild(expected, keys);
                Assert.AreEqual(Read(name + ".bits.txt"), Bits(snapshot.HashBits), name);
                Assert.AreEqual(Read(name + ".core.json"), JsonConvert.SerializeObject(snapshot, new BloomFilterJsonConverter()), name);
            }
        }

        [TestMethod]
        public void SmallSnapshotIsByteIdenticalInEveryFormat()
        {
            var snapshot = Rebuild(10, 5);
            Assert.AreEqual(Read("small.newtonsoft.json"), JsonConvert.SerializeObject(snapshot, new Newtonsoft.Json.BloomFilterConverter()));
            Assert.AreEqual(Read("small.stj.json"), STJ.Serialize(snapshot, Options()));
            Assert.AreEqual(Read("small.protobuf.b64"), Convert.ToBase64String(Protobuf(snapshot)));
        }

        /// <summary>
        /// Set M5_ORACLE_OUT to a directory to write current snapshots there, then
        /// run <c>tools/V201Oracle -- --read &lt;directory&gt;</c> so the published
        /// 2.0.1 code reads them. Without the variable this only checks round trips.
        /// </summary>
        [TestMethod]
        public void CurrentSnapshotsRoundTripAndCanBeExportedForTheOracle()
        {
            string output = Environment.GetEnvironmentVariable("M5_ORACLE_OUT");
            if (!string.IsNullOrEmpty(output)) Directory.CreateDirectory(output);
            foreach (var (name, expected, keys, _) in Snapshots)
            {
                var snapshot = Rebuild(expected, keys);
                string core = JsonConvert.SerializeObject(snapshot, new BloomFilterJsonConverter());
                string newtonsoft = JsonConvert.SerializeObject(snapshot, new Newtonsoft.Json.BloomFilterConverter());
                string stj = STJ.Serialize(snapshot, Options());
                string protobuf = Convert.ToBase64String(Protobuf(snapshot));

                string bits = Bits(snapshot.HashBits);
                Assert.AreEqual(bits, Bits(JsonConvert.DeserializeObject<IImmutableBloomFilter>(core, new BloomFilterJsonConverter()).HashBits));
                Assert.AreEqual(bits, Bits(JsonConvert.DeserializeObject<IImmutableBloomFilter>(newtonsoft, new Newtonsoft.Json.BloomFilterConverter()).HashBits));
                Assert.AreEqual(bits, Bits(STJ.Deserialize<IImmutableBloomFilter>(stj, Options()).HashBits));
                Assert.AreEqual(bits, Bits(Serializer.Deserialize<BloomFilter>(new MemoryStream(Convert.FromBase64String(protobuf))).AsBloomFilter().HashBits));

                if (string.IsNullOrEmpty(output)) continue;
                File.WriteAllText(Path.Combine(output, name + ".core.json"), core);
                File.WriteAllText(Path.Combine(output, name + ".newtonsoft.json"), newtonsoft);
                File.WriteAllText(Path.Combine(output, name + ".stj.json"), stj);
                File.WriteAllText(Path.Combine(output, name + ".protobuf.b64"), protobuf);
            }
        }
    }
}
