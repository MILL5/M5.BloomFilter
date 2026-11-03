using System;
using System.Buffers.Binary;
using System.Security.Cryptography;

namespace M5.BloomFilter
{

    public class HashCryptoSHA1 : HashCrypto
    {
        public override int[] ComputeHash(byte[] data, int m, int k)
        {
            using var hashAlgorithm = SHA1.Create();
            return ComputeHash(hashAlgorithm, data, m, k);
        }
    }

    public class HashCryptoSHA256 : HashCrypto
    {
        public override int[] ComputeHash(byte[] data, int m, int k)
        {
            using var hashAlgorithm = SHA256.Create();
            return ComputeHash(hashAlgorithm, data, m, k);
        }
    }

    public class HashCryptoSHA384 : HashCrypto
    {
        public override int[] ComputeHash(byte[] data, int m, int k)
        {
            using var hashAlgorithm = SHA384.Create();
            return ComputeHash(hashAlgorithm, data, m, k);
        }
    }

    public class HashCryptoSHA512 : HashCrypto
    {
        public override int[] ComputeHash(byte[] data, int m, int k)
        {
            using var hashAlgorithm = SHA512.Create();
            return ComputeHash(hashAlgorithm, data, m, k);
        }
    }

    public abstract class HashCrypto : HashFunction
    {
        protected int[] ComputeHash(HashAlgorithm hashAlgorithm, byte[] data, int m, int k)
        {
            Validate(data, m, k);
            int[] positions = new int[k];
            var input = new byte[checked(data.Length + 4)];
            Buffer.BlockCopy(data, 0, input, 0, data.Length);
            int computed = 0;
            uint block = 0;
            while (computed < k)
            {
                BinaryPrimitives.WriteUInt32LittleEndian(input.AsSpan(data.Length), block++);
                var digest = hashAlgorithm.ComputeHash(input);
                for (int offset = 0; offset + 4 <= digest.Length && computed < k; offset += 4)
                    positions[computed++] = (int)(BinaryPrimitives.ReadUInt32LittleEndian(digest.AsSpan(offset, 4)) % (uint)m);
            }
            return positions;
        }
    }
}