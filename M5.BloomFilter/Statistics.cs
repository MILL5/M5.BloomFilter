using System;
using static Pineapple.Common.Preconditions;

namespace M5.BloomFilter
{
    public class Statistics
    {
        private static int MaxInt(double value)
        {
            return (int)Math.Min(value, int.MaxValue);
        }

        private static long MaxLong(double value)
        {
            return (long)Math.Min(value, long.MaxValue);
        }

        private static short MaxShort(double value)
        {
            return (short)Math.Min(value, short.MaxValue);
        }

        private static int BestM(long n, double p)
        {
            return MaxInt(Math.Ceiling(-1 * (n * Math.Log(p)) / Math.Pow(Math.Log(2), 2)));
        }

        private static short BestK(long n, int m)
        {
            return MaxShort(Math.Ceiling(Math.Log(2) * m / n));
        }

        private static long BestN(int k, int m)
        {
            return MaxLong(Math.Ceiling(Math.Log(2) * m / k));
        }

        private static double BestP(short k, long m, long insertedElements)
        {
            return Math.Pow(1 - Math.Exp(-k * insertedElements * 1.0 / (m * 1.0)), k * 1.0);
        }

        /// <summary>
        /// Use if you know the size of the Bloom Filter and number of hash functions
        /// </summary>
        /// <param name="m">The number of bits in the Bloom Filter</param>
        /// <param name="k">The number of hash functions in the Bloom Filter</param>
        public Statistics(int m, short k)
        {
            CheckIsNotLessThanOrEqualTo(nameof(m), m, 0);
            CheckIsNotLessThanOrEqualTo(nameof(k), k, 0);

            M = m;
            K = k;
            N = BestN(k, m);
            P = BestP(K, M, N);
        }

        /// <summary>
        /// Use if you know the size of the Bloom Filter and approximate number of items 
        /// </summary>
        /// <param name="m">The number of bits in the Bloom Filter</param>
        /// <param name="n">The number of expected items stored in the set</param>
        public Statistics(int m, long n)
        {
            CheckIsNotLessThanOrEqualTo(nameof(m), m, 0);
            CheckIsNotLessThanOrEqualTo(nameof(n), n, 0);

            M = m;
            N = n;
            K = BestK(N, M);
            P = BestP(K, M, N);
        }

        /// <summary>
        /// Use if you know the approximate number of items and probability
        /// </summary>
        /// <param name="n">The number of expected items stored in the set</param>
        /// <param name="p">The desired probability of false positives</param>
        public Statistics(long n, double p)
        {
            CheckIsNotLessThanOrEqualTo(nameof(n), n, 0);

            if ((p <= 0) || (p >= 1))
                throw new ArgumentOutOfRangeException(nameof(p), p, "p must be between 0 and 1.");
 
            CheckIsNotLessThanOrEqualTo(nameof(n), n, 0);

            N = n;
            M = BestM(N, p);
            K = BestK(N, M);
            P = BestP(K, M, N);
        }

        /// <summary>
        /// Capacity of the Bloom Filter
        /// </summary>
        public int M { get; }
        /// <summary>
        /// Number of Hash Functions of the Bloom Filter
        /// </summary>
        public short K { get; }
        /// <summary>
        /// Expected nuber of items in the Bloom Filter
        /// </summary>
        public long N { get; }
        /// <summary>
        /// Probability for a false positive from the Bloom Filter (i.e., error rate)
        /// </summary>
        public double P { get; }
    }
}
