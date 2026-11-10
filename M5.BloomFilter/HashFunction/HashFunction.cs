using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace M5.BloomFilter
{
    public abstract class HashFunction
    {
        public readonly static IReadOnlyDictionary<HashMethod, HashFunction> Functions = new ReadOnlyDictionary<HashMethod, HashFunction>(new Dictionary<HashMethod, HashFunction>
        {
            { HashMethod.LCGWithFNV1, new LCGWithFNV() },
            { HashMethod.LCGWithFNV1a, new LCGWithFNV1a() },
            { HashMethod.LCGModifiedFNV1, new LCGModifiedFNV1() },

            { HashMethod.RNGWithFNV1, new RNGWithFNV1() },
            { HashMethod.RNGWithFNV1a, new RNGWithFNV1a() },
            { HashMethod.RNGModifiedFNV1, new RNGModifiedFNV1() },

            { HashMethod.CRC32, new HashChecksumCrc32() },
            { HashMethod.CRC32u, new HashChecksumCrc32u() },
            { HashMethod.Adler32, new HashChecksumAdler32() },

            { HashMethod.Murmur2, new Murmur2() },
            { HashMethod.Murmur3, new Murmur3() },
            { HashMethod.Murmur3KirschMitzenmacher, new Murmur3KirschMitzenmacher() },

            { HashMethod.SHA1, new HashCryptoSHA1() },
            { HashMethod.SHA256, new HashCryptoSHA256() },
            { HashMethod.SHA384, new HashCryptoSHA384() },
            { HashMethod.SHA512, new HashCryptoSHA512() },
        });

        public static HashFunction GetHashFunction(HashMethod hashMethod)
        {
            return Functions.GetValueOrDefault(hashMethod);
        }

        public abstract int[] ComputeHash(byte[] data, int m, int k);

        /// <summary>
        /// Writes positions.Length positions without allocating and returns true,
        /// or returns false when this exact type has no span implementation. An
        /// override must return false for derived types so that a subclass which
        /// overrides the array method keeps being called.
        /// </summary>
        internal virtual bool TryComputeHash(ReadOnlySpan<byte> data, int m, Span<int> positions) => false;

        protected static void Validate(byte[] data, int m, int k)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (m <= 0) throw new ArgumentOutOfRangeException(nameof(m));
            if (k <= 0) throw new ArgumentOutOfRangeException(nameof(k));
        }

        protected int Rejection(uint random, int m)
        {
            if (m <= 0) throw new ArgumentOutOfRangeException(nameof(m));
            ulong range = 1UL << 32;
            return random >= range - range % (uint)m ? -1 : (int)(random % (uint)m);
        }

        protected int Rejection(int random, int m) => Rejection(unchecked((uint)random), m);

        // Murmur3's 32-bit final avalanche; all arithmetic wraps modulo 2^32.
        protected static uint Mix(uint value)
        {
            unchecked
            {
                value ^= value >> 16;
                value *= 0x85ebca6b;
                value ^= value >> 13;
                value *= 0xc2b2ae35;
                return value ^ (value >> 16);
            }
        }

        public static uint NumberOfTrailingZeros(uint i)
        {
            // HD, Figure 5-14
            uint y;
            if (i == 0) return 32;
            uint n = 31;
            y = i << 16; if (y != 0) { n -= 16; i = y; }
            y = i << 8; if (y != 0) { n -= 8; i = y; }
            y = i << 4; if (y != 0) { n -= 4; i = y; }
            y = i << 2; if (y != 0) { n -= 2; i = y; }
            return n - ((i << 1) >> 31);
        }

        public static uint NumberOfLeadingZeros(uint i)
        {
            // HD, Figure 5-6
            if (i == 0)
                return 32;
            uint n = 1;
            if (i >> 16 == 0) { n += 16; i <<= 16; }
            if (i >> 24 == 0) { n += 8; i <<= 8; }
            if (i >> 28 == 0) { n += 4; i <<= 4; }
            if (i >> 30 == 0) { n += 2; i <<= 2; }
            n -= i >> 31;
            return n;
        }

        public static uint RotateLeft(uint original, int bits)
        {
            return (original << bits) | (original >> (32 - bits));
        }

        public static uint RotateRight(uint original, int bits)
        {
            return (original >> bits) | (original << (32 - bits));
        }

        public static ulong RotateLeft(ulong original, int bits)
        {
            return (original << bits) | (original >> (64 - bits));
        }

        public static ulong RotateRight(ulong original, int bits)
        {
            return (original >> bits) | (original << (64 - bits));
        }

        public static int BitToIntOne(BitArray bit, int from, int to)
        {
            const int size = 32;
            int len = to - from;
            int bitCount = bit.Count;
            int result = 0;

            for (int i = 0; i < len && i < bitCount && i < size; i++)
            {
                result = bit[i + from] ? result + (1 << i) : result;
            }

            return result;
        }

        public static int RightMove(int value, int pos)
        {
            if (pos != 0)
            {
                var mask = 0x7fffffff;
                value >>= 1;
                value &= mask;
                value >>= pos - 1;
            }

            return value;
        }

        public static long RightMove(long value, int pos) => (long)((ulong)value >> pos);

        public static uint RightMove(uint value, int pos)
        {
            if (pos != 0)
            {
                uint mask = 0x7fffffff;
                value >>= 1;
                value &= mask;
                value >>= pos - 1;
            }

            return value;
        }
    }
}