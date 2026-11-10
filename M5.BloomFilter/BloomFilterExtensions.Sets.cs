using System;
using System.Collections;
using System.Numerics;

namespace M5.BloomFilter
{
    public static partial class BloomFilterExtensions
    {
        /// <summary>
        /// Counts the filter's set bits once and reports fill ratio, the
        /// false-positive rate implied by the bits set now, and an estimate of how
        /// many distinct keys were added. On a filter that is being written the
        /// result is a moving estimate.
        /// </summary>
        public static BloomFilterOccupancy GetOccupancy(this IImmutableBloomFilter filter)
        {
            ArgumentNullException.ThrowIfNull(filter);
            var statistics = filter.Statistics;
            int setBits = BuiltIn(filter) is { } bits
                ? bits.CountSetBits()
                : CountSetBits(FilterBits.CopyWords(filter.HashBits, statistics.M));
            return new BloomFilterOccupancy(setBits, statistics.M, statistics.K);
        }

        /// <summary>
        /// True when the two filters have the same size, the same number of hash
        /// positions and the same hash strategy, so their bits can be combined.
        /// </summary>
        public static bool IsCompatibleWith(this IImmutableBloomFilter filter, IImmutableBloomFilter other)
        {
            ArgumentNullException.ThrowIfNull(filter);
            ArgumentNullException.ThrowIfNull(other);
            var left = filter.Statistics;
            var right = other.Statistics;
            if (left.M != right.M || left.K != right.K) return false;
            if (filter.Hash == null || other.Hash == null) return false;
            if (ReferenceEquals(filter.Hash, other.Hash)) return true;
            var method = filter.Hash.ToHashMethod();
            return method != HashMethod.Unknown && method == other.Hash.ToHashMethod();
        }

        /// <summary>
        /// Adds every bit of <paramref name="other"/> to this filter. Afterwards
        /// every key added to either filter tests as possibly present. The filter
        /// must have been created by <see cref="FilterBuilder"/>.
        /// </summary>
        /// <exception cref="ArgumentException">The filters are not compatible.</exception>
        /// <exception cref="NotSupportedException">The target is not a built-in filter.</exception>
        public static void UnionWith(this IBloomFilter filter, IImmutableBloomFilter other)
        {
            ArgumentNullException.ThrowIfNull(filter);
            ArgumentNullException.ThrowIfNull(other);
            if (filter is not BloomFilter builtIn)
                throw new NotSupportedException("UnionWith requires a filter created by FilterBuilder. Use Union to combine other implementations into a snapshot.");
            RequireCompatible(filter, other);
            if (ReferenceEquals(filter, other)) return;
            // Copy the source first so the two locks are never held together.
            builtIn.Bits.Or(Words(other));
        }

        /// <summary>
        /// Returns a snapshot holding every bit of both filters. Its
        /// <see cref="Statistics"/> are this filter's design values; call
        /// <see cref="GetOccupancy"/> for what the merged filter actually holds.
        /// </summary>
        /// <exception cref="ArgumentException">The filters are not compatible.</exception>
        public static ReadOnlyFilter Union(this IImmutableBloomFilter filter, IImmutableBloomFilter other)
        {
            ArgumentNullException.ThrowIfNull(filter);
            ArgumentNullException.ThrowIfNull(other);
            RequireCompatible(filter, other);
            int[] words = Words(filter);
            int[] source = Words(other);
            for (int i = 0; i < words.Length; i++)
                words[i] |= source[i];
            return new ReadOnlyFilter(words, filter.Hash, filter.Statistics);
        }

        private static void RequireCompatible(IImmutableBloomFilter filter, IImmutableBloomFilter other)
        {
            if (!filter.IsCompatibleWith(other))
                throw new ArgumentException("Filters must have the same bit count, hash count and hash strategy to be combined.", nameof(other));
        }

        // A private, consistent copy of a filter's words.
        private static int[] Words(IImmutableBloomFilter filter)
            => BuiltIn(filter) is { } bits
                ? bits.CopyWords()
                : FilterBits.CopyWords(filter.HashBits, filter.Statistics.M);

        private static int CountSetBits(int[] words)
        {
            long count = 0;
            foreach (int word in words)
                count += BitOperations.PopCount((uint)word);
            return (int)count;
        }
    }
}
