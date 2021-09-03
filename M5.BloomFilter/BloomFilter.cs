using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Pineapple.Common.Preconditions;

namespace M5.BloomFilter
{
    internal class BloomFilter : IBloomFilter
    {
        private readonly object _sync = new();

        public BloomFilter(long expectedElements, double errorRate, HashFunction hashFunction)
        {
            CheckIsNotLessThanOrEqualTo(nameof(expectedElements), expectedElements, 0);

            if (errorRate >= 1 || errorRate <= 0)
                throw new ArgumentOutOfRangeException(nameof(errorRate), errorRate, $"errorRate must be between 0 and 1, exclusive. Was {errorRate}");

            CheckIsNotNull(nameof(HashFunction), hashFunction);

            Hash = hashFunction;

            Statistics = new Statistics(expectedElements, errorRate);

            HashBits = new BitArray(Statistics.M);
        }

        public BloomFilter(int capacity, short hashes, HashFunction hashFunction)
        {
            CheckIsNotLessThan(nameof(capacity), capacity, 1);
            CheckIsNotLessThan(nameof(hashes), hashes, 1);
            CheckIsNotNull(nameof(HashFunction), hashFunction);

            Hash = hashFunction;

            Statistics = new Statistics(capacity, hashes);

            HashBits = new BitArray(Statistics.M);
        }

        public BitArray HashBits { get; }

        public HashFunction Hash { get; }

        public Statistics Statistics { get; } 
        
        public virtual void Add(byte[] element)
        {
            var positions = ComputeHash(element);

            lock (_sync)
            {
                foreach (int position in positions)
                {
                    HashBits.Set(position, true);
                }
            }
        }

        public virtual Task AddAsync(byte[] element)
        {
            Add(element);
            return Task.CompletedTask;
        }

        public virtual void Add(IEnumerable<byte[]> elements)
        {
            var hashes = new List<int>();

            foreach (var element in elements)
            {
                hashes.AddRange(ComputeHash(element));
            }

            lock (_sync)
            {
                for (var i = 0; i < hashes.Count; i++)
                {
                    HashBits.Set(hashes[i], true);
                }
            }
        }

        public virtual Task AddAsync(IEnumerable<byte[]> elements)
        {
            Add(elements);
            return Task.CompletedTask;
        }

        public virtual bool Contains(byte[] element)
        {
            var positions = ComputeHash(element);
            
            lock (_sync)
            {
                foreach (int position in positions)
                {
                    if (!HashBits.Get(position))
                        return false;
                }
            }

            return true;
        }

        public virtual Task<bool> ContainsAsync(byte[] element)
        {
            return Task.FromResult(Contains(element));
        }

        public virtual ICollection<bool> Contains(IEnumerable<byte[]> elements)
        {
            var hashes = new List<int>();
            foreach (var element in elements)
            {
                hashes.AddRange(ComputeHash(element));
            }

            var processResults = new bool[hashes.Count];
            lock (_sync)
            {
                for (var i = 0; i < hashes.Count; i++)
                {
                    processResults[i] = HashBits.Get(hashes[i]);
                }
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

        public virtual Task<ICollection<bool>> ContainsAsync(IEnumerable<byte[]> elements)
        {
            return Task.FromResult(Contains(elements));
        }

        public virtual bool All(IEnumerable<byte[]> elements)
        {
            return Contains(elements).All(e => e);
        }

        public virtual Task<bool> AllAsync(IEnumerable<byte[]> elements)
        {
            return Task.FromResult(All(elements));
        }

        public int[] ComputeHash(byte[] data)
        {
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
