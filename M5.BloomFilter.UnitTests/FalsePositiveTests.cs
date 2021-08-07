using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace M5.BloomFilter.UnitTests
{
    [TestClass]
    public class FalsePositiveTests
    {
        private IEnumerable<int> _items;

        public FalsePositiveTests()
        {
            var items = new List<int>(100);

            for (int i = 0; i < 1000; i++)
            {
                if (i % 10 == 0)
                {
                    items.Add(i);
                }
            }

            items.Add(3);
            items.Add(15);
            items.Add(111);

            _items = items;
        }

        [DataRow(HashMethod.LCGModifiedFNV1)]
        [DataRow(HashMethod.LCGWithFNV1)]
        [DataRow(HashMethod.LCGWithFNV1a)]
        [DataRow(HashMethod.Murmur2)]
        [DataRow(HashMethod.Murmur3KirschMitzenmacher)]
        [DataRow(HashMethod.RNGModifiedFNV1)]
        [DataRow(HashMethod.RNGWithFNV1)]
        [DataRow(HashMethod.RNGWithFNV1a)]
        [DataRow(HashMethod.SHA1)]
        [DataRow(HashMethod.SHA256)]
        [DataRow(HashMethod.SHA384)]
        [DataRow(HashMethod.SHA512)]
        [DataTestMethod]
        public void TenPercent(HashMethod hashMethod)
        {
            const int ExpectedResults = 1000;
            const double ErrorRate = 0.01;

            var test = new BloomIntTest(hashMethod, _items, expectedElements : ExpectedResults, errorRate : ErrorRate);

            var s = (test.Filter as Filter).Statistics;

            Console.WriteLine($"M={s.M}");
            Console.WriteLine($"K={s.K}");
            Console.WriteLine($"N={s.N}");
            Console.WriteLine($"P={s.P}");

            var probablyIn = new List<int>(100);
            var notIn = new List<int>(1000);

            for (int i = 0; i < ExpectedResults; i++)
            {
                if (test.Filter.Contains(i))
                {
                    probablyIn.Add(i);
                }
                else
                {
                    notIn.Add(i);
                }
            }

            Console.WriteLine($"Probably In Count: {probablyIn.Count}");
            Console.WriteLine($"Not In Count: {notIn.Count}");
        }
    }
}
