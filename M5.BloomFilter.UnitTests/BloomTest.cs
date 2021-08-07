using System;
using System.Collections.Generic;

namespace M5.BloomFilter.UnitTests
{
    public class BloomIntTest
    {
        public BloomIntTest(HashMethod hashMethod, IEnumerable<int> items, int expectedElements = int.MaxValue, double errorRate = 0.01)
        {
            var filter = FilterBuilder.Build(expectedElements, errorRate, hashMethod);

            foreach (var item in items)
            {
                filter.Add(item);
            }

            Filter = filter;
            Items = items;
        }

        public IBloomFilter Filter { get; }

        public IEnumerable<int> Items { get; }
    }
}
