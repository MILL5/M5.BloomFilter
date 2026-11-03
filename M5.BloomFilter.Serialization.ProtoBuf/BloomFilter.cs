using M5.BitArraySerialization;
using ProtoBuf;
using System;
using static Pineapple.Common.Preconditions;

namespace M5.BloomFilter.Serialization
{
    [ProtoContract]
    public class BloomFilter
    {
        public BloomFilter()
        {
        }

        public BloomFilter(IImmutableBloomFilter bloomFilter)
        {
            CheckIsNotNull(nameof(bloomFilter), bloomFilter);

            HashBits = new BitArray(bloomFilter.HashBits);

            var hashMethod = bloomFilter.Hash.ToHashMethod();

            if (hashMethod == HashMethod.Unknown)
                throw new Exception("No hashmethod found.");

            HashMethod = hashMethod;

            var s = bloomFilter.Statistics;

            M = s.M;
            K = s.K;
            N = s.N;
            P = s.P;
            FormatVersion = ReadOnlyFilter.CurrentFormatVersion;
        }

        [ProtoMember(1, Name = "hb")]
        public BitArray HashBits { get; set; }

        [ProtoMember(2, Name = "hm")]
        public HashMethod HashMethod { get; set; }

        [ProtoMember(3, Name = "m")]
        public int M { get; set; }
        [ProtoMember(4, Name = "k")]
        public short K { get; set; }
        [ProtoMember(5, Name = "n")]
        public long N { get; set; }
        [ProtoMember(6, Name = "p")]
        public double P { get; set; }

        [ProtoMember(7, Name = "v")]
        public int FormatVersion { get; set; }

        public ReadOnlyFilter AsBloomFilter()
        {
            if (FormatVersion != ReadOnlyFilter.CurrentFormatVersion)
                throw new NotSupportedException("Unsupported Bloom filter format. Rebuild the filter from original keys with format version 2.");
            if (HashBits == null) throw new FormatException("Bloom filter bit data is missing.");
            return new ReadOnlyFilter(HashBits.AsBitArray(), HashMethod, M, N, K, P, FormatVersion);
        }
    }
}
