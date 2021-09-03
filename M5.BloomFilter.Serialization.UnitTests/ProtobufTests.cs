using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProtoBuf;
using Shouldly;

namespace M5.BloomFilter.Serialization.UnitTests
{
    [TestClass]
    public class ProtobufTests
    {
        static ProtobufTests()
        {
        }

        [TestMethod]
        public void SerializeDeserialize()
        {
            var nonSerializableBloomFilter = FilterBuilder.Build(7000000);

            var serializableBloomFilter = new BloomFilter(nonSerializableBloomFilter);
            BloomFilter deserializedBloomFilter;

            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, serializableBloomFilter);

                ms.Seek(0, SeekOrigin.Begin);

                deserializedBloomFilter = Serializer.Deserialize<BloomFilter>(ms);
            }

            var readOnlyBloomFilter = deserializedBloomFilter.AsBloomFilter();

            readOnlyBloomFilter.ShouldNotBeNull();
            readOnlyBloomFilter.HashBits.Length.ShouldBe(nonSerializableBloomFilter.HashBits.Length);
            readOnlyBloomFilter.Statistics.M.ShouldBe(nonSerializableBloomFilter.Statistics.M);
            readOnlyBloomFilter.Statistics.K.ShouldBe(nonSerializableBloomFilter.Statistics.K);
            readOnlyBloomFilter.Statistics.N.ShouldBe(nonSerializableBloomFilter.Statistics.N);
            readOnlyBloomFilter.Statistics.P.ShouldBe(nonSerializableBloomFilter.Statistics.P);
        }
    }
}
