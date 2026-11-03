using System;
using System.Collections;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using STJ = System.Text.Json.JsonSerializer;

namespace M5.BitArraySerialization.Json.UnitTests
{
    [TestClass]
    public class BitArrayValidationTests
    {
        [TestMethod]
        public void BoundariesRoundTripWithBothJsonLibraries()
        {
            var options = new System.Text.Json.JsonSerializerOptions();
            options.Converters.Add(new System.Text.Json.Serialization.BitArrayConverter());
            foreach (var length in new[] { 0, 1, 7, 8, 9, 256, 257, 258 })
            {
                var bits = new BitArray(length);
                for (int i = 0; i < length; i++) bits[i] = i % 3 == 0;
                var nj = JsonConvert.DeserializeObject<BitArray>(JsonConvert.SerializeObject(bits, new Newtonsoft.Json.BitArrayConverter()), new Newtonsoft.Json.BitArrayConverter());
                var sj = STJ.Deserialize<BitArray>(STJ.Serialize(bits, options), options);
                CollectionAssert.AreEqual(bits.Cast<bool>().ToArray(), nj.Cast<bool>().ToArray());
                CollectionAssert.AreEqual(bits.Cast<bool>().ToArray(), sj.Cast<bool>().ToArray());
            }
        }

        [TestMethod]
        public void RejectsInvalidLengthsMissingDataAndTrailingCompressedBytes()
        {
            foreach (var dto in new[] {
                new BitArrayDTO { L = -1, B = Array.Empty<byte>() },
                new BitArrayDTO { L = 1 },
                new BitArrayDTO { L = 9, B = new byte[1] },
                new BitArrayDTO { L = 8, B = new byte[2] },
                new BitArrayDTO { L = 0, B = new byte[1] },
                new BitArrayDTO { L = 257, B = new byte[] { 0 } } })
                Assert.Throws<FormatException>(() => dto.AsBitArray());
            var compressed = new BitArrayDTO(new BitArray(258));
            compressed.B = compressed.B.Concat(new byte[] { 0 }).ToArray();
            Assert.Throws<FormatException>(() => compressed.AsBitArray());
            compressed = new BitArrayDTO(new BitArray(258));
            compressed.B = compressed.B.Take(compressed.B.Length - 1).ToArray();
            Assert.Throws<FormatException>(() => compressed.AsBitArray());
            compressed = new BitArrayDTO(new BitArray(512));
            compressed.L = 257;
            Assert.Throws<FormatException>(() => compressed.AsBitArray());
            compressed = new BitArrayDTO(new BitArray(257));
            compressed.L = 512;
            Assert.Throws<FormatException>(() => compressed.AsBitArray());
        }
    }
}
