using System;

namespace M5.BloomFilter
{
    /// <summary>Design capacity and the ideal independent-hash false-positive estimate.</summary>
    public class Statistics
    {
        private static void Positive(long value, string name)
        {
            if (value <= 0) throw new ArgumentOutOfRangeException(name);
        }

        private static void Probability(double value, string name)
        {
            if (!double.IsFinite(value) || value <= 0 || value >= 1)
                throw new ArgumentOutOfRangeException(name, "Probability must be finite and strictly between zero and one.");
        }

        private static double BestP(short k, int m, long n)
            => Math.Pow(1 - Math.Exp(-(double)k * n / m), k);

        private static short BestK(long n, int m)
        {
            double ideal = Math.Log(2) * m / n;
            short lower = (short)Math.Clamp(Math.Floor(ideal), 1, short.MaxValue);
            short upper = (short)Math.Clamp(Math.Ceiling(ideal), 1, short.MaxValue);
            return BestP(lower, m, n) <= BestP(upper, m, n) ? lower : upper;
        }

        public Statistics(int m, short k)
        {
            Positive(m, nameof(m));
            Positive(k, nameof(k));
            M = m;
            K = k;
            N = Math.Max(1, (long)Math.Floor(Math.Log(2) * m / k));
            P = BestP(K, M, N);
        }

        public Statistics(int m, long n)
        {
            Positive(m, nameof(m));
            Positive(n, nameof(n));
            M = m;
            N = n;
            K = BestK(n, m);
            P = BestP(K, M, N);
        }

        public Statistics(long n, double p)
        {
            Positive(n, nameof(n));
            Probability(p, nameof(p));
            double bits = Math.Ceiling(-n * Math.Log(p) / (Math.Log(2) * Math.Log(2)));
            M = CheckedBits(bits);
            N = n;
            K = BestK(n, M);
            // Integer k can need a few more bits than the continuous optimum.
            if (BestP(K, M, n) > p)
            {
                bits = Math.Ceiling(-(double)K * n / Math.Log(1 - Math.Pow(p, 1.0 / K)));
                M = CheckedBits(Math.Max(M, bits));
                K = BestK(n, M);
            }
            P = BestP(K, M, N);
        }

        private static int CheckedBits(double bits)
        {
            if (!double.IsFinite(bits) || bits > int.MaxValue)
                throw new ArgumentOutOfRangeException("n", "Requested filter exceeds Int32 bit capacity.");
            return Math.Max(1, (int)bits);
        }

        public Statistics(int m, long n, short k, double p)
        {
            Positive(m, nameof(m));
            Positive(n, nameof(n));
            Positive(k, nameof(k));
            // An estimate can underflow to zero or round to one at extreme load.
            if (!double.IsFinite(p) || p < 0 || p > 1)
                throw new ArgumentOutOfRangeException(nameof(p));
            M = m;
            N = n;
            K = k;
            P = p;
        }

        public int M { get; }
        public short K { get; }
        public long N { get; }
        public double P { get; }
    }
}
