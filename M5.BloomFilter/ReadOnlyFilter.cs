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
        private readonly BitArray _hashBits;
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
            _hashBits = new BitArray(hashBits);
            Hash = HashFunction.GetHashFunction(hashMethod);

            CheckIsNotNull(nameof(hashMethod), Hash);

            Statistics = new Statistics(m, n, k, p);
        }

        public ReadOnlyFilter(IBloomFilter filter)
        {
            CheckIsNotNull(nameof(filter), filter);

            _hashBits = new BitArray(filter.HashBits);
            Hash = filter.Hash;
            Statistics = filter.Statistics;
        }

        public BitArray HashBits => new BitArray(_hashBits);
        public HashFunction Hash { get; }
        public Statistics Statistics { get; }

        public bool Contains(byte[] element)
        {
            var positions = ComputeHash(element);

            foreach (int position in positions)
            {
                if (!_hashBits.Get(position))
                    return false;
            }

            return true;
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

            var processResults = new bool[hashes.Count];

            for (var i = 0; i < hashes.Count; i++)
            {
                processResults[i] = _hashBits.Get(hashes[i]);
            }

            var results = new List<bool>();
            bool isPresent = true;
            int processed = 0;

            //For each value check, if all bits in ranges of hashes bits are set
            foreach (var item in processResults)
            {
                if (!item) isPresent = false;
                if ((processed + 1) % Statistics.K == 0)
                {
                    results.Add(isPresent);
                    isPresent = true;
                }
                processed++;
            }

            return results;
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
