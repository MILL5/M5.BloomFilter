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
        public static bool Contains(this IImmutableBloomFilter bloomFilter, byte data) => bloomFilter.Contains(new[] { data });

        public static Task<bool> ContainsAsync(this IImmutableBloomFilter bloomFilter, byte data) => bloomFilter.ContainsAsync(new[] { data });

        public static ICollection<bool> Contains(this IImmutableBloomFilter bloomFilter, IEnumerable<byte> elements) => bloomFilter.Contains(elements.Select(data => new byte[] { data }));

        public static Task<ICollection<bool>> ContainsAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<byte> elements) => bloomFilter.ContainsAsync(elements.Select(data => new byte[] { data }));

        public static bool All(this IImmutableBloomFilter bloomFilter, IEnumerable<byte> elements) => bloomFilter.All(elements.Select(data => new byte[] { data }));

        public static Task<bool> AllAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<byte> elements) => bloomFilter.AllAsync(elements.Select(data => new byte[] { data }));

        public static int[] ComputeHash(this IImmutableBloomFilter bloomFilter, byte data) => bloomFilter.ComputeHash(new byte[] { data });
        #endregion Byte

        #region String
        public static bool Contains(this IImmutableBloomFilter bloomFilter, string data) => bloomFilter.Contains(Encoding.UTF8.GetBytes(data));

        public static Task<bool> ContainsAsync(this IImmutableBloomFilter bloomFilter, string data) => bloomFilter.ContainsAsync(Encoding.UTF8.GetBytes(data));

        public static ICollection<bool> Contains(this IImmutableBloomFilter bloomFilter, IEnumerable<string> elements) => bloomFilter.Contains(elements.Select(data => Encoding.UTF8.GetBytes(data)));

        public static Task<ICollection<bool>> ContainsAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<string> elements) => bloomFilter.ContainsAsync(elements.Select(data => Encoding.UTF8.GetBytes(data)));

        public static bool All(this IImmutableBloomFilter bloomFilter, IEnumerable<string> elements) => bloomFilter.All(elements.Select(data => Encoding.UTF8.GetBytes(data)));

        public static Task<bool> AllAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<string> elements) => bloomFilter.AllAsync(elements.Select(data => Encoding.UTF8.GetBytes(data)));

        public static int[] ComputeHash(this IImmutableBloomFilter bloomFilter, string data) => bloomFilter.ComputeHash(Encoding.UTF8.GetBytes(data));
        #endregion String

        #region Double
        public static bool Contains(this IImmutableBloomFilter bloomFilter, double data) => bloomFilter.Contains(BitConverter.GetBytes(data));

        public static Task<bool> ContainsAsync(this IImmutableBloomFilter bloomFilter, double data) => bloomFilter.ContainsAsync(BitConverter.GetBytes(data));

        public static ICollection<bool> Contains(this IImmutableBloomFilter bloomFilter, IEnumerable<double> elements) => bloomFilter.Contains(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<ICollection<bool>> ContainsAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<double> elements) => bloomFilter.ContainsAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static bool All(this IImmutableBloomFilter bloomFilter, IEnumerable<double> elements) => bloomFilter.All(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<bool> AllAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<double> elements) => bloomFilter.AllAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static int[] ComputeHash(this IImmutableBloomFilter bloomFilter, double data) => bloomFilter.ComputeHash(BitConverter.GetBytes(data));
        #endregion Double

        #region Single
        public static bool Contains(this IImmutableBloomFilter bloomFilter, float data) => bloomFilter.Contains(BitConverter.GetBytes(data));

        public static Task<bool> ContainsAsync(this IImmutableBloomFilter bloomFilter, float data) => bloomFilter.ContainsAsync(BitConverter.GetBytes(data));

        public static ICollection<bool> Contains(this IImmutableBloomFilter bloomFilter, IEnumerable<float> elements) => bloomFilter.Contains(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<ICollection<bool>> ContainsAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<float> elements) => bloomFilter.ContainsAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static bool All(this IImmutableBloomFilter bloomFilter, IEnumerable<float> elements) => bloomFilter.All(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<bool> AllAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<float> elements) => bloomFilter.AllAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static int[] ComputeHash(this IImmutableBloomFilter bloomFilter, float data) => bloomFilter.ComputeHash(BitConverter.GetBytes(data));
        #endregion Single

        #region Int16
        public static bool Contains(this IImmutableBloomFilter bloomFilter, short data) => bloomFilter.Contains(BitConverter.GetBytes(data));

        public static Task<bool> ContainsAsync(this IImmutableBloomFilter bloomFilter, short data) => bloomFilter.ContainsAsync(BitConverter.GetBytes(data));

        public static ICollection<bool> Contains(this IImmutableBloomFilter bloomFilter, IEnumerable<short> elements) => bloomFilter.Contains(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<ICollection<bool>> ContainsAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<short> elements) => bloomFilter.ContainsAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static bool All(this IImmutableBloomFilter bloomFilter, IEnumerable<short> elements) => bloomFilter.All(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<bool> AllAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<short> elements) => bloomFilter.AllAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static int[] ComputeHash(this IImmutableBloomFilter bloomFilter, short data) => bloomFilter.ComputeHash(BitConverter.GetBytes(data));
        #endregion Int16

        #region Int32
        public static bool Contains(this IImmutableBloomFilter bloomFilter, int data) => bloomFilter.Contains(BitConverter.GetBytes(data));

        public static Task<bool> ContainsAsync(this IImmutableBloomFilter bloomFilter, int data) => bloomFilter.ContainsAsync(BitConverter.GetBytes(data));

        public static ICollection<bool> Contains(this IImmutableBloomFilter bloomFilter, IEnumerable<int> elements) => bloomFilter.Contains(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<ICollection<bool>> ContainsAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<int> elements) => bloomFilter.ContainsAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static bool All(this IImmutableBloomFilter bloomFilter, IEnumerable<int> elements) => bloomFilter.All(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<bool> AllAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<int> elements) => bloomFilter.AllAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static int[] ComputeHash(this IImmutableBloomFilter bloomFilter, int data) => bloomFilter.ComputeHash(BitConverter.GetBytes(data));
        #endregion Int32

        #region Int64
        public static bool Contains(this IImmutableBloomFilter bloomFilter, long data) => bloomFilter.Contains(BitConverter.GetBytes(data));

        public static Task<bool> ContainsAsync(this IImmutableBloomFilter bloomFilter, long data) => bloomFilter.ContainsAsync(BitConverter.GetBytes(data));

        public static ICollection<bool> Contains(this IImmutableBloomFilter bloomFilter, IEnumerable<long> elements) => bloomFilter.Contains(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<ICollection<bool>> ContainsAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<long> elements) => bloomFilter.ContainsAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static bool All(this IImmutableBloomFilter bloomFilter, IEnumerable<long> elements) => bloomFilter.All(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<bool> AllAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<long> elements) => bloomFilter.AllAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static int[] ComputeHash(this IImmutableBloomFilter bloomFilter, long data) => bloomFilter.ComputeHash(BitConverter.GetBytes(data));
        #endregion Int64

        #region UInt16
        public static bool Contains(this IImmutableBloomFilter bloomFilter, ushort data) => bloomFilter.Contains(BitConverter.GetBytes(data));

        public static Task<bool> ContainsAsync(this IImmutableBloomFilter bloomFilter, ushort data) => bloomFilter.ContainsAsync(BitConverter.GetBytes(data));

        public static ICollection<bool> Contains(this IImmutableBloomFilter bloomFilter, IEnumerable<ushort> elements) => bloomFilter.Contains(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<ICollection<bool>> ContainsAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<ushort> elements) => bloomFilter.ContainsAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static bool All(this IImmutableBloomFilter bloomFilter, IEnumerable<ushort> elements) => bloomFilter.All(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<bool> AllAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<ushort> elements) => bloomFilter.AllAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static int[] ComputeHash(this IImmutableBloomFilter bloomFilter, ushort data) => bloomFilter.ComputeHash(BitConverter.GetBytes(data));
        #endregion UInt16

        #region UInt32
        public static bool Contains(this IImmutableBloomFilter bloomFilter, uint data) => bloomFilter.Contains(BitConverter.GetBytes(data));

        public static Task<bool> ContainsAsync(this IImmutableBloomFilter bloomFilter, uint data) => bloomFilter.ContainsAsync(BitConverter.GetBytes(data));

        public static ICollection<bool> Contains(this IImmutableBloomFilter bloomFilter, IEnumerable<uint> elements) => bloomFilter.Contains(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<ICollection<bool>> ContainsAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<uint> elements) => bloomFilter.ContainsAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static bool All(this IImmutableBloomFilter bloomFilter, IEnumerable<uint> elements) => bloomFilter.All(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<bool> AllAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<uint> elements) => bloomFilter.AllAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static int[] ComputeHash(this IImmutableBloomFilter bloomFilter, uint data) => bloomFilter.ComputeHash(BitConverter.GetBytes(data));
        #endregion UInt32

        #region UInt64
        public static bool Contains(this IImmutableBloomFilter bloomFilter, ulong data) => bloomFilter.Contains(BitConverter.GetBytes(data));

        public static Task<bool> ContainsAsync(this IImmutableBloomFilter bloomFilter, ulong data) => bloomFilter.ContainsAsync(BitConverter.GetBytes(data));

        public static ICollection<bool> Contains(this IImmutableBloomFilter bloomFilter, IEnumerable<ulong> elements) => bloomFilter.Contains(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<ICollection<bool>> ContainsAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<ulong> elements) => bloomFilter.ContainsAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static bool All(this IImmutableBloomFilter bloomFilter, IEnumerable<ulong> elements) => bloomFilter.All(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<bool> AllAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<ulong> elements) => bloomFilter.AllAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static int[] ComputeHash(this IImmutableBloomFilter bloomFilter, ulong data) => bloomFilter.ComputeHash(BitConverter.GetBytes(data));
        #endregion UInt64

        #region DateTime
        public static bool Contains(this IImmutableBloomFilter bloomFilter, DateTime data) => bloomFilter.Contains(BitConverter.GetBytes(data.Ticks));

        public static Task<bool> ContainsAsync(this IImmutableBloomFilter bloomFilter, DateTime data) => bloomFilter.ContainsAsync(BitConverter.GetBytes(data.Ticks));

        public static ICollection<bool> Contains(this IImmutableBloomFilter bloomFilter, IEnumerable<DateTime> elements) => bloomFilter.Contains(elements.Select(data => BitConverter.GetBytes(data.Ticks)));

        public static Task<ICollection<bool>> ContainsAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<DateTime> elements) => bloomFilter.ContainsAsync(elements.Select(data => BitConverter.GetBytes(data.Ticks)));

        public static bool All(this IImmutableBloomFilter bloomFilter, IEnumerable<DateTime> elements) => bloomFilter.All(elements.Select(data => BitConverter.GetBytes(data.Ticks)));

        public static Task<bool> AllAsync(this IImmutableBloomFilter bloomFilter, IEnumerable<DateTime> elements) => bloomFilter.AllAsync(elements.Select(data => BitConverter.GetBytes(data.Ticks)));

        public static int[] ComputeHash(this IImmutableBloomFilter bloomFilter, DateTime data) => bloomFilter.ComputeHash(BitConverter.GetBytes(data.Ticks));
        #endregion DateTime

        #region Object
        public static bool Contains<T>(this IImmutableBloomFilter bloomFilter, T data) => bloomFilter.Contains(Convert.ToString(data, CultureInfo.InvariantCulture));
        public static Task<bool> ContainsAsync<T>(this IImmutableBloomFilter bloomFilter, T data) => bloomFilter.ContainsAsync(Convert.ToString(data, CultureInfo.InvariantCulture));
        public static ICollection<bool> Contains<T>(this IImmutableBloomFilter bloomFilter, IEnumerable<T> elements) => bloomFilter.Contains(elements.Select(data => Encoding.UTF8.GetBytes(Convert.ToString(data, CultureInfo.InvariantCulture))));
        public static Task<ICollection<bool>> ContainsAsync<T>(this IImmutableBloomFilter bloomFilter, IEnumerable<T> elements) => bloomFilter.ContainsAsync(elements.Select(data => Encoding.UTF8.GetBytes(Convert.ToString(data, CultureInfo.InvariantCulture))));
        public static bool All<T>(this IImmutableBloomFilter bloomFilter, IEnumerable<T> elements) => bloomFilter.All(elements.Select(data => Encoding.UTF8.GetBytes(Convert.ToString(data, CultureInfo.InvariantCulture))));
        public static Task<bool> AllAsync<T>(this IImmutableBloomFilter bloomFilter, IEnumerable<T> elements) => bloomFilter.AllAsync(elements.Select(data => Encoding.UTF8.GetBytes(Convert.ToString(data, CultureInfo.InvariantCulture))));
        public static int[] ComputeHash<T>(this IImmutableBloomFilter bloomFilter, T data) => bloomFilter.ComputeHash(Encoding.UTF8.GetBytes(data.ToString()));
        #endregion Object

        public static ReadOnlyFilter ToImmutable(this IBloomFilter bloomFilter)
        {
            return new ReadOnlyFilter(bloomFilter);
        }
    }
}