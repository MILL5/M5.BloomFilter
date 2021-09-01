using System.Collections;

namespace M5.BitArraySerialization
{
    internal static class BitArrayExtensions
    {
        // Copied from this answer https://stackoverflow.com/a/4619295
        // To https://stackoverflow.com/questions/560123/convert-from-bitarray-to-byte
        // By https://stackoverflow.com/users/313088/tedd-hansen
        // And made an extension method.
        public static byte[] BitArrayToByteArray(this System.Collections.BitArray bits)
        {
            byte[] ret = new byte[bits.ByteArrayLength()];
            bits.CopyTo(ret, 0);
            return ret;
        }

        public static int ByteArrayLength(this System.Collections.BitArray bits)
        {
            return (bits.Length - 1) / 8 + 1;
        }

        public static int ByteArrayLength(this int NumberOfBits)
        {
            return (NumberOfBits - 1) / 8 + 1;
        }
    }
}