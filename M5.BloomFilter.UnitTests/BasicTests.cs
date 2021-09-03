using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;

namespace M5.BloomFilter.UnitTests
{
    [TestClass]
    public class BasicTests
    {
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
        public void SimpleTest(HashMethod hashMethod)
        {
            const int ExpectedResults = 1000;

            var test = new BloomIntTest(hashMethod, new[] { 250, 500, 750 } );

            int contains = 0;

            for (int i = 0; i < ExpectedResults; i++)
            {
                if (test.Filter.Contains(i))
                    contains++;
            }

            var f = test.Filter as BloomFilter;

            Console.WriteLine(f.HashBits.Count);
            contains.ShouldBe(3);
        }
    }
}
