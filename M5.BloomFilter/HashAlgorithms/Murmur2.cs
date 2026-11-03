using System;
using System.Buffers.Binary;

namespace M5.BloomFilter
{
    public class Murmur2 : HashFunction
    {
        public override int[] ComputeHash(byte[] data, int m, int k)
        {
            Validate(data, m, k);
            var positions = new int[k];
            for (int i = 0; i < k; i++)
                positions[i] = (int)(MurmurHash2(unchecked(89478583U + (uint)i * 0x9e3779b9U), data) % (uint)m);
            return positions;
        }

        internal static uint MurmurHash2(uint seed, byte[] data)
        {
            unchecked
            {
                const uint multiplier = 0x5bd1e995;
                uint hash = seed ^ (uint)data.Length;
                int offset = 0;
                while (data.Length - offset >= 4)
                {
                    uint word = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(offset, 4));
                    word *= multiplier;
                    word ^= word >> 24;
                    word *= multiplier;
                    hash = hash * multiplier ^ word;
                    offset += 4;
                }
                switch (data.Length - offset)
                {
                    case 3: hash ^= (uint)data[offset + 2] << 16; goto case 2;
                    case 2: hash ^= (uint)data[offset + 1] << 8; goto case 1;
                    case 1: hash ^= data[offset]; hash *= multiplier; break;
                }
                hash ^= hash >> 13;
                hash *= multiplier;
                return hash ^ (hash >> 15);
            }
        }
    }
}
