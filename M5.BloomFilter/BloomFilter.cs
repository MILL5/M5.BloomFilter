using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Pineapple.Common.Preconditions;

namespace M5.BloomFilter
{
    internal sealed class BloomFilter : IBloomFilter
    {
        private readonly FilterBits _bits;

        public BloomFilter(long expectedElements, double errorRate, HashFunction hashFunction)
        {
            CheckIsNotLessThanOrEqualTo(nameof(expectedElements), expectedElements, 0);

            if (!double.IsFinite(errorRate) || errorRate >= 1 || errorRate <= 0)
                throw new ArgumentOutOfRangeException(nameof(errorRate), errorRate, $"errorRate must be between 0 and 1, exclusive. Was {errorRate}");

            CheckIsNotNull(nameof(HashFunction), hashFunction);

            Hash = hashFunction;

            Statistics = new Statistics(expectedElements, errorRate);

            _bits = new FilterBits(Statistics.M, Statistics.K, hashFunction);
        }

        public BloomFilter(int capacity, short hashes, HashFunction hashFunction)
        {
            CheckIsNotLessThan(nameof(capacity), capacity, 1);
            CheckIsNotLessThan(nameof(hashes), hashes, 1);
            CheckIsNotNull(nameof(HashFunction), hashFunction);

            Hash = hashFunction;

            Statistics = new Statistics(capacity, hashes);

            _bits = new FilterBits(Statistics.M, Statistics.K, hashFunction);
        }

        internal FilterBits Bits => _bits;

        public BitArray HashBits => _bits.Export();

        public HashFunction Hash { get; }

        public Statistics Statistics { get; } 
        
        public void Add(byte[] element)
        {
            _bits.Add(element);
        }

        public Task AddAsync(byte[] element)
        {
            Add(element);
            return Task.CompletedTask;
        }

        public void Add(IEnumerable<byte[]> elements)
        {
            ArgumentNullException.ThrowIfNull(elements);
            var hashes = new List<int>();

            foreach (var element in elements)
            {
                hashes.AddRange(ComputeHash(element));
            }

            _bits.Set(hashes);
        }

        public Task AddAsync(IEnumerable<byte[]> elements)
        {
            Add(elements);
            return Task.CompletedTask;
        }

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

        public IImmutableBloomFilter ToImmutable()
        {
            return new ReadOnlyFilter(this);
        }

        public override string ToString()
        {
            return $"Capacity:{Statistics.M},Hashes:{Statistics.K},ExpectedElements:{Statistics.N},ErrorRate:{Statistics.P}";
        }
    }
}
