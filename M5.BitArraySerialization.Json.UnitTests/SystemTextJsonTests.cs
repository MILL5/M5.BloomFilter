using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using System.Collections;
using Shouldly;
using System;
using System.Text.Json.Serialization;

namespace M5.BitArraySerialization.Json.UnitTests
{
    [TestClass]
    public class SystemTextJsonTests
    {
        private static readonly BitArray _10000bits = new(10000, true);
        private static readonly BitArray _1000bits = new(1000, true);
        private static readonly BitArray _1bit = new(1, false);
        private static readonly BitArray _17bits = new(17, false);
        private static readonly BitArray _29bits = new(29, false);
        private static readonly BitArray _97bits = new(97, false);
        private static readonly BitArray _509bits = new(509, false);

        public TestContext TestContext { get; set; }

        [TestMethod]
        public void DetermineCompressionThreshold()
        {
            int numberOfBits = 1;

            var ba = new BitArray(numberOfBits);
            var baAsBytes = ba.BitArrayToByteArray();
            var baDto = new BitArrayDTO(ba);
            var json = JsonSerializer.Serialize(baDto);
 
            while (json.Length >= baAsBytes.Length)
            {
                numberOfBits++;
                ba = new BitArray(numberOfBits);
                baAsBytes = ba.BitArrayToByteArray();
                baDto = new BitArrayDTO(ba);
                json = JsonSerializer.Serialize(baDto);
            }

            TestContext.WriteLine($"Bits={numberOfBits}");
        }

        [TestMethod]
        public void BitArrayCompare()
        {
            var ba = new BitArray(10000, true);
            ba.ShouldBe(_10000bits);
            ba.ShouldNotBe(_1000bits);
            ba.ShouldNotBe(_509bits);
            ba.ShouldNotBe(_97bits);
            ba.ShouldNotBe(_29bits);
            ba.ShouldNotBe(_17bits);
            ba.ShouldNotBe(_1bit);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), AllowDerivedTypes = true)]
        public void SerializeBitArrayNoJsonSerializerer()
        {
            var jsonOptions = new JsonSerializerOptions();
            var json = JsonSerializer.Serialize(_1000bits, jsonOptions);
            TestContext.WriteLine($"Length of JSON:{json.Length}");
            var ba = JsonSerializer.Deserialize<BitArray>(json);
            ba.ShouldBe(_1000bits);
        }

        [TestMethod]
        public void SerializeBitArrayUsingJsonSerializerer()
        {
            var jsonOptions = new JsonSerializerOptions();
            jsonOptions.Converters.Add(new BitArrayConverter());
            var json = JsonSerializer.Serialize(_1000bits, jsonOptions);
            TestContext.WriteLine($"Length of JSON:{json.Length}");
            var ba = JsonSerializer.Deserialize<BitArray>(json, jsonOptions);
            ba.ShouldBe(_1000bits);
        }

        [TestMethod]
        public void SerializeOneBitUsingJsonSerializerer()
        {
            var jsonOptions = new JsonSerializerOptions();
            jsonOptions.Converters.Add(new BitArrayConverter());
            var json = JsonSerializer.Serialize(_1bit, jsonOptions);
            TestContext.WriteLine($"Length of JSON:{json.Length}");
            var ba = JsonSerializer.Deserialize<BitArray>(json, jsonOptions);
            ba.ShouldBe(_1bit);
        }

        [TestMethod]
        public void SerializeSeventeenBitsUsingJsonSerializerer()
        {
            var jsonOptions = new JsonSerializerOptions();
            jsonOptions.Converters.Add(new BitArrayConverter());
            var json = JsonSerializer.Serialize(_17bits, jsonOptions);
            TestContext.WriteLine($"Length of JSON:{json.Length}");
            var ba = JsonSerializer.Deserialize<BitArray>(json, jsonOptions);
            ba.ShouldBe(_17bits);
        }

        [TestMethod]
        public void SerializeTwentyNineBitsUsingJsonSerializerer()
        {
            var jsonOptions = new JsonSerializerOptions();
            jsonOptions.Converters.Add(new BitArrayConverter());
            var json = JsonSerializer.Serialize(_29bits, jsonOptions);
            TestContext.WriteLine($"Length of JSON:{json.Length}");
            var ba = JsonSerializer.Deserialize<BitArray>(json, jsonOptions);
            ba.ShouldBe(_29bits);
        }

        [TestMethod]
        public void SerializeNinetySevenBitsUsingJsonSerializerer()
        {
            var jsonOptions = new JsonSerializerOptions();
            jsonOptions.Converters.Add(new BitArrayConverter());
            var json = JsonSerializer.Serialize(_97bits, jsonOptions);
            TestContext.WriteLine($"Length of JSON:{json.Length}");
            var ba = JsonSerializer.Deserialize<BitArray>(json, jsonOptions);
            ba.ShouldBe(_97bits);
        }

        [TestMethod]
        public void SerializeFiveHundredNineBitsUsingJsonSerializerer()
        {
            var jsonOptions = new JsonSerializerOptions();
            jsonOptions.Converters.Add(new BitArrayConverter());
            var json = JsonSerializer.Serialize(_509bits, jsonOptions);
            TestContext.WriteLine($"Length of JSON:{json.Length}");
            var ba = JsonSerializer.Deserialize<BitArray>(json, jsonOptions);
            ba.ShouldBe(_509bits);
        }

        [TestMethod]
        public void SerializeTenThousandBitsUsingJsonSerializerer()
        {
            var jsonOptions = new JsonSerializerOptions();
            jsonOptions.Converters.Add(new BitArrayConverter());
            var json = JsonSerializer.Serialize(_10000bits, jsonOptions);
            TestContext.WriteLine($"Length of JSON:{json.Length}");
            var ba = JsonSerializer.Deserialize<BitArray>(json, jsonOptions);
            ba.ShouldBe(_10000bits);
        }
    }
}