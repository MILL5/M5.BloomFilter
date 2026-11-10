using System;
using System.Buffers.Binary;
using System.Collections;
using System.Globalization;
using System.Text;

namespace M5.BloomFilter
{
    public static partial class BloomFilterExtensions
    {
        // Built-in storage when the filter is a built-in (sealed) type whose hash
        // strategy has a span core; otherwise null and the caller takes the 2.0 path.
        private static FilterBits Fast(IImmutableBloomFilter filter) => filter switch
        {
            ReadOnlyFilter snapshot => snapshot.Bits.SpanHash ? snapshot.Bits : null,
            BloomFilter builtIn => builtIn.Bits.SpanHash ? builtIn.Bits : null,
            _ => null,
        };

        // The same little-endian bytes Encode(object) produces, without boxing.
        private static ReadOnlySpan<byte> Bytes(Span<byte> buffer, byte value) { buffer[0] = value; return buffer; }
        private static ReadOnlySpan<byte> Bytes(Span<byte> buffer, short value) { BinaryPrimitives.WriteInt16LittleEndian(buffer, value); return buffer; }
        private static ReadOnlySpan<byte> Bytes(Span<byte> buffer, ushort value) { BinaryPrimitives.WriteUInt16LittleEndian(buffer, value); return buffer; }
        private static ReadOnlySpan<byte> Bytes(Span<byte> buffer, int value) { BinaryPrimitives.WriteInt32LittleEndian(buffer, value); return buffer; }
        private static ReadOnlySpan<byte> Bytes(Span<byte> buffer, uint value) { BinaryPrimitives.WriteUInt32LittleEndian(buffer, value); return buffer; }
        private static ReadOnlySpan<byte> Bytes(Span<byte> buffer, long value) { BinaryPrimitives.WriteInt64LittleEndian(buffer, value); return buffer; }
        private static ReadOnlySpan<byte> Bytes(Span<byte> buffer, ulong value) { BinaryPrimitives.WriteUInt64LittleEndian(buffer, value); return buffer; }

        private static byte[] Encode(object value)
        {
            ArgumentNullException.ThrowIfNull(value);
            switch (value)
            {
                case byte[] bytes: return bytes;
                case string text: return Encoding.UTF8.GetBytes(text);
                case byte number: return new[] { number };
                case short number: { var b = new byte[2]; BinaryPrimitives.WriteInt16LittleEndian(b, number); return b; }
                case ushort number: { var b = new byte[2]; BinaryPrimitives.WriteUInt16LittleEndian(b, number); return b; }
                case int number: { var b = new byte[4]; BinaryPrimitives.WriteInt32LittleEndian(b, number); return b; }
                case uint number: { var b = new byte[4]; BinaryPrimitives.WriteUInt32LittleEndian(b, number); return b; }
                case long number: { var b = new byte[8]; BinaryPrimitives.WriteInt64LittleEndian(b, number); return b; }
                case ulong number: { var b = new byte[8]; BinaryPrimitives.WriteUInt64LittleEndian(b, number); return b; }
                case float number: return Encode(BitConverter.SingleToInt32Bits(number));
                case double number: return Encode(BitConverter.DoubleToInt64Bits(number));
                case DateTime date: return Encode(date.Ticks);
                case IEnumerable: throw new ArgumentException("Pass collections through the IEnumerable<T> overload.", nameof(value));
                default: return Encoding.UTF8.GetBytes(Convert.ToString(value, CultureInfo.InvariantCulture));
            }
        }
    }
}
