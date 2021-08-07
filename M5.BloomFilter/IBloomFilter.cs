using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace M5.BloomFilter
{
    public interface IBloomFilter : IImmutableBloomFilter
    {
        
        void Add(byte[] element);

        Task AddAsync(byte[] element);

        void Add(IEnumerable<byte[]> elements);

        Task AddAsync(IEnumerable<byte[]> elements);
    }
}
