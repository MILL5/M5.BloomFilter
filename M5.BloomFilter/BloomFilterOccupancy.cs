using System;

namespace M5.BloomFilter
{
    /// <summary>
    /// What a filter currently holds, measured from its bits. <see cref="Statistics"/>
    /// describes the design; this describes the contents.
    /// </summary>
    public readonly struct BloomFilterOccupancy : IEquatable<BloomFilterOccupancy>
    {
        public BloomFilterOccupancy(int setBits, int totalBits, int hashCount)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(totalBits);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(hashCount);
            ArgumentOutOfRangeException.ThrowIfNegative(setBits);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(setBits, totalBits);
            SetBits = setBits;
            TotalBits = totalBits;
            HashCount = hashCount;
        }

        /// <summary>Number of bits set to one.</summary>
        public int SetBits { get; }

        /// <summary>Filter size in bits (m).</summary>
        public int TotalBits { get; }

        /// <summary>Positions tested per key (k).</summary>
        public int HashCount { get; }

        /// <summary>Fraction of bits set, from 0 to 1.</summary>
        public double FillRatio => TotalBits == 0 ? 0 : (double)SetBits / TotalBits;

        /// <summary>
        /// Probability that a key which was never added tests as possibly present,
        /// given the bits set now: FillRatio raised to HashCount. Assumes the hash
        /// spreads keys evenly; it is an estimate, not a guarantee.
        /// </summary>
        public double EstimatedFalsePositiveRate => HashCount == 0 ? 0 : Math.Pow(FillRatio, HashCount);

        /// <summary>
        /// Estimated number of distinct keys added: -(m / k) ln(1 - FillRatio).
        /// Positive infinity when every bit is set.
        /// </summary>
        public double EstimatedCount
        {
            get
            {
                if (HashCount == 0 || SetBits == 0) return 0;
                if (SetBits == TotalBits) return double.PositiveInfinity;
                return -(double)TotalBits / HashCount * double.LogP1(-FillRatio);
            }
        }

        public bool Equals(BloomFilterOccupancy other)
            => SetBits == other.SetBits && TotalBits == other.TotalBits && HashCount == other.HashCount;

        public override bool Equals(object obj) => obj is BloomFilterOccupancy other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(SetBits, TotalBits, HashCount);

        public static bool operator ==(BloomFilterOccupancy left, BloomFilterOccupancy right) => left.Equals(right);

        public static bool operator !=(BloomFilterOccupancy left, BloomFilterOccupancy right) => !left.Equals(right);

        public override string ToString()
            => $"SetBits:{SetBits},TotalBits:{TotalBits},Hashes:{HashCount},FillRatio:{FillRatio:0.####}";
    }
}
