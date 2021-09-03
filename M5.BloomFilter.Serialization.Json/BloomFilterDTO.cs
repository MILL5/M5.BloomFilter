using Newtonsoft.Json;
using System;
using System.Collections;
using System.Text.Json.Serialization;
using static Pineapple.Common.Preconditions;

namespace M5.BloomFilter.Serialization
{
    internal class BloomFilterDTO
    {
        public BloomFilterDTO()
        {
        }

        public BloomFilterDTO(ReadOnlyFilter bloomFilter)
        {
            CheckIsNotNull(nameof(bloomFilter), bloomFilter);

            HashBits = bloomFilter.HashBits;

            var hashMethod = bloomFilter.Hash.ToHashMethod();

            if (hashMethod == HashMethod.Unknown)
                throw new Exception("No hashmethod found.");

            HashMethod = hashMethod;

            var s = bloomFilter.Statistics;

            M = s.M;
            K = s.K;
            N = s.N;
            P = s.P;
        }

        [JsonProperty(PropertyName = "hb")]
        [JsonPropertyName(name:"hb")]
        public BitArray HashBits { get; set; }

        [JsonProperty(PropertyName = "hm")]
        [JsonPropertyName(name: "hm")]
        public HashMethod HashMethod { get; set; }

        [JsonProperty(PropertyName = "m")]
        [JsonPropertyName(name: "m")]
        public int M { get; set; }
        
        [JsonProperty(PropertyName = "k")]
        [JsonPropertyName(name: "k")]
        public short K { get; set; }
        
        [JsonProperty(PropertyName = "n")]
        [JsonPropertyName(name: "n")]
        public long N { get; set; }
        
        [JsonProperty(PropertyName = "p")]
        [JsonPropertyName(name: "p")]
        public double P { get; set; }

        public ReadOnlyFilter AsBloomFilter()
        {
            return new ReadOnlyFilter(HashBits, HashMethod, M, N, K, P);
        }
    }
}
