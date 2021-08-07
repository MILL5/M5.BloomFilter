using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M5.BloomFilter
{
    public class BloomFilterJsonConverter : JsonConverter<IImmutableBloomFilter>
    {
        public override IImmutableBloomFilter ReadJson(JsonReader reader, Type objectType, IImmutableBloomFilter existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;
            
            //else if (reader.TokenType != JsonToken.String)
            //    throw new JsonSerializationException(string.Format("Unexpected token {0}", reader.TokenType));
            //var s = (string)reader.Value;
            //var bitArray = new BitArray(s.Length);
            //for (int i = 0; i < s.Length; i++)
            //    bitArray[i] = s[i] == '0' ? false : s[i] == '1' ? true : throw new JsonSerializationException(string.Format("Unknown bit value {0}", s[i]));
            //return bitArray;

            return null;
        }

        public override void WriteJson(JsonWriter writer, IImmutableBloomFilter value, JsonSerializer serializer)
        {
//            writer.WriteValue(value.Cast<bool>().Aggregate(new StringBuilder(value.Length), (sb, b) => sb.Append(b ? "1" : "0")).ToString());
        }
    }
}
