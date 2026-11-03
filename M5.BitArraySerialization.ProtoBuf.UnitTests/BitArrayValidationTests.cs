using System;
using System.Collections;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProtoBuf;

namespace M5.BitArraySerialization.ProtoBuf.UnitTests
{
    [TestClass]
    public class BitArrayValidationTests
    {
        [TestMethod]
        public void BoundariesRoundTripThroughProtobuf()
        {
            foreach (var length in new[] { 0, 1, 7, 8, 9, 256, 257, 258 })
            {
                var bits = new System.Collections.BitArray(length);
                for (int i = 0; i < length; i++) bits[i] = i % 3 == 0;
                using var stream = new MemoryStream();
                Serializer.Serialize(stream, new BitArray(bits));
                stream.Position = 0;
                var restored = Serializer.Deserialize<BitArray>(stream).AsBitArray();
                CollectionAssert.AreEqual(bits.Cast<bool>().ToArray(), restored.Cast<bool>().ToArray());
            }
        }

        [TestMethod]
        public void RejectsCorruptionIncludingExtraDataForEmptyArray()
        {
            foreach (var dto in new[] {
                new BitArray { L = -1 }, new BitArray { L = 1 },
                new BitArray { L = 9, B = new byte[1] },
                new BitArray { L = 8, B = new byte[2] },
                new BitArray { L = 0, B = new byte[1] },
                new BitArray { L = 257, B = new byte[] { 0 } } })
                Assert.Throws<FormatException>(() => dto.AsBitArray());
            var compressed = new BitArray(new System.Collections.BitArray(258));
            compressed.B = compressed.B.Concat(new byte[] { 0 }).ToArray();
            Assert.Throws<FormatException>(() => compressed.AsBitArray());
            compressed = new BitArray(new System.Collections.BitArray(258));
            compressed.B = compressed.B.Take(compressed.B.Length - 1).ToArray();
            Assert.Throws<FormatException>(() => compressed.AsBitArray());
            compressed = new BitArray(new System.Collections.BitArray(512));
            compressed.L = 257;
            Assert.Throws<FormatException>(() => compressed.AsBitArray());
            compressed = new BitArray(new System.Collections.BitArray(257));
            compressed.L = 512;
            Assert.Throws<FormatException>(() => compressed.AsBitArray());
            Assert.AreEqual(0, new BitArray().AsBitArray().Length);
            Assert.AreEqual(0, new BitArray(new System.Collections.BitArray(0)).B.Length);
        }
    }
}
