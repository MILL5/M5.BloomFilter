using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace M5.BloomFilter
{
    /// <summary>
    /// Building a Better Bloom Filter" by Adam Kirsch and Michael Mitzenmacher,
    /// https://www.eecs.harvard.edu/~michaelm/postscripts/tr-02-05.pdf
    /// </summary>
    public class Murmur3KirschMitzenmacher : Murmur3
    {
        public override int[] ComputeHash(byte[] data, int m, int k)
        {
            Validate(data, m, k);
            int[] positions = new int[k];
            long hash1 = MurmurHash3_32(89478583, data, 0, data.Length);
            long hash2 = MurmurHash3_32((uint)hash1, data, 0, data.Length);
            if (hash2 == 0) hash2 = 0x9e3779b9;
            for (int i = 0; i < k; i++)
            {
                positions[i] = (int)((hash1 + i * hash2) % m);
            }
            return positions;
        }

        internal override bool TryComputeHash(ReadOnlySpan<byte> data, int m, Span<int> positions)
        {
            if (GetType() != typeof(Murmur3KirschMitzenmacher)) return false;
            if (m <= 0) throw new ArgumentOutOfRangeException(nameof(m));
            First(data, m, out int position, out uint step);
            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] = position;
                position = Next(position, step, m);
            }
            return true;
        }

        // Position i is (hash1 + i * hash2) mod m. It is produced by stepping:
        // start at hash1 mod m and add hash2 mod m each time, subtracting m when
        // the sum reaches it. Both residues are below m <= 2^31 - 1, so the sum
        // fits in a uint and (sum - m) fits in an int.
        internal static void First(ReadOnlySpan<byte> data, int m, out int position, out uint step)
        {
            uint hash1 = MurmurHash3_32(89478583, data);
            uint hash2 = MurmurHash3_32(hash1, data);
            if (hash2 == 0) hash2 = 0x9e3779b9;
            position = (int)(hash1 % (uint)m);
            step = hash2 % (uint)m;
        }

        // Branch-free, because whether the sum wraps is effectively random.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int Next(int position, uint step, int m)
        {
            int over = (int)((uint)position + step - (uint)m);
            return over + ((over >> 31) & m);
        }
    }

    public class Murmur3 : HashFunction
    {
        public override int[] ComputeHash(byte[] data, int m, int k)
        {
            Validate(data, m, k);
            int[] positions = new int[k];
            for (int i = 0; i < k; i++)
                positions[i] = (int)(MurmurHash3_32(unchecked(89478583U + (uint)i * 0x9e3779b9U), data, 0, data.Length) % (uint)m);
            return positions;
        }

        internal override bool TryComputeHash(ReadOnlySpan<byte> data, int m, Span<int> positions)
        {
            if (GetType() != typeof(Murmur3)) return false;
            if (m <= 0) throw new ArgumentOutOfRangeException(nameof(m));
            for (int i = 0; i < positions.Length; i++)
                positions[i] = (int)(MurmurHash3_32(unchecked(89478583U + (uint)i * 0x9e3779b9U), data) % (uint)m);
            return true;
        }

        protected static uint MurmurHash3_32(uint seed, byte[] data, int offset, int count)
            => MurmurHash3_32(seed, data.AsSpan(offset, count));

        // One core for arrays and spans; positions are pinned by the 2.0.1 fixtures.
        private protected static uint MurmurHash3_32(uint seed, ReadOnlySpan<byte> data)
        {
            const uint c1 = 0xcc9e2d51;
            const uint c2 = 0x1b873593;

            int length = data.Length;
            int blocks = length / 4;
            uint hash = seed;

            uint k1;
            // body
            for (int i = 0; i < blocks; ++i)
            {
                k1 = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(i * 4, 4));

                k1 *= c1;
                k1 = RotateLeft(k1, 15);
                k1 *= c2;

                hash ^= k1;
                hash = RotateLeft(hash, 13);
                hash = hash * 5 + 0xe6546b64;
            }

            // tail
            k1 = 0;
            var tailIdx = blocks * 4;
            switch (length & 3)
            {
                case 3:
                    k1 ^= (uint)(data[tailIdx + 2]) << 16;
                    goto case 2;
                case 2:
                    k1 ^= (uint)(data[tailIdx + 1]) << 8;
                    goto case 1;
                case 1:
                    k1 ^= data[tailIdx + 0];
                    k1 *= c1; k1 = RotateLeft(k1, 15);
                    k1 *= c2;
                    hash ^= k1;
                    break;
            };

            // finalization
            hash ^= (uint)length;

            hash ^= RightMove(hash, 16);
            hash *= 0x85ebca6b;
            hash ^= RightMove(hash, 13);
            hash *= 0xc2b2ae35;
            hash ^= RightMove(hash, 16);

            return hash;
        }
    }
}