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

        public static void Add(this IBloomFilter bloomFilter, byte data)
        {
            if (Fast(bloomFilter) is { } bits) bits.Add(Bytes(stackalloc byte[1], data));
            else bloomFilter.Add(new[] { data });
        }

        public static Task AddAsync(this IBloomFilter bloomFilter, byte data) => bloomFilter.AddAsync(new[] { data });

        public static bool Contains(this IBloomFilter bloomFilter, byte data)
            => Fast(bloomFilter) is { } bits ? bits.Contains(Bytes(stackalloc byte[1], data)) : bloomFilter.Contains(new[] { data });

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, byte data) => bloomFilter.ContainsAsync(new[] { data });

        public static void Add(this IBloomFilter bloomFilter, IEnumerable<byte> elements) => bloomFilter.Add(elements.Select(data => new byte[] { data }));

        public static Task AddAsync(this IBloomFilter bloomFilter, IEnumerable<byte> elements) => bloomFilter.AddAsync(elements.Select(data => new byte[] { data }));

        public static ICollection<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<byte> elements) => bloomFilter.Contains(elements.Select(data => new byte[] { data }));

        public static Task<ICollection<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<byte> elements) => bloomFilter.ContainsAsync(elements.Select(data => new byte[] { data }));

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<byte> elements) => bloomFilter.All(elements.Select(data => new byte[] { data }));

        public static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<byte> elements) => bloomFilter.AllAsync(elements.Select(data => new byte[] { data }));

        public static int[] ComputeHash(this IBloomFilter bloomFilter, byte data) => bloomFilter.ComputeHash(new byte[] { data });
        #endregion Byte

        #region String

        public static void Add(this IBloomFilter bloomFilter, string data)
        {
            if (data != null && Fast(bloomFilter) is { } bits) bits.AddUtf8(data);
            else bloomFilter.Add(Encoding.UTF8.GetBytes(data));
        }

        public static Task AddAsync(this IBloomFilter bloomFilter, string data) => bloomFilter.AddAsync(Encoding.UTF8.GetBytes(data));

        public static bool Contains(this IBloomFilter bloomFilter, string data)
            => data != null && Fast(bloomFilter) is { } bits ? bits.ContainsUtf8(data) : bloomFilter.Contains(Encoding.UTF8.GetBytes(data));

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, string data) => bloomFilter.ContainsAsync(Encoding.UTF8.GetBytes(data));

        public static void Add(this IBloomFilter bloomFilter, IEnumerable<string> elements) => bloomFilter.Add(elements.Select(data => Encoding.UTF8.GetBytes(data)));

        public static Task AddAsync(this IBloomFilter bloomFilter, IEnumerable<string> elements) => bloomFilter.AddAsync(elements.Select(data => Encoding.UTF8.GetBytes(data)));

        public static ICollection<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<string> elements) => bloomFilter.Contains(elements.Select(data => Encoding.UTF8.GetBytes(data)));

        public static Task<ICollection<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<string> elements) => bloomFilter.ContainsAsync(elements.Select(data => Encoding.UTF8.GetBytes(data)));

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<string> elements) => bloomFilter.All(elements.Select(data => Encoding.UTF8.GetBytes(data)));

        public static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<string> elements) => bloomFilter.AllAsync(elements.Select(data => Encoding.UTF8.GetBytes(data)));

        public static int[] ComputeHash(this IBloomFilter bloomFilter, string data) => bloomFilter.ComputeHash(Encoding.UTF8.GetBytes(data));
        #endregion String

        #region Double

        public static void Add(this IBloomFilter bloomFilter, double data)
        {
            if (Fast(bloomFilter) is { } bits) bits.Add(Bytes(stackalloc byte[8], BitConverter.DoubleToInt64Bits(data)));
            else bloomFilter.Add(Encode(data));
        }

        public static Task AddAsync(this IBloomFilter bloomFilter, double data) => bloomFilter.AddAsync(Encode(data));

        public static bool Contains(this IBloomFilter bloomFilter, double data)
            => Fast(bloomFilter) is { } bits ? bits.Contains(Bytes(stackalloc byte[8], BitConverter.DoubleToInt64Bits(data))) : bloomFilter.Contains(Encode(data));

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, double data) => bloomFilter.ContainsAsync(Encode(data));

        public static void Add(this IBloomFilter bloomFilter, IEnumerable<double> elements) => bloomFilter.Add(elements.Select(data => Encode(data)));

        public static Task AddAsync(this IBloomFilter bloomFilter, IEnumerable<double> elements) => bloomFilter.AddAsync(elements.Select(data => Encode(data)));

        public static ICollection<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<double> elements) => bloomFilter.Contains(elements.Select(data => Encode(data)));

        public static Task<ICollection<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<double> elements) => bloomFilter.ContainsAsync(elements.Select(data => Encode(data)));

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<double> elements) => bloomFilter.All(elements.Select(data => Encode(data)));

        public static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<double> elements) => bloomFilter.AllAsync(elements.Select(data => Encode(data)));

        public static int[] ComputeHash(this IBloomFilter bloomFilter, double data) => bloomFilter.ComputeHash(Encode(data));
        #endregion Double

        #region Single

        public static void Add(this IBloomFilter bloomFilter, float data)
        {
            if (Fast(bloomFilter) is { } bits) bits.Add(Bytes(stackalloc byte[4], BitConverter.SingleToInt32Bits(data)));
            else bloomFilter.Add(Encode(data));
        }

        public static Task AddAsync(this IBloomFilter bloomFilter, float data) => bloomFilter.AddAsync(Encode(data));

        public static bool Contains(this IBloomFilter bloomFilter, float data)
            => Fast(bloomFilter) is { } bits ? bits.Contains(Bytes(stackalloc byte[4], BitConverter.SingleToInt32Bits(data))) : bloomFilter.Contains(Encode(data));

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, float data) => bloomFilter.ContainsAsync(Encode(data));

        public static void Add(this IBloomFilter bloomFilter, IEnumerable<float> elements) => bloomFilter.Add(elements.Select(data => Encode(data)));

        public static Task AddAsync(this IBloomFilter bloomFilter, IEnumerable<float> elements) => bloomFilter.AddAsync(elements.Select(data => Encode(data)));

        public static ICollection<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<float> elements) => bloomFilter.Contains(elements.Select(data => Encode(data)));

        public static Task<ICollection<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<float> elements) => bloomFilter.ContainsAsync(elements.Select(data => Encode(data)));

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<float> elements) => bloomFilter.All(elements.Select(data => Encode(data)));

        public static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<float> elements) => bloomFilter.AllAsync(elements.Select(data => Encode(data)));

        public static int[] ComputeHash(this IBloomFilter bloomFilter, float data) => bloomFilter.ComputeHash(Encode(data));
        #endregion Single

        #region Int16

        public static void Add(this IBloomFilter bloomFilter, short data)
        {
            if (Fast(bloomFilter) is { } bits) bits.Add(Bytes(stackalloc byte[2], data));
            else bloomFilter.Add(Encode(data));
        }

        public static Task AddAsync(this IBloomFilter bloomFilter, short data) => bloomFilter.AddAsync(Encode(data));

        public static bool Contains(this IBloomFilter bloomFilter, short data)
            => Fast(bloomFilter) is { } bits ? bits.Contains(Bytes(stackalloc byte[2], data)) : bloomFilter.Contains(Encode(data));

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, short data) => bloomFilter.ContainsAsync(Encode(data));

        public static void Add(this IBloomFilter bloomFilter, IEnumerable<short> elements) => bloomFilter.Add(elements.Select(data => Encode(data)));

        public static Task AddAsync(this IBloomFilter bloomFilter, IEnumerable<short> elements) => bloomFilter.AddAsync(elements.Select(data => Encode(data)));

        public static ICollection<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<short> elements) => bloomFilter.Contains(elements.Select(data => Encode(data)));

        public static Task<ICollection<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<short> elements) => bloomFilter.ContainsAsync(elements.Select(data => Encode(data)));

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<short> elements) => bloomFilter.All(elements.Select(data => Encode(data)));

        public static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<short> elements) => bloomFilter.AllAsync(elements.Select(data => Encode(data)));

        public static int[] ComputeHash(this IBloomFilter bloomFilter, short data) => bloomFilter.ComputeHash(Encode(data));
        #endregion Int16

        #region Int32

        public static void Add(this IBloomFilter bloomFilter, int data)
        {
            if (Fast(bloomFilter) is { } bits) bits.Add(Bytes(stackalloc byte[4], data));
            else bloomFilter.Add(Encode(data));
        }

        public static Task AddAsync(this IBloomFilter bloomFilter, int data) => bloomFilter.AddAsync(Encode(data));

        public static bool Contains(this IBloomFilter bloomFilter, int data)
            => Fast(bloomFilter) is { } bits ? bits.Contains(Bytes(stackalloc byte[4], data)) : bloomFilter.Contains(Encode(data));

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, int data) => bloomFilter.ContainsAsync(Encode(data));

        public static void Add(this IBloomFilter bloomFilter, IEnumerable<int> elements) => bloomFilter.Add(elements.Select(data => Encode(data)));

        public static Task AddAsync(this IBloomFilter bloomFilter, IEnumerable<int> elements) => bloomFilter.AddAsync(elements.Select(data => Encode(data)));

        public static ICollection<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<int> elements) => bloomFilter.Contains(elements.Select(data => Encode(data)));

        public static Task<ICollection<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<int> elements) => bloomFilter.ContainsAsync(elements.Select(data => Encode(data)));

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<int> elements) => bloomFilter.All(elements.Select(data => Encode(data)));

        public static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<int> elements) => bloomFilter.AllAsync(elements.Select(data => Encode(data)));

        public static int[] ComputeHash(this IBloomFilter bloomFilter, int data) => bloomFilter.ComputeHash(Encode(data));
        #endregion Int32

        #region Int64

        public static void Add(this IBloomFilter bloomFilter, long data)
        {
            if (Fast(bloomFilter) is { } bits) bits.Add(Bytes(stackalloc byte[8], data));
            else bloomFilter.Add(Encode(data));
        }

        public static Task AddAsync(this IBloomFilter bloomFilter, long data) => bloomFilter.AddAsync(Encode(data));

        public static bool Contains(this IBloomFilter bloomFilter, long data)
            => Fast(bloomFilter) is { } bits ? bits.Contains(Bytes(stackalloc byte[8], data)) : bloomFilter.Contains(Encode(data));

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, long data) => bloomFilter.ContainsAsync(Encode(data));

        public static void Add(this IBloomFilter bloomFilter, IEnumerable<long> elements) => bloomFilter.Add(elements.Select(data => Encode(data)));

        public static Task AddAsync(this IBloomFilter bloomFilter, IEnumerable<long> elements) => bloomFilter.AddAsync(elements.Select(data => Encode(data)));

        public static ICollection<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<long> elements) => bloomFilter.Contains(elements.Select(data => Encode(data)));

        public static Task<ICollection<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<long> elements) => bloomFilter.ContainsAsync(elements.Select(data => Encode(data)));

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<long> elements) => bloomFilter.All(elements.Select(data => Encode(data)));

        public static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<long> elements) => bloomFilter.AllAsync(elements.Select(data => Encode(data)));

        public static int[] ComputeHash(this IBloomFilter bloomFilter, long data) => bloomFilter.ComputeHash(Encode(data));
        #endregion Int64

        #region UInt16

        public static void Add(this IBloomFilter bloomFilter, ushort data)
        {
            if (Fast(bloomFilter) is { } bits) bits.Add(Bytes(stackalloc byte[2], data));
            else bloomFilter.Add(Encode(data));
        }

        public static Task AddAsync(this IBloomFilter bloomFilter, ushort data) => bloomFilter.AddAsync(Encode(data));

        public static bool Contains(this IBloomFilter bloomFilter, ushort data)
            => Fast(bloomFilter) is { } bits ? bits.Contains(Bytes(stackalloc byte[2], data)) : bloomFilter.Contains(Encode(data));

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, ushort data) => bloomFilter.ContainsAsync(Encode(data));

        public static void Add(this IBloomFilter bloomFilter, IEnumerable<ushort> elements) => bloomFilter.Add(elements.Select(data => Encode(data)));

        public static Task AddAsync(this IBloomFilter bloomFilter, IEnumerable<ushort> elements) => bloomFilter.AddAsync(elements.Select(data => Encode(data)));

        public static ICollection<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<ushort> elements) => bloomFilter.Contains(elements.Select(data => Encode(data)));

        public static Task<ICollection<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<ushort> elements) => bloomFilter.ContainsAsync(elements.Select(data => Encode(data)));

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<ushort> elements) => bloomFilter.All(elements.Select(data => Encode(data)));

        public static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<ushort> elements) => bloomFilter.AllAsync(elements.Select(data => Encode(data)));

        public static int[] ComputeHash(this IBloomFilter bloomFilter, ushort data) => bloomFilter.ComputeHash(Encode(data));
        #endregion UInt16

        #region UInt32

        public static void Add(this IBloomFilter bloomFilter, uint data)
        {
            if (Fast(bloomFilter) is { } bits) bits.Add(Bytes(stackalloc byte[4], data));
            else bloomFilter.Add(Encode(data));
        }

        public static Task AddAsync(this IBloomFilter bloomFilter, uint data) => bloomFilter.AddAsync(Encode(data));

        public static bool Contains(this IBloomFilter bloomFilter, uint data)
            => Fast(bloomFilter) is { } bits ? bits.Contains(Bytes(stackalloc byte[4], data)) : bloomFilter.Contains(Encode(data));

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, uint data) => bloomFilter.ContainsAsync(Encode(data));

        public static void Add(this IBloomFilter bloomFilter, IEnumerable<uint> elements) => bloomFilter.Add(elements.Select(data => Encode(data)));

        public static Task AddAsync(this IBloomFilter bloomFilter, IEnumerable<uint> elements) => bloomFilter.AddAsync(elements.Select(data => Encode(data)));

        public static ICollection<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<uint> elements) => bloomFilter.Contains(elements.Select(data => Encode(data)));

        public static Task<ICollection<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<uint> elements) => bloomFilter.ContainsAsync(elements.Select(data => Encode(data)));

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<uint> elements) => bloomFilter.All(elements.Select(data => Encode(data)));

        public static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<uint> elements) => bloomFilter.AllAsync(elements.Select(data => Encode(data)));

        public static int[] ComputeHash(this IBloomFilter bloomFilter, uint data) => bloomFilter.ComputeHash(Encode(data));
        #endregion UInt32

        #region UInt64

        public static void Add(this IBloomFilter bloomFilter, ulong data)
        {
            if (Fast(bloomFilter) is { } bits) bits.Add(Bytes(stackalloc byte[8], data));
            else bloomFilter.Add(Encode(data));
        }

        public static Task AddAsync(this IBloomFilter bloomFilter, ulong data) => bloomFilter.AddAsync(Encode(data));

        public static bool Contains(this IBloomFilter bloomFilter, ulong data)
            => Fast(bloomFilter) is { } bits ? bits.Contains(Bytes(stackalloc byte[8], data)) : bloomFilter.Contains(Encode(data));

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, ulong data) => bloomFilter.ContainsAsync(Encode(data));

        public static void Add(this IBloomFilter bloomFilter, IEnumerable<ulong> elements) => bloomFilter.Add(elements.Select(data => Encode(data)));

        public static Task AddAsync(this IBloomFilter bloomFilter, IEnumerable<ulong> elements) => bloomFilter.AddAsync(elements.Select(data => Encode(data)));

        public static ICollection<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<ulong> elements) => bloomFilter.Contains(elements.Select(data => Encode(data)));

        public static Task<ICollection<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<ulong> elements) => bloomFilter.ContainsAsync(elements.Select(data => Encode(data)));

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<ulong> elements) => bloomFilter.All(elements.Select(data => Encode(data)));

        public static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<ulong> elements) => bloomFilter.AllAsync(elements.Select(data => Encode(data)));

        public static int[] ComputeHash(this IBloomFilter bloomFilter, ulong data) => bloomFilter.ComputeHash(Encode(data));
        #endregion UInt64

        #region DateTime

        public static void Add(this IBloomFilter bloomFilter, DateTime data)
        {
            if (Fast(bloomFilter) is { } bits) bits.Add(Bytes(stackalloc byte[8], data.Ticks));
            else bloomFilter.Add(Encode(data));
        }

        public static Task AddAsync(this IBloomFilter bloomFilter, DateTime data) => bloomFilter.AddAsync(Encode(data));

        public static bool Contains(this IBloomFilter bloomFilter, DateTime data)
            => Fast(bloomFilter) is { } bits ? bits.Contains(Bytes(stackalloc byte[8], data.Ticks)) : bloomFilter.Contains(Encode(data));

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, DateTime data) => bloomFilter.ContainsAsync(Encode(data));

        public static void Add(this IBloomFilter bloomFilter, IEnumerable<DateTime> elements) => bloomFilter.Add(elements.Select(data => Encode(data)));

        public static Task AddAsync(this IBloomFilter bloomFilter, IEnumerable<DateTime> elements) => bloomFilter.AddAsync(elements.Select(data => Encode(data)));

        public static ICollection<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<DateTime> elements) => bloomFilter.Contains(elements.Select(data => Encode(data)));

        public static Task<ICollection<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<DateTime> elements) => bloomFilter.ContainsAsync(elements.Select(data => Encode(data)));

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<DateTime> elements) => bloomFilter.All(elements.Select(data => Encode(data)));

        public static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<DateTime> elements) => bloomFilter.AllAsync(elements.Select(data => Encode(data)));

        public static int[] ComputeHash(this IBloomFilter bloomFilter, DateTime data) => bloomFilter.ComputeHash(Encode(data));
        #endregion DateTime

        #region Object
        public static void Add(this IBloomFilter bloomFilter, object data) => bloomFilter.Add(Encode(data));
        public static Task AddAsync(this IBloomFilter bloomFilter, object data) => bloomFilter.AddAsync(Encode(data));
        public static bool Contains(this IBloomFilter bloomFilter, object data) => bloomFilter.Contains(Encode(data));
        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, object data) => bloomFilter.ContainsAsync(Encode(data));
        public static void Add<T>(this IBloomFilter bloomFilter, IEnumerable<T> elements) => bloomFilter.Add(elements.Select(data => Encode(data)));
        public static Task AddAsync<T>(this IBloomFilter bloomFilter, IEnumerable<T> elements) => bloomFilter.AddAsync(elements.Select(data => Encode(data)));
        public static ICollection<bool> Contains<T>(this IBloomFilter bloomFilter, IEnumerable<T> elements) => bloomFilter.Contains(elements.Select(data => Encode(data)));
        public static Task<ICollection<bool>> ContainsAsync<T>(this IBloomFilter bloomFilter, IEnumerable<T> elements) => bloomFilter.ContainsAsync(elements.Select(data => Encode(data)));
        public static bool All<T>(this IBloomFilter bloomFilter, IEnumerable<T> elements) => bloomFilter.All(elements.Select(data => Encode(data)));
        public static Task<bool> AllAsync<T>(this IBloomFilter bloomFilter, IEnumerable<T> elements) => bloomFilter.AllAsync(elements.Select(data => Encode(data)));
        public static int[] ComputeHash(this IBloomFilter bloomFilter, object data) => bloomFilter.ComputeHash(Encode(data));
        #endregion Object
    }
}