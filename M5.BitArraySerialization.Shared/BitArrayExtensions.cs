using System;
using System.Buffers;
using System.IO.Compression;

namespace M5.BitArraySerialization
{
    internal static class BitArrayExtensions
    {
        public static byte[] BitArrayToByteArray(this System.Collections.BitArray bits)
        {
            ArgumentNullException.ThrowIfNull(bits);
            var bytes = new byte[bits.ByteArrayLength()];
            bits.CopyTo(bytes, 0);
            return bytes;
        }

        public static int ByteArrayLength(this System.Collections.BitArray bits) => bits.Length.ByteArrayLength();

        public static int ByteArrayLength(this int numberOfBits)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(numberOfBits);
            return numberOfBits / 8 + (numberOfBits % 8 == 0 ? 0 : 1);
        }

        public static System.Collections.BitArray DecodeBits(byte[] bytes, int length)
        {
            if (length < 0)
                throw new FormatException("Bit length cannot be negative.");
            var expected = length.ByteArrayLength();
            if (bytes == null)
            {
                if (length == 0) bytes = Array.Empty<byte>();
                else throw new FormatException("Bit data is missing.");
            }
            if (length >= 257)
            {
                var output = new byte[expected];
                using var decoder = new BrotliDecoder();
                var status = decoder.Decompress(bytes, output, out var consumed, out var written);
                if (status != OperationStatus.Done || consumed != bytes.Length || written != expected)
                    throw new FormatException("Compressed bit data must contain exactly the declared bits and no trailing data.");
                bytes = output;
            }
            else if (bytes.Length != expected)
                throw new FormatException("Bit data length does not match the declared length.");
            return new System.Collections.BitArray(bytes) { Length = length };
        }
    }
}
