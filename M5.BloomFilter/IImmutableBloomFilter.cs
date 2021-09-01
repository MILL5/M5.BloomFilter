using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace M5.BloomFilter
{
    public interface IImmutableBloomFilter
    {
        public BitArray HashBits { get; }
        public HashFunction Hash { get; }
        public Statistics Statistics { get; }

        bool Contains(byte[] element);
        Task<bool> ContainsAsync(byte[] element);
        ICollection<bool> Contains(IEnumerable<byte[]> elements);
        Task<ICollection<bool>> ContainsAsync(IEnumerable<byte[]> elements);
        bool All(IEnumerable<byte[]> elements);
        Task<bool> AllAsync(IEnumerable<byte[]> elements);
        int[] ComputeHash(byte[] data);
    }
}
