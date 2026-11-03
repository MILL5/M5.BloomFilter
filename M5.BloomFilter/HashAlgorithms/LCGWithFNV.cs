using System;

namespace M5.BloomFilter
{
    public class LCGWithFNV1a : LCGWithFNV
    {
        public override uint Hash(byte[] data)
        {
            int end = data.Length;
            uint hash = Init;
            for (int i = 0; i < end; i++)
            {
                hash ^= data[i];
                hash *= Prime;
            }
            return hash;
        }
    }

    public class LCGModifiedFNV1 : LCGWithFNV1a
    {
        public override uint Hash(byte[] data)
        {
            var hash = base.Hash(data);
            hash += hash << 13;
            hash ^= hash >> 7;
            hash += hash << 3;
            hash ^= hash >> 17;
            hash += hash << 5;
            return hash;
        }
    }

    public class LCGWithFNV : HashFunction
    {
        protected const uint Prime = 16777619;
        protected const uint Init = 2166136261;

        public override int[] ComputeHash(byte[] data, int m, int k)
        {
            Validate(data, m, k);
            const ulong multiplier = 0x5DEECE66DUL;
            const ulong mask = (1UL << 48) - 1;
            ulong seed = Hash(data);
            int[] positions = new int[k];
            for (int i = 0; i < k; i++)
            {
                seed = unchecked(seed * multiplier + 11) & mask;
                positions[i] = (int)((seed >> 16) % (uint)m);
            }
            return positions;
        }

        public virtual uint Hash(byte[] data)
        {
            int end = data.Length;
            uint hash = Init;
            for (int i = 0; i < end; i++)
            {
                hash *= Prime;
                hash ^= data[i];
            }
            return hash;
        }
    }
}