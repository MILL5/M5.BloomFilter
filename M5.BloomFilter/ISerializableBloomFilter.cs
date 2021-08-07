using System;
using System.Collections;

namespace M5.BloomFilter
{
    internal interface ISerializableBloomFilter
    {
        public BitArray HashBits { get; }
        public HashFunction Hash { get; }
        public Statistics Statistics { get; }
    }
}
