using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;

namespace M5.BloomFilter
{
    public class BloomFilterJsonConverter : JsonConverter<IImmutableBloomFilter>
    {
        public override IImmutableBloomFilter ReadJson(JsonReader reader, Type objectType, IImmutableBloomFilter existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            try
            {
                var value = JObject.Load(reader);
                int version = Required<int>(value, "v");
                if (version != ReadOnlyFilter.CurrentFormatVersion)
                    throw new NotSupportedException("Unsupported Bloom filter format. Rebuild the filter from original keys with format version 2.");
                var bits = value["hb"] as JObject ?? throw new JsonSerializationException("Bloom filter hb object is required.");
                int length = Required<int>(bits, "l");
                if (length < 0) throw new JsonSerializationException("Bit length cannot be negative.");
                var bytes = Convert.FromBase64String(Required<string>(bits, "b"));
                if (bytes.Length != length / 8 + (length % 8 == 0 ? 0 : 1))
                    throw new JsonSerializationException("Bit data length does not match the declared length.");
                return new ReadOnlyFilter(new BitArray(bytes) { Length = length }, Required<HashMethod>(value, "hm"),
                    Required<int>(value, "m"), Required<long>(value, "n"), Required<short>(value, "k"), Required<double>(value, "p"), version);
            }
            catch (NotSupportedException) { throw; }
            catch (JsonSerializationException) { throw; }
            catch (Exception error) when (error is ArgumentException || error is FormatException || error is OverflowException)
            {
                throw new JsonSerializationException("Invalid Bloom filter data.", error);
            }
        }

        private static T Required<T>(JObject value, string name)
        {
            var token = value[name];
            if (token == null || token.Type == JTokenType.Null)
            {
                if (name == "v") throw new NotSupportedException("Legacy Bloom filter format is unsupported. Rebuild the filter from original keys with format version 2.");
                throw new JsonSerializationException("Bloom filter field " + name + " is required.");
            }
            return token.ToObject<T>();
        }

        public override void WriteJson(JsonWriter writer, IImmutableBloomFilter value, JsonSerializer serializer)
        {
            if (value == null) { writer.WriteNull(); return; }
            var bits = value.HashBits;
            var bytes = new byte[bits.Length / 8 + (bits.Length % 8 == 0 ? 0 : 1)];
            bits.CopyTo(bytes, 0);
            var stats = value.Statistics;
            new JObject {
                ["hb"] = new JObject { ["b"] = Convert.ToBase64String(bytes), ["l"] = bits.Length },
                ["hm"] = (int)value.Hash.ToHashMethod(), ["m"] = stats.M, ["k"] = stats.K,
                ["n"] = stats.N, ["p"] = stats.P, ["v"] = ReadOnlyFilter.CurrentFormatVersion
            }.WriteTo(writer);
        }
    }
}
