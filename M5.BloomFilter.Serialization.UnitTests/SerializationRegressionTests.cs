using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProtoBuf;
using STJ = System.Text.Json.JsonSerializer;

namespace M5.BloomFilter.Serialization.UnitTests
{
    [TestClass]
    public class SerializationRegressionTests
    {
        [TestMethod]
        public void PopulatedMembershipSurvivesAllFormatsAndHashMethods()
        {
            var options = new System.Text.Json.JsonSerializerOptions();
            options.Converters.Add(new System.Text.Json.Serialization.BloomFilterConverter());
            foreach (var method in Enum.GetValues<HashMethod>().Where(x => x != HashMethod.Unknown))
            {
                var filter = FilterBuilder.Build(100, 0.01, method);
                var keys = Enumerable.Range(0, 20).Select(BitConverter.GetBytes).ToArray();
                foreach (var key in keys) filter.Add(key);
                IImmutableBloomFilter original = filter.ToImmutable();
                var json = JsonConvert.SerializeObject(original, new Newtonsoft.Json.BloomFilterConverter());
                var nj = JsonConvert.DeserializeObject<ReadOnlyFilter>(json, new Newtonsoft.Json.BloomFilterConverter());
                var ni = JsonConvert.DeserializeObject<IImmutableBloomFilter>(json, new Newtonsoft.Json.BloomFilterConverter());
                var sj = STJ.Deserialize<ReadOnlyFilter>(STJ.Serialize(original, options), options);
                var si = STJ.Deserialize<IImmutableBloomFilter>(STJ.Serialize(original, options), options);
                using var stream = new MemoryStream();
                Serializer.Serialize(stream, new BloomFilter(original));
                stream.Position = 0;
                var pb = Serializer.Deserialize<BloomFilter>(stream).AsBloomFilter();
                var coreJson = JsonConvert.SerializeObject(original, new BloomFilterJsonConverter());
                var core = JsonConvert.DeserializeObject<IImmutableBloomFilter>(coreJson, new BloomFilterJsonConverter());
                foreach (var restored in new IImmutableBloomFilter[] { nj, ni, sj, si, pb, core })
                {
                    foreach (var key in keys) Assert.IsTrue(restored.Contains(key), method.ToString());
                    foreach (var key in Enumerable.Range(20, 100).Select(BitConverter.GetBytes))
                        Assert.AreEqual(original.Contains(key), restored.Contains(key), method.ToString());
                    Assert.AreEqual(original.Statistics.M, restored.Statistics.M);
                    Assert.AreEqual(original.Statistics.N, restored.Statistics.N);
                }
            }
            Assert.AreEqual(1, options.Converters.Count);
        }

        [TestMethod]
        public void JsonRejectsLegacyAndUnsupportedVersionsAndInvalidDimensions()
        {
            var filter = FilterBuilder.Build(10).ToImmutable();
            var converter = new Newtonsoft.Json.BloomFilterConverter();
            var document = JObject.Parse(JsonConvert.SerializeObject(filter, converter));
            var options = new System.Text.Json.JsonSerializerOptions();
            options.Converters.Add(new System.Text.Json.Serialization.BloomFilterConverter());
            foreach (int? version in new int?[] { null, 0, 1, 3 })
            {
                var altered = (JObject)document.DeepClone();
                if (version == null) altered.Remove("v"); else altered["v"] = version;
                Assert.Throws<NotSupportedException>(() => JsonConvert.DeserializeObject<ReadOnlyFilter>(altered.ToString(), converter));
                Assert.Throws<NotSupportedException>(() => STJ.Deserialize<ReadOnlyFilter>(altered.ToString(), options));
                Assert.Throws<NotSupportedException>(() => JsonConvert.DeserializeObject<IImmutableBloomFilter>(altered.ToString(), new BloomFilterJsonConverter()));
            }
            document["m"] = 1;
            Assert.Throws<ArgumentException>(() => JsonConvert.DeserializeObject<ReadOnlyFilter>(document.ToString(), converter));
            Assert.IsNull(JsonConvert.DeserializeObject<ReadOnlyFilter>("null", converter));
            Assert.IsNull(STJ.Deserialize<ReadOnlyFilter>("null", options));
            Assert.IsNull(JsonConvert.DeserializeObject<IImmutableBloomFilter>("null", new BloomFilterJsonConverter()));
        }

        [TestMethod]
        public void ProtobufRejectsLegacyAndMissingBits()
        {
            var dto = new BloomFilter(FilterBuilder.Build(10));
            foreach (var version in new[] { 0, 1, 3 })
            {
                dto.FormatVersion = version;
                using var stream = new MemoryStream();
                Serializer.Serialize(stream, dto);
                stream.Position = 0;
                Assert.Throws<NotSupportedException>(() => Serializer.Deserialize<BloomFilter>(stream).AsBloomFilter());
            }
            dto.FormatVersion = ReadOnlyFilter.CurrentFormatVersion;
            dto.HashBits = null;
            Assert.Throws<FormatException>(() => dto.AsBloomFilter());
        }
    }
}
