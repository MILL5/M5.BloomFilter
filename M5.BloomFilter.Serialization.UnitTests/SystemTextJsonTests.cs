using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace M5.BloomFilter.Serialization
{
    [TestClass]
    public class SystemTextJsonTests
    {
        private static readonly JsonSerializerOptions _jsonSerializerOptions;

        static SystemTextJsonTests()
        {
            var jsonSerializerOptions = new JsonSerializerOptions();
            jsonSerializerOptions.Converters.Add(new BitArrayConverter());
            jsonSerializerOptions.Converters.Add(new BloomFilterConverter());
            _jsonSerializerOptions = jsonSerializerOptions;
        }

        [TestMethod]
        public void SerializeDeserialize()
        {
            var f = FilterBuilder.Build(7000000);

            var json = JsonSerializer.Serialize(f.ToImmutable(), _jsonSerializerOptions);

            var o = JsonSerializer.Deserialize<ReadOnlyFilter>(json, _jsonSerializerOptions);
        }
    }
}
