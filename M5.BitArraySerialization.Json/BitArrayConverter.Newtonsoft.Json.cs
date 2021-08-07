using System;
using System.Collections;
using M5.BitArraySerialization.Json;

namespace Newtonsoft.Json
{
    public class BitArrayConverter : JsonConverter<BitArray>
    {
        public override BitArray ReadJson(JsonReader reader, Type objectType, BitArray existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var dto = serializer.Deserialize<BitArrayDTO>(reader);
            if (dto == null)
                return null;
            var bitArray = dto.AsBitArray();
            bitArray.Length = dto.L;
            return bitArray;
        }

        public override void WriteJson(JsonWriter writer, BitArray value, JsonSerializer serializer)
        {
            var dto = new BitArrayDTO(value);
            serializer.Serialize(writer, dto);
        }
    }
}