using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace M5.BloomFilter.Serialization
{
    [TestClass]
    public partial class NewtonsoftJsonTests
    {
        private static readonly JsonSerializerSettings _jsonSerializerSettings;

        static NewtonsoftJsonTests()
        {
            var jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.Converters.Add(new BitArrayConverter());
            jsonSerializerSettings.Converters.Add(new BloomFilterConverter());
            _jsonSerializerSettings = jsonSerializerSettings;
        }

        [TestMethod]
        public void SerializeDeserialize()
        {
            var f = FilterBuilder.Build(7000000);

            var json = JsonConvert.SerializeObject(f.ToImmutable(), _jsonSerializerSettings);

            var o = JsonConvert.DeserializeObject<ReadOnlyFilter>(json, _jsonSerializerSettings);
        }
    }
}
