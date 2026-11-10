using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace M5.BloomFilter
{
    /// <summary>
    /// BloomFilterExtensions
    /// </summary>
    public static partial class BloomFilterExtensions
    {
        #region Byte
        public static bool Contains(this IImmutableBloomFilter bloomFilter, byte data)
            => Fast(bloomFilter) is { } bits ? bits.Contains(Bytes(stackalloc byte[1], data)) : bloomFilter.Contains(new[] { data });

        public static Task<bool> ContainsAsync(this IImmutableBloomFilter bloomFilter, byte data) => bloomFilter.ContainsAsync(new[] { data });

        public static ICollection<bool> Contains(this IImmutableBloomFilter bloomFilter, IEnumerable<byte> elements) => bloomFilter.Contains(elements.Select(data => new byte[] { data }));

        public static Task<ICollection<bool>> ContainsAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<byte> elements) => bloomFilter.ContainsAsync(elements.Select(data => new byte[] { data }));

        public static bool All(this IImmutableBloomFilter bloomFilter, IEnumerable<byte> elements) => bloomFilter.All(elements.Select(data => new byte[] { data }));

        public static Task<bool> AllAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<byte> elements) => bloomFilter.AllAsync(elements.Select(data => new byte[] { data }));

        public static int[] ComputeHash(this IImmutableBloomFilter bloomFilter, byte data) => bloomFilter.ComputeHash(new byte[] { data });
        #endregion Byte

        #region String
        public static bool Contains(this IImmutableBloomFilter bloomFilter, string data)
            => data != null && Fast(bloomFilter) is { } bits ? bits.ContainsUtf8(data) : bloomFilter.Contains(Encoding.UTF8.GetBytes(data));

        public static Task<bool> ContainsAsync(this IImmutableBloomFilter bloomFilter, string data) => bloomFilter.ContainsAsync(Encoding.UTF8.GetBytes(data));

        public static ICollection<bool> Contains(this IImmutableBloomFilter bloomFilter, IEnumerable<string> elements) => bloomFilter.Contains(elements.Select(data => Encoding.UTF8.GetBytes(data)));

        public static Task<ICollection<bool>> ContainsAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<string> elements) => bloomFilter.ContainsAsync(elements.Select(data => Encoding.UTF8.GetBytes(data)));

        public static bool All(this IImmutableBloomFilter bloomFilter, IEnumerable<string> elements) => bloomFilter.All(elements.Select(data => Encoding.UTF8.GetBytes(data)));

        public static Task<bool> AllAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<string> elements) => bloomFilter.AllAsync(elements.Select(data => Encoding.UTF8.GetBytes(data)));

        public static int[] ComputeHash(this IImmutableBloomFilter bloomFilter, string data) => bloomFilter.ComputeHash(Encoding.UTF8.GetBytes(data));
        #endregion String

        #region Double
        public static bool Contains(this IImmutableBloomFilter bloomFilter, double data)
            => Fast(bloomFilter) is { } bits ? bits.Contains(Bytes(stackalloc byte[8], BitConverter.DoubleToInt64Bits(data))) : bloomFilter.Contains(Encode(data));

        public static Task<bool> ContainsAsync(this IImmutableBloomFilter bloomFilter, double data) => bloomFilter.ContainsAsync(Encode(data));

        public static ICollection<bool> Contains(this IImmutableBloomFilter bloomFilter, IEnumerable<double> elements) => bloomFilter.Contains(elements.Select(data => Encode(data)));

        public static Task<ICollection<bool>> ContainsAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<double> elements) => bloomFilter.ContainsAsync(elements.Select(data => Encode(data)));

        public static bool All(this IImmutableBloomFilter bloomFilter, IEnumerable<double> elements) => bloomFilter.All(elements.Select(data => Encode(data)));

        public static Task<bool> AllAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<double> elements) => bloomFilter.AllAsync(elements.Select(data => Encode(data)));

        public static int[] ComputeHash(this IImmutableBloomFilter bloomFilter, double data) => bloomFilter.ComputeHash(Encode(data));
        #endregion Double

        #region Single
        public static bool Contains(this IImmutableBloomFilter bloomFilter, float data)
            => Fast(bloomFilter) is { } bits ? bits.Contains(Bytes(stackalloc byte[4], BitConverter.SingleToInt32Bits(data))) : bloomFilter.Contains(Encode(data));

        public static Task<bool> ContainsAsync(this IImmutableBloomFilter bloomFilter, float data) => bloomFilter.ContainsAsync(Encode(data));

        public static ICollection<bool> Contains(this IImmutableBloomFilter bloomFilter, IEnumerable<float> elements) => bloomFilter.Contains(elements.Select(data => Encode(data)));

        public static Task<ICollection<bool>> ContainsAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<float> elements) => bloomFilter.ContainsAsync(elements.Select(data => Encode(data)));

        public static bool All(this IImmutableBloomFilter bloomFilter, IEnumerable<float> elements) => bloomFilter.All(elements.Select(data => Encode(data)));

        public static Task<bool> AllAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<float> elements) => bloomFilter.AllAsync(elements.Select(data => Encode(data)));

        public static int[] ComputeHash(this IImmutableBloomFilter bloomFilter, float data) => bloomFilter.ComputeHash(Encode(data));
        #endregion Single

        #region Int16
        public static bool Contains(this IImmutableBloomFilter bloomFilter, short data)
            => Fast(bloomFilter) is { } bits ? bits.Contains(Bytes(stackalloc byte[2], data)) : bloomFilter.Contains(Encode(data));

        public static Task<bool> ContainsAsync(this IImmutableBloomFilter bloomFilter, short data) => bloomFilter.ContainsAsync(Encode(data));

        public static ICollection<bool> Contains(this IImmutableBloomFilter bloomFilter, IEnumerable<short> elements) => bloomFilter.Contains(elements.Select(data => Encode(data)));

        public static Task<ICollection<bool>> ContainsAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<short> elements) => bloomFilter.ContainsAsync(elements.Select(data => Encode(data)));

        public static bool All(this IImmutableBloomFilter bloomFilter, IEnumerable<short> elements) => bloomFilter.All(elements.Select(data => Encode(data)));

        public static Task<bool> AllAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<short> elements) => bloomFilter.AllAsync(elements.Select(data => Encode(data)));

        public static int[] ComputeHash(this IImmutableBloomFilter bloomFilter, short data) => bloomFilter.ComputeHash(Encode(data));
        #endregion Int16

        #region Int32
        public static bool Contains(this IImmutableBloomFilter bloomFilter, int data)
            => Fast(bloomFilter) is { } bits ? bits.Contains(Bytes(stackalloc byte[4], data)) : bloomFilter.Contains(Encode(data));

        public static Task<bool> ContainsAsync(this IImmutableBloomFilter bloomFilter, int data) => bloomFilter.ContainsAsync(Encode(data));

        public static ICollection<bool> Contains(this IImmutableBloomFilter bloomFilter, IEnumerable<int> elements) => bloomFilter.Contains(elements.Select(data => Encode(data)));

        public static Task<ICollection<bool>> ContainsAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<int> elements) => bloomFilter.ContainsAsync(elements.Select(data => Encode(data)));

        public static bool All(this IImmutableBloomFilter bloomFilter, IEnumerable<int> elements) => bloomFilter.All(elements.Select(data => Encode(data)));

        public static Task<bool> AllAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<int> elements) => bloomFilter.AllAsync(elements.Select(data => Encode(data)));

        public static int[] ComputeHash(this IImmutableBloomFilter bloomFilter, int data) => bloomFilter.ComputeHash(Encode(data));
        #endregion Int32

        #region Int64
        public static bool Contains(this IImmutableBloomFilter bloomFilter, long data)
            => Fast(bloomFilter) is { } bits ? bits.Contains(Bytes(stackalloc byte[8], data)) : bloomFilter.Contains(Encode(data));

        public static Task<bool> ContainsAsync(this IImmutableBloomFilter bloomFilter, long data) => bloomFilter.ContainsAsync(Encode(data));

        public static ICollection<bool> Contains(this IImmutableBloomFilter bloomFilter, IEnumerable<long> elements) => bloomFilter.Contains(elements.Select(data => Encode(data)));

        public static Task<ICollection<bool>> ContainsAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<long> elements) => bloomFilter.ContainsAsync(elements.Select(data => Encode(data)));

        public static bool All(this IImmutableBloomFilter bloomFilter, IEnumerable<long> elements) => bloomFilter.All(elements.Select(data => Encode(data)));

        public static Task<bool> AllAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<long> elements) => bloomFilter.AllAsync(elements.Select(data => Encode(data)));

        public static int[] ComputeHash(this IImmutableBloomFilter bloomFilter, long data) => bloomFilter.ComputeHash(Encode(data));
        #endregion Int64

        #region UInt16
        public static bool Contains(this IImmutableBloomFilter bloomFilter, ushort data)
            => Fast(bloomFilter) is { } bits ? bits.Contains(Bytes(stackalloc byte[2], data)) : bloomFilter.Contains(Encode(data));

        public static Task<bool> ContainsAsync(this IImmutableBloomFilter bloomFilter, ushort data) => bloomFilter.ContainsAsync(Encode(data));

        public static ICollection<bool> Contains(this IImmutableBloomFilter bloomFilter, IEnumerable<ushort> elements) => bloomFilter.Contains(elements.Select(data => Encode(data)));

        public static Task<ICollection<bool>> ContainsAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<ushort> elements) => bloomFilter.ContainsAsync(elements.Select(data => Encode(data)));

        public static bool All(this IImmutableBloomFilter bloomFilter, IEnumerable<ushort> elements) => bloomFilter.All(elements.Select(data => Encode(data)));

        public static Task<bool> AllAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<ushort> elements) => bloomFilter.AllAsync(elements.Select(data => Encode(data)));

        public static int[] ComputeHash(this IImmutableBloomFilter bloomFilter, ushort data) => bloomFilter.ComputeHash(Encode(data));
        #endregion UInt16

        #region UInt32
        public static bool Contains(this IImmutableBloomFilter bloomFilter, uint data)
            => Fast(bloomFilter) is { } bits ? bits.Contains(Bytes(stackalloc byte[4], data)) : bloomFilter.Contains(Encode(data));

        public static Task<bool> ContainsAsync(this IImmutableBloomFilter bloomFilter, uint data) => bloomFilter.ContainsAsync(Encode(data));

        public static ICollection<bool> Contains(this IImmutableBloomFilter bloomFilter, IEnumerable<uint> elements) => bloomFilter.Contains(elements.Select(data => Encode(data)));

        public static Task<ICollection<bool>> ContainsAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<uint> elements) => bloomFilter.ContainsAsync(elements.Select(data => Encode(data)));

        public static bool All(this IImmutableBloomFilter bloomFilter, IEnumerable<uint> elements) => bloomFilter.All(elements.Select(data => Encode(data)));

        public static Task<bool> AllAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<uint> elements) => bloomFilter.AllAsync(elements.Select(data => Encode(data)));

        public static int[] ComputeHash(this IImmutableBloomFilter bloomFilter, uint data) => bloomFilter.ComputeHash(Encode(data));
        #endregion UInt32

        #region UInt64
        public static bool Contains(this IImmutableBloomFilter bloomFilter, ulong data)
            => Fast(bloomFilter) is { } bits ? bits.Contains(Bytes(stackalloc byte[8], data)) : bloomFilter.Contains(Encode(data));

        public static Task<bool> ContainsAsync(this IImmutableBloomFilter bloomFilter, ulong data) => bloomFilter.ContainsAsync(Encode(data));

        public static ICollection<bool> Contains(this IImmutableBloomFilter bloomFilter, IEnumerable<ulong> elements) => bloomFilter.Contains(elements.Select(data => Encode(data)));

        public static Task<ICollection<bool>> ContainsAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<ulong> elements) => bloomFilter.ContainsAsync(elements.Select(data => Encode(data)));

        public static bool All(this IImmutableBloomFilter bloomFilter, IEnumerable<ulong> elements) => bloomFilter.All(elements.Select(data => Encode(data)));

        public static Task<bool> AllAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<ulong> elements) => bloomFilter.AllAsync(elements.Select(data => Encode(data)));

        public static int[] ComputeHash(this IImmutableBloomFilter bloomFilter, ulong data) => bloomFilter.ComputeHash(Encode(data));
        #endregion UInt64

        #region DateTime
        public static bool Contains(this IImmutableBloomFilter bloomFilter, DateTime data)
            => Fast(bloomFilter) is { } bits ? bits.Contains(Bytes(stackalloc byte[8], data.Ticks)) : bloomFilter.Contains(Encode(data));

        public static Task<bool> ContainsAsync(this IImmutableBloomFilter bloomFilter, DateTime data) => bloomFilter.ContainsAsync(Encode(data));

        public static ICollection<bool> Contains(this IImmutableBloomFilter bloomFilter, IEnumerable<DateTime> elements) => bloomFilter.Contains(elements.Select(data => Encode(data)));

        public static Task<ICollection<bool>> ContainsAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<DateTime> elements) => bloomFilter.ContainsAsync(elements.Select(data => Encode(data)));

        public static bool All(this IImmutableBloomFilter bloomFilter, IEnumerable<DateTime> elements) => bloomFilter.All(elements.Select(data => Encode(data)));

        public static Task<bool> AllAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<DateTime> elements) => bloomFilter.AllAsync(elements.Select(data => Encode(data)));

        public static int[] ComputeHash(this IImmutableBloomFilter bloomFilter, DateTime data) => bloomFilter.ComputeHash(Encode(data));
        #endregion DateTime

        #region Object
        public static bool Contains(this IImmutableBloomFilter bloomFilter, object data) => bloomFilter.Contains(Encode(data));
        public static Task<bool> ContainsAsync(this IImmutableBloomFilter bloomFilter, object data) => bloomFilter.ContainsAsync(Encode(data));
        public static ICollection<bool> Contains<T>(this IImmutableBloomFilter bloomFilter, IEnumerable<T> elements) => bloomFilter.Contains(elements.Select(data => Encode(data)));
        public static Task<ICollection<bool>> ContainsAsync<T>(this IImmutableBloomFilter bloomFilter, IEnumerable<T> elements) => bloomFilter.ContainsAsync(elements.Select(data => Encode(data)));
        public static bool All<T>(this IImmutableBloomFilter bloomFilter, IEnumerable<T> elements) => bloomFilter.All(elements.Select(data => Encode(data)));
        public static Task<bool> AllAsync<T>(this IImmutableBloomFilter bloomFilter, IEnumerable<T> elements) => bloomFilter.AllAsync(elements.Select(data => Encode(data)));
        public static int[] ComputeHash(this IImmutableBloomFilter bloomFilter, object data) => bloomFilter.ComputeHash(Encode(data));
        #endregion Object

        public static ReadOnlyFilter ToImmutable(this IBloomFilter bloomFilter)
        {
            return new ReadOnlyFilter(bloomFilter);
        }
    }
}