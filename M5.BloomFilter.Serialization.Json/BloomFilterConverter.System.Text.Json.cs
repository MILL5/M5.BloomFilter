using System;
using M5.BloomFilter;
using M5.BloomFilter.Serialization;

namespace System.Text.Json.Serialization
{
    public class BloomFilterConverter : JsonConverter<IImmutableBloomFilter>
    {
        public override IImmutableBloomFilter Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dto = JsonSerializer.Deserialize<BloomFilterDTO>(ref reader, options);
            return dto.AsBloomFilter();
        }

        public override void Write(Utf8JsonWriter writer, IImmutableBloomFilter value, JsonSerializerOptions options)
        {
            var dto = new BloomFilterDTO(value);
            JsonSerializer.Serialize(writer, dto, options);
        }
    }
}