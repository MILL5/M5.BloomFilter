using ProtoBuf;
using System;
using System.IO.Compression;
using static Pineapple.Common.Preconditions;

namespace M5.BitArraySerialization
{
    [ProtoContract]
    public class BitArray
    {
        private const int CompressLength = 257;

        public BitArray()
        {
        }

        public BitArray(System.Collections.BitArray ba)
        {
            CheckIsNotNull(nameof(ba), ba);

            L = ba.Length;
            var value = ba.BitArrayToByteArray();

            if (L >= CompressLength)
            {
                var ros = new ReadOnlySpan<byte>(value);
                int maxLength = BrotliEncoder.GetMaxCompressedLength(value.Length);
                var s = new Span<byte>(new byte[maxLength]);
                if (!BrotliEncoder.TryCompress(ros, s, out int bytesWritten, 11, 24))
                    throw new Exception("Cannot compress!");
                B = s.Slice(0, bytesWritten).ToArray();
            }
            else
            {
                B = value;
            }
        }

        [ProtoMember(1, Name = "b")]
        public byte[] B { get; set; }

        [ProtoMember(2, Name = "l")]
        public int L { get; set; }

        public static BitArray FromBitArray(System.Collections.BitArray bitArray)
        {
            return new BitArray(bitArray);
        }

        public System.Collections.BitArray AsBitArray()
        {
            if (L == 0)
                return new System.Collections.BitArray(0);

            byte[] bytes;
            
            if (L >= CompressLength)
            {
                var ros = new ReadOnlySpan<byte>(B);
                var s = new Span<byte>(new byte[L.ByteArrayLength()]);

                if (!BrotliDecoder.TryDecompress(ros, s, out int bytesWritten))
                    throw new Exception("Unable to decompress.");

                bytes = s.Slice(0, bytesWritten).ToArray();
            }
            else
            {
                bytes = B;
            }

            return new System.Collections.BitArray(bytes)
            {
                Length = L
            };
        }
    }
}