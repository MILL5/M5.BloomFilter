using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections;
using Shouldly;
using System;

namespace M5.BitArraySerialization.Json.UnitTests
{
    [TestClass]
    public class NewtonsoftJsonTests
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
            var json = JsonConvert.SerializeObject(baDto);
 
            while (json.Length >= baAsBytes.Length)
            {
                numberOfBits++;
                ba = new BitArray(numberOfBits);
                baAsBytes = ba.BitArrayToByteArray();
                baDto = new BitArrayDTO(ba);
                json = JsonConvert.SerializeObject(baDto);
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
        public void SerializeObjectBitArrayNoJsonConverter()
        {
            var jsonSettings = new JsonSerializerSettings();
            var json = JsonConvert.SerializeObject(_1000bits, jsonSettings);
            TestContext.WriteLine($"Length of JSON:{json.Length}");
            var ba = JsonConvert.DeserializeObject<BitArray>(json);
            ba.ShouldBe(_1000bits);
        }

        [TestMethod]
        public void SerializeObjectBitArrayUsingJsonConverter()
        {
            var jsonSettings = new JsonSerializerSettings();
            jsonSettings.Converters.Add(new BitArrayConverter());
            var json = JsonConvert.SerializeObject(_1000bits, jsonSettings);
            TestContext.WriteLine($"Length of JSON:{json.Length}");
            var ba = JsonConvert.DeserializeObject<BitArray>(json, jsonSettings);
            ba.ShouldBe(_1000bits);
        }

        [TestMethod]
        public void SerializeObjectOneBitUsingJsonConverter()
        {
            var jsonSettings = new JsonSerializerSettings();
            jsonSettings.Converters.Add(new BitArrayConverter());
            var json = JsonConvert.SerializeObject(_1bit, jsonSettings);
            TestContext.WriteLine($"Length of JSON:{json.Length}");
            var ba = JsonConvert.DeserializeObject<BitArray>(json, jsonSettings);
            ba.ShouldBe(_1bit);
        }

        [TestMethod]
        public void SerializeObjectSeventeenBitsUsingJsonConverter()
        {
            var jsonSettings = new JsonSerializerSettings();
            jsonSettings.Converters.Add(new BitArrayConverter());
            var json = JsonConvert.SerializeObject(_17bits, jsonSettings);
            TestContext.WriteLine($"Length of JSON:{json.Length}");
            var ba = JsonConvert.DeserializeObject<BitArray>(json, jsonSettings);
            ba.ShouldBe(_17bits);
        }

        [TestMethod]
        public void SerializeObjectTwentyNineBitsUsingJsonConverter()
        {
            var jsonSettings = new JsonSerializerSettings();
            jsonSettings.Converters.Add(new BitArrayConverter());
            var json = JsonConvert.SerializeObject(_29bits, jsonSettings);
            TestContext.WriteLine($"Length of JSON:{json.Length}");
            var ba = JsonConvert.DeserializeObject<BitArray>(json, jsonSettings);
            ba.ShouldBe(_29bits);
        }

        [TestMethod]
        public void SerializeObjectNinetySevenBitsUsingJsonConverter()
        {
            var jsonSettings = new JsonSerializerSettings();
            jsonSettings.Converters.Add(new BitArrayConverter());
            var json = JsonConvert.SerializeObject(_97bits, jsonSettings);
            TestContext.WriteLine($"Length of JSON:{json.Length}");
            var ba = JsonConvert.DeserializeObject<BitArray>(json, jsonSettings);
            ba.ShouldBe(_97bits);
        }

        [TestMethod]
        public void SerializeObjectFiveHundredNineBitsUsingJsonConverter()
        {
            var jsonSettings = new JsonSerializerSettings();
            jsonSettings.Converters.Add(new BitArrayConverter());
            var json = JsonConvert.SerializeObject(_509bits, jsonSettings);
            TestContext.WriteLine($"Length of JSON:{json.Length}");
            var ba = JsonConvert.DeserializeObject<BitArray>(json, jsonSettings);
            ba.ShouldBe(_509bits);
        }

        [TestMethod]
        public void SerializeObjectTenThousandBitsUsingJsonConverter()
        {
            var jsonSettings = new JsonSerializerSettings();
            jsonSettings.Converters.Add(new BitArrayConverter());
            var json = JsonConvert.SerializeObject(_10000bits, jsonSettings);
            TestContext.WriteLine($"Length of JSON:{json.Length}");
            var ba = JsonConvert.DeserializeObject<BitArray>(json, jsonSettings);
            ba.ShouldBe(_10000bits);
        }
    }
}