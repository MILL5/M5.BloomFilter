using System;
using System.Collections;
using M5.BloomFilter;
using M5.BloomFilter.Serialization;

namespace Newtonsoft.Json
{
    public class BloomFilterConverter : JsonConverter<IImmutableBloomFilter>
    {
        public override IImmutableBloomFilter ReadJson(JsonReader reader, Type objectType, IImmutableBloomFilter existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var dto = serializer.Deserialize<BloomFilterDTO>(reader);
            if (dto == null)
                return null;
            var bloomFilter = dto.AsBloomFilter();
            return bloomFilter;
        }

        public override void WriteJson(JsonWriter writer, IImmutableBloomFilter value, JsonSerializer serializer)
        {
            var dto = new BloomFilterDTO(value);
            serializer.Serialize(writer, dto);
        }
    }
}