using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M5.BloomFilter
{
    /// <summary>
    /// BloomFilterExtensions
    /// </summary>
    public static partial class BloomFilterExtensions
    {
        public static IImmutableBloomFilter ToImmutable(this IBloomFilter bloomFilter)
        {
            return new ReadOnlyFilter(bloomFilter);
        }
    }
}