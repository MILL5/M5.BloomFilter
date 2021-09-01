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

        public static void Add(this IBloomFilter bloomFilter, byte data) => bloomFilter.Add(new[] { data });

        public static Task AddAsync(this IBloomFilter bloomFilter, byte data) => bloomFilter.AddAsync(new[] { data });

        public static bool Contains(this IBloomFilter bloomFilter, byte data) => bloomFilter.Contains(new[] { data });

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

        public static void Add(this IBloomFilter bloomFilter, string data) => bloomFilter.Add(Encoding.UTF8.GetBytes(data));

        public static Task AddAsync(this IBloomFilter bloomFilter, string data) => bloomFilter.AddAsync(Encoding.UTF8.GetBytes(data));

        public static bool Contains(this IBloomFilter bloomFilter, string data) => bloomFilter.Contains(Encoding.UTF8.GetBytes(data));

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

        public static void Add(this IBloomFilter bloomFilter, double data) => bloomFilter.Add(BitConverter.GetBytes(data));

        public static Task AddAsync(this IBloomFilter bloomFilter, double data) => bloomFilter.AddAsync(BitConverter.GetBytes(data));

        public static bool Contains(this IBloomFilter bloomFilter, double data) => bloomFilter.Contains(BitConverter.GetBytes(data));

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, double data) => bloomFilter.ContainsAsync(BitConverter.GetBytes(data));

        public static void Add(this IBloomFilter bloomFilter, IEnumerable<double> elements) => bloomFilter.Add(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task AddAsync(this IBloomFilter bloomFilter, IEnumerable<double> elements) => bloomFilter.AddAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static ICollection<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<double> elements) => bloomFilter.Contains(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<ICollection<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<double> elements) => bloomFilter.ContainsAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<double> elements) => bloomFilter.All(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<double> elements) => bloomFilter.AllAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static int[] ComputeHash(this IBloomFilter bloomFilter, double data) => bloomFilter.ComputeHash(BitConverter.GetBytes(data));
        #endregion Double

        #region Single

        public static void Add(this IBloomFilter bloomFilter, float data) => bloomFilter.Add(BitConverter.GetBytes(data));

        public static Task AddAsync(this IBloomFilter bloomFilter, float data) => bloomFilter.AddAsync(BitConverter.GetBytes(data));

        public static bool Contains(this IBloomFilter bloomFilter, float data) => bloomFilter.Contains(BitConverter.GetBytes(data));

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, float data) => bloomFilter.ContainsAsync(BitConverter.GetBytes(data));

        public static void Add(this IBloomFilter bloomFilter, IEnumerable<float> elements) => bloomFilter.Add(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task AddAsync(this IBloomFilter bloomFilter, IEnumerable<float> elements) => bloomFilter.AddAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static ICollection<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<float> elements) => bloomFilter.Contains(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<ICollection<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<float> elements) => bloomFilter.ContainsAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<float> elements) => bloomFilter.All(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<float> elements) => bloomFilter.AllAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static int[] ComputeHash(this IBloomFilter bloomFilter, float data) => bloomFilter.ComputeHash(BitConverter.GetBytes(data));
        #endregion Single

        #region Int16

        public static void Add(this IBloomFilter bloomFilter, short data) => bloomFilter.Add(BitConverter.GetBytes(data));

        public static Task AddAsync(this IBloomFilter bloomFilter, short data) => bloomFilter.AddAsync(BitConverter.GetBytes(data));

        public static bool Contains(this IBloomFilter bloomFilter, short data) => bloomFilter.Contains(BitConverter.GetBytes(data));

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, short data) => bloomFilter.ContainsAsync(BitConverter.GetBytes(data));

        public static void Add(this IBloomFilter bloomFilter, IEnumerable<short> elements) => bloomFilter.Add(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task AddAsync(this IBloomFilter bloomFilter, IEnumerable<short> elements) => bloomFilter.AddAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static ICollection<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<short> elements) => bloomFilter.Contains(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<ICollection<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<short> elements) => bloomFilter.ContainsAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<short> elements) => bloomFilter.All(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<short> elements) => bloomFilter.AllAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static int[] ComputeHash(this IBloomFilter bloomFilter, short data) => bloomFilter.ComputeHash(BitConverter.GetBytes(data));
        #endregion Int16

        #region Int32

        public static void Add(this IBloomFilter bloomFilter, int data) => bloomFilter.Add(BitConverter.GetBytes(data));

        public static Task AddAsync(this IBloomFilter bloomFilter, int data) => bloomFilter.AddAsync(BitConverter.GetBytes(data));

        public static bool Contains(this IBloomFilter bloomFilter, int data) => bloomFilter.Contains(BitConverter.GetBytes(data));

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, int data) => bloomFilter.ContainsAsync(BitConverter.GetBytes(data));

        public static void Add(this IBloomFilter bloomFilter, IEnumerable<int> elements) => bloomFilter.Add(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task AddAsync(this IBloomFilter bloomFilter, IEnumerable<int> elements) => bloomFilter.AddAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static ICollection<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<int> elements) => bloomFilter.Contains(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<ICollection<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<int> elements) => bloomFilter.ContainsAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<int> elements) => bloomFilter.All(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<int> elements) => bloomFilter.AllAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static int[] ComputeHash(this IBloomFilter bloomFilter, int data) => bloomFilter.ComputeHash(BitConverter.GetBytes(data));
        #endregion Int32

        #region Int64

        public static void Add(this IBloomFilter bloomFilter, long data) => bloomFilter.Add(BitConverter.GetBytes(data));

        public static Task AddAsync(this IBloomFilter bloomFilter, long data) => bloomFilter.AddAsync(BitConverter.GetBytes(data));

        public static bool Contains(this IBloomFilter bloomFilter, long data) => bloomFilter.Contains(BitConverter.GetBytes(data));

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, long data) => bloomFilter.ContainsAsync(BitConverter.GetBytes(data));

        public static void Add(this IBloomFilter bloomFilter, IEnumerable<long> elements) => bloomFilter.Add(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task AddAsync(this IBloomFilter bloomFilter, IEnumerable<long> elements) => bloomFilter.AddAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static ICollection<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<long> elements) => bloomFilter.Contains(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<ICollection<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<long> elements) => bloomFilter.ContainsAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<long> elements) => bloomFilter.All(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<long> elements) => bloomFilter.AllAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static int[] ComputeHash(this IBloomFilter bloomFilter, long data) => bloomFilter.ComputeHash(BitConverter.GetBytes(data));
        #endregion Int64

        #region UInt16

        public static void Add(this IBloomFilter bloomFilter, ushort data) => bloomFilter.Add(BitConverter.GetBytes(data));

        public static Task AddAsync(this IBloomFilter bloomFilter, ushort data) => bloomFilter.AddAsync(BitConverter.GetBytes(data));

        public static bool Contains(this IBloomFilter bloomFilter, ushort data) => bloomFilter.Contains(BitConverter.GetBytes(data));

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, ushort data) => bloomFilter.ContainsAsync(BitConverter.GetBytes(data));

        public static void Add(this IBloomFilter bloomFilter, IEnumerable<ushort> elements) => bloomFilter.Add(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task AddAsync(this IBloomFilter bloomFilter, IEnumerable<ushort> elements) => bloomFilter.AddAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static ICollection<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<ushort> elements) => bloomFilter.Contains(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<ICollection<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<ushort> elements) => bloomFilter.ContainsAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<ushort> elements) => bloomFilter.All(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<ushort> elements) => bloomFilter.AllAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static int[] ComputeHash(this IBloomFilter bloomFilter, ushort data) => bloomFilter.ComputeHash(BitConverter.GetBytes(data));
        #endregion UInt16

        #region UInt32

        public static void Add(this IBloomFilter bloomFilter, uint data) => bloomFilter.Add(BitConverter.GetBytes(data));

        public static Task AddAsync(this IBloomFilter bloomFilter, uint data) => bloomFilter.AddAsync(BitConverter.GetBytes(data));

        public static bool Contains(this IBloomFilter bloomFilter, uint data) => bloomFilter.Contains(BitConverter.GetBytes(data));

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, uint data) => bloomFilter.ContainsAsync(BitConverter.GetBytes(data));

        public static void Add(this IBloomFilter bloomFilter, IEnumerable<uint> elements) => bloomFilter.Add(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task AddAsync(this IBloomFilter bloomFilter, IEnumerable<uint> elements) => bloomFilter.AddAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static ICollection<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<uint> elements) => bloomFilter.Contains(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<ICollection<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<uint> elements) => bloomFilter.ContainsAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<uint> elements) => bloomFilter.All(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<uint> elements) => bloomFilter.AllAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static int[] ComputeHash(this IBloomFilter bloomFilter, uint data) => bloomFilter.ComputeHash(BitConverter.GetBytes(data));
        #endregion UInt32

        #region UInt64

        public static void Add(this IBloomFilter bloomFilter, ulong data) => bloomFilter.Add(BitConverter.GetBytes(data));

        public static Task AddAsync(this IBloomFilter bloomFilter, ulong data) => bloomFilter.AddAsync(BitConverter.GetBytes(data));

        public static bool Contains(this IBloomFilter bloomFilter, ulong data) => bloomFilter.Contains(BitConverter.GetBytes(data));

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, ulong data) => bloomFilter.ContainsAsync(BitConverter.GetBytes(data));

        public static void Add(this IBloomFilter bloomFilter, IEnumerable<ulong> elements) => bloomFilter.Add(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task AddAsync(this IBloomFilter bloomFilter, IEnumerable<ulong> elements) => bloomFilter.AddAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static ICollection<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<ulong> elements) => bloomFilter.Contains(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<ICollection<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<ulong> elements) => bloomFilter.ContainsAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<ulong> elements) => bloomFilter.All(elements.Select(data => BitConverter.GetBytes(data)));

        public static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<ulong> elements) => bloomFilter.AllAsync(elements.Select(data => BitConverter.GetBytes(data)));

        public static int[] ComputeHash(this IBloomFilter bloomFilter, ulong data) => bloomFilter.ComputeHash(BitConverter.GetBytes(data));
        #endregion UInt64

        #region DateTime

        public static void Add(this IBloomFilter bloomFilter, DateTime data) => bloomFilter.Add(BitConverter.GetBytes(data.Ticks));

        public static Task AddAsync(this IBloomFilter bloomFilter, DateTime data) => bloomFilter.AddAsync(BitConverter.GetBytes(data.Ticks));

        public static bool Contains(this IBloomFilter bloomFilter, DateTime data) => bloomFilter.Contains(BitConverter.GetBytes(data.Ticks));

        public static Task<bool> ContainsAsync(this IBloomFilter bloomFilter, DateTime data) => bloomFilter.ContainsAsync(BitConverter.GetBytes(data.Ticks));

        public static void Add(this IBloomFilter bloomFilter, IEnumerable<DateTime> elements) => bloomFilter.Add(elements.Select(data => BitConverter.GetBytes(data.Ticks)));

        public static Task AddAsync(this IBloomFilter bloomFilter, IEnumerable<DateTime> elements) => bloomFilter.AddAsync(elements.Select(data => BitConverter.GetBytes(data.Ticks)));

        public static ICollection<bool> Contains(this IBloomFilter bloomFilter, IEnumerable<DateTime> elements) => bloomFilter.Contains(elements.Select(data => BitConverter.GetBytes(data.Ticks)));

        public static Task<ICollection<bool>> ContainsAsync(this IBloomFilter bloomFilter, IEnumerable<DateTime> elements) => bloomFilter.ContainsAsync(elements.Select(data => BitConverter.GetBytes(data.Ticks)));

        public static bool All(this IBloomFilter bloomFilter, IEnumerable<DateTime> elements) => bloomFilter.All(elements.Select(data => BitConverter.GetBytes(data.Ticks)));

        public static Task<bool> AllAsync(this IBloomFilter bloomFilter, IEnumerable<DateTime> elements) => bloomFilter.AllAsync(elements.Select(data => BitConverter.GetBytes(data.Ticks)));

        public static int[] ComputeHash(this IBloomFilter bloomFilter, DateTime data) => bloomFilter.ComputeHash(BitConverter.GetBytes(data.Ticks));
        #endregion DateTime

        #region Object
        public static void Add<T>(this IBloomFilter bloomFilter, T data) => bloomFilter.Add(Convert.ToString(data, CultureInfo.InvariantCulture));
        public static Task AddAsync<T>(this IBloomFilter bloomFilter, T data) => bloomFilter.AddAsync(Convert.ToString(data, CultureInfo.InvariantCulture));
        public static bool Contains<T>(this IBloomFilter bloomFilter, T data) => bloomFilter.Contains(Convert.ToString(data, CultureInfo.InvariantCulture));
        public static Task<bool> ContainsAsync<T>(this IBloomFilter bloomFilter, T data) => bloomFilter.ContainsAsync(Convert.ToString(data, CultureInfo.InvariantCulture));
        public static void Add<T>(this IBloomFilter bloomFilter, IEnumerable<T> elements) => bloomFilter.Add(elements.Select(data => Encoding.UTF8.GetBytes(Convert.ToString(data, CultureInfo.InvariantCulture))));
        public static Task AddAsync<T>(this IBloomFilter bloomFilter, IEnumerable<T> elements) => bloomFilter.AddAsync(elements.Select(data => Encoding.UTF8.GetBytes(Convert.ToString(data, CultureInfo.InvariantCulture))));
        public static ICollection<bool> Contains<T>(this IBloomFilter bloomFilter, IEnumerable<T> elements) => bloomFilter.Contains(elements.Select(data => Encoding.UTF8.GetBytes(Convert.ToString(data, CultureInfo.InvariantCulture))));
        public static Task<ICollection<bool>> ContainsAsync<T>(this IBloomFilter bloomFilter, IEnumerable<T> elements) => bloomFilter.ContainsAsync(elements.Select(data => Encoding.UTF8.GetBytes(Convert.ToString(data, CultureInfo.InvariantCulture))));
        public static bool All<T>(this IBloomFilter bloomFilter, IEnumerable<T> elements) => bloomFilter.All(elements.Select(data => Encoding.UTF8.GetBytes(Convert.ToString(data, CultureInfo.InvariantCulture))));
        public static Task<bool> AllAsync<T>(this IBloomFilter bloomFilter, IEnumerable<T> elements) => bloomFilter.AllAsync(elements.Select(data => Encoding.UTF8.GetBytes(Convert.ToString(data, CultureInfo.InvariantCulture))));
        public static int[] ComputeHash<T>(this IBloomFilter bloomFilter, T data) => bloomFilter.ComputeHash(Encoding.UTF8.GetBytes(data.ToString()));
        #endregion Object
    }
}