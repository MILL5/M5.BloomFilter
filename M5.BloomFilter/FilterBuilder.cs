﻿using System;

namespace M5.BloomFilter
{
    public partial class FilterBuilder
    {
        public static IBloomFilter Build(int expectedElements)
        {
            return Build(expectedElements, 0.01);
        }

        public static IBloomFilter Build(int expectedElements, HashMethod hashMethod)
        {
            return Build(expectedElements, 0.01, hashMethod);
        }

        public static IBloomFilter Build(int expectedElements, HashFunction hashFunction)
        {
            return Build(expectedElements, 0.01, hashFunction);
        }

        public static IBloomFilter Build(int expectedElements, double errorRate)
        {
            return Build(expectedElements, errorRate, HashFunction.Functions[HashMethod.Murmur3KirschMitzenmacher]);
        }

        public static IBloomFilter Build(int expectedElements, double errorRate, HashMethod hashMethod)
        {
            return new Filter(expectedElements, errorRate, HashFunction.Functions[hashMethod]);
        }

        public static IBloomFilter Build(int expectedElements, double errorRate, HashFunction hashFunction)
        {
            return new Filter(expectedElements, errorRate, hashFunction);
        }
    }
}