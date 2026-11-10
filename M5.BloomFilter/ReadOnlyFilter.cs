using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Pineapple.Common.Preconditions;

namespace M5.BloomFilter
{
    public sealed class ReadOnlyFilter : IImmutableBloomFilter
    {
        public const int CurrentFormatVersion = 2;
        private readonly FilterBits _bits;
        public int FormatVersion => CurrentFormatVersion;
        public ReadOnlyFilter(BitArray hashBits, HashMethod hashMethod, int m, long n, short k, double p)
            : this(hashBits, hashMethod, m, n, k, p, CurrentFormatVersion)
        {
        }

        public ReadOnlyFilter(BitArray hashBits, HashMethod hashMethod, int m, long n, short k, double p, int formatVersion)
        {
            if (formatVersion != CurrentFormatVersion)
                throw new NotSupportedException("Unsupported Bloom filter format. Rebuild the filter from original keys with format version 2.");
            CheckIsNotNull(nameof(hashBits), hashBits);
            if (hashBits.Length != m)
                throw new ArgumentException("Bit length must equal m.", nameof(hashBits));
            Hash = HashFunction.GetHashFunction(hashMethod);

            CheckIsNotNull(nameof(hashMethod), Hash);

            Statistics = new Statistics(m, n, k, p);
            _bits = new FilterBits(hashBits, m, k, Hash);
        }

        public ReadOnlyFilter(IBloomFilter filter)
        {
            CheckIsNotNull(nameof(filter), filter);

            Hash = filter.Hash;
            Statistics = filter.Statistics;
            _bits = filter is BloomFilter builtIn
                ? builtIn.Bits.Snapshot()
                : new FilterBits(filter.HashBits, Statistics.M, Statistics.K, Hash);
        }

        internal ReadOnlyFilter(int[] words, HashFunction hash, Statistics statistics)
        {
            Hash = hash;
            Statistics = statistics;
            _bits = new FilterBits(words, statistics.M, statistics.K, hash);
        }

        internal FilterBits Bits => _bits;

        public BitArray HashBits => _bits.Export();
        public HashFunction Hash { get; }
        public Statistics Statistics { get; }

        public bool Contains(byte[] element)
        {
            return _bits.Contains(element);
        }

        public Task<bool> ContainsAsync(byte[] element)
        {
            return Task.FromResult(Contains(element));
        }

        public ICollection<bool> Contains(IEnumerable<byte[]> elements)
        {
            ArgumentNullException.ThrowIfNull(elements);
            var hashes = new List<int>();
            foreach (var element in elements)
            {
                hashes.AddRange(ComputeHash(element));
            }

            return _bits.TestGroups(hashes);
        }

        public Task<ICollection<bool>> ContainsAsync(IEnumerable<byte[]> elements)
        {
            return Task.FromResult(Contains(elements));
        }

        public bool All(IEnumerable<byte[]> elements)
        {
            return Contains(elements).All(e => e);
        }

        public Task<bool> AllAsync(IEnumerable<byte[]> elements)
        {
            return Task.FromResult(All(elements));
        }

        public int[] ComputeHash(byte[] data)
        {
            ArgumentNullException.ThrowIfNull(data);
            return Hash.ComputeHash(data, Statistics.M, Statistics.K);
        }

        public override string ToString()
        {
            return $"Capacity:{Statistics.M},Hashes:{Statistics.K},ExpectedElements:{Statistics.N},ErrorRate:{Statistics.P}";
        }
    }
}
