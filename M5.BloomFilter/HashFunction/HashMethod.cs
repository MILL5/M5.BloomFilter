using System;

namespace M5.BloomFilter
{
    public enum HashMethod
    {
        Unknown = 0,
        LCGWithFNV1 = 1,
        LCGWithFNV1a = 2,
        LCGModifiedFNV1 = 3,
        RNGWithFNV1 = 4,
        RNGWithFNV1a = 5,
        RNGModifiedFNV1 = 6,
        CRC32 = 7,
        CRC32u = 8,
        Adler32 = 9,
        Murmur2 = 10,
        Murmur3 = 11,
        Murmur3KirschMitzenmacher = 12,
        SHA1 = 13,
        SHA256 = 14,
        SHA384 = 15,
        SHA512 = 16
    }
}