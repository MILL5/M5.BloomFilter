using System;
using System.Buffers.Binary;

namespace M5.BloomFilter
{
    public class HashChecksumCrc32 : HashChecksum
    {
        public override int[] ComputeHash(byte[] data, int m, int k)
        {
            using var checkSum = new Crc32();
            return ComputeHash(checkSum, data, m, k);
        }
    }

    public class HashChecksumCrc32u : HashChecksum
    {
        public override int[] ComputeHash(byte[] data, int m, int k)
        {
            using var checkSum = new Crc32u();
            return ComputeHash(checkSum, data, m, k);
        }
    }

    public class HashChecksumAdler32 : HashChecksum
    {
        public override int[] ComputeHash(byte[] data, int m, int k)
        {
            using var checkSum = new Adler32();
            return ComputeHash(checkSum, data, m, k);
        }
    }

    public abstract class HashChecksum : HashFunction
    {
        protected int[] ComputeHash(IChecksum checksum, byte[] data, int m, int k)
        {
            if (checksum == null)
            {
                throw new ArgumentNullException(nameof(checksum));
            }

            Validate(data, m, k);
            // Checksums alone have structured collisions (especially Adler32).
            // Independently discriminate the complete key before deriving positions.
            uint discriminator = 2166136261;
            foreach (byte value in data)
                discriminator = unchecked((discriminator ^ value) * 16777619);
            int[] positions = new int[k];
            var counter = new byte[4];
            for (int i = 0; i < k; i++)
            {
                checksum.Reset();
                checksum.Update(data);
                BinaryPrimitives.WriteUInt32LittleEndian(counter, (uint)i);
                checksum.Update(counter);
                uint hash = Mix(unchecked(((uint)checksum.Value ^ discriminator) + 0x9e3779b9U * ((uint)i + 1)));
                positions[i] = (int)(hash % (uint)m);
            }
            return positions;
        }
    }
}