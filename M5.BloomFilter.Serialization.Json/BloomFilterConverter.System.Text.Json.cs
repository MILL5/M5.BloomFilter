using System;
using M5.BloomFilter;
using M5.BloomFilter.Serialization;

namespace System.Text.Json.Serialization
{
    public class BloomFilterConverter : JsonConverter<IImmutableBloomFilter>
    {
        public override bool CanConvert(Type typeToConvert)
            => typeToConvert == typeof(IImmutableBloomFilter) || typeToConvert == typeof(ReadOnlyFilter);

        public override IImmutableBloomFilter Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dto = JsonSerializer.Deserialize<BloomFilterDTO>(ref reader, options);
            return dto?.AsBloomFilter();
        }

        public override void Write(Utf8JsonWriter writer, IImmutableBloomFilter value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, new BloomFilterDTO(value), options);
        }
    }
}
