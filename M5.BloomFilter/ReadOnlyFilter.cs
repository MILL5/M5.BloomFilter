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
        public ReadOnlyFilter(BitArray hashBits, HashMethod hashMethod, int m, long n, short k, double p)
        {
            CheckIsNotNull(nameof(hashBits), hashBits);

            HashBits = hashBits;
            Hash = HashFunction.GetHashFunction(hashMethod);

            CheckIsNotNull(nameof(hashMethod), Hash);

            Statistics = new Statistics(m, n, k, p);
        }

        public ReadOnlyFilter(IBloomFilter filter)
        {
            CheckIsNotNull(nameof(filter), filter);

            HashBits = filter.HashBits.Clone() as BitArray;
            Hash = filter.Hash;
            Statistics = filter.Statistics;
        }

        public BitArray HashBits { get; }
        public HashFunction Hash { get; }
        public Statistics Statistics { get; }

        public bool Contains(byte[] element)
        {
            var positions = ComputeHash(element);

            foreach (int position in positions)
            {
                if (!HashBits.Get(position))
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
            var hashes = new List<int>();
            foreach (var element in elements)
            {
                hashes.AddRange(ComputeHash(element));
            }

            var processResults = new bool[hashes.Count];

            for (var i = 0; i < hashes.Count; i++)
            {
                processResults[i] = HashBits.Get(hashes[i]);
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
            return Hash.ComputeHash(data, Statistics.M, Statistics.K);
        }

        public override string ToString()
        {
            return $"Capacity:{Statistics.M},Hashes:{Statistics.K},ExpectedElements:{Statistics.N},ErrorRate:{Statistics.P}";
        }
    }
}
