using System;
using System.Text;

namespace M5.BloomFilter
{
    public static partial class BloomFilterExtensions
    {
        /// <summary>
        /// Adds one key from a span of bytes. Sets the same bits as <c>Add(byte[])</c>
        /// and does not allocate for the default hash strategy.
        /// </summary>
        public static void AddKey(this IBloomFilter filter, ReadOnlySpan<byte> key)
        {
            ArgumentNullException.ThrowIfNull(filter);
            if (BuiltIn(filter) is { } bits) bits.Add(key);
            else filter.Add(key.ToArray());
        }

        /// <summary>
        /// Adds one key from a span of characters, encoded as UTF-8. Sets the same
        /// bits as <c>Add(string)</c>. A null string converted to a span is an empty
        /// span and therefore the empty key; use the string overload to reject null.
        /// </summary>
        public static void AddKey(this IBloomFilter filter, ReadOnlySpan<char> key)
        {
            ArgumentNullException.ThrowIfNull(filter);
            if (BuiltIn(filter) is { } bits) bits.AddUtf8(key);
            else filter.Add(Utf8(key));
        }

        /// <summary>Adds one string key, encoded as UTF-8. Null is rejected.</summary>
        public static void AddKey(this IBloomFilter filter, string key)
        {
            ArgumentNullException.ThrowIfNull(key);
            filter.AddKey(key.AsSpan());
        }

        /// <summary>
        /// Tests one key from a span of bytes. False means definitely absent; true
        /// means possibly present. Reads the same bits as <c>Contains(byte[])</c>.
        /// </summary>
        public static bool ContainsKey(this IImmutableBloomFilter filter, ReadOnlySpan<byte> key)
        {
            ArgumentNullException.ThrowIfNull(filter);
            return BuiltIn(filter) is { } bits ? bits.Contains(key) : filter.Contains(key.ToArray());
        }

        /// <summary>
        /// Tests one key from a span of characters, encoded as UTF-8. A null string
        /// converted to a span is the empty key; use the string overload to reject null.
        /// </summary>
        public static bool ContainsKey(this IImmutableBloomFilter filter, ReadOnlySpan<char> key)
        {
            ArgumentNullException.ThrowIfNull(filter);
            return BuiltIn(filter) is { } bits ? bits.ContainsUtf8(key) : filter.Contains(Utf8(key));
        }

        /// <summary>Tests one string key, encoded as UTF-8. Null is rejected.</summary>
        public static bool ContainsKey(this IImmutableBloomFilter filter, string key)
        {
            ArgumentNullException.ThrowIfNull(key);
            return filter.ContainsKey(key.AsSpan());
        }

        // Storage of a built-in (sealed) filter, whatever its hash strategy.
        private static FilterBits BuiltIn(IImmutableBloomFilter filter) => filter switch
        {
            ReadOnlyFilter snapshot => snapshot.Bits,
            BloomFilter builtIn => builtIn.Bits,
            _ => null,
        };

        // One key is always one byte[]; never hand a char[] to the batch overloads.
        private static byte[] Utf8(ReadOnlySpan<char> key)
        {
            var bytes = new byte[Encoding.UTF8.GetByteCount(key)];
            Encoding.UTF8.GetBytes(key, bytes);
            return bytes;
        }
    }
}
