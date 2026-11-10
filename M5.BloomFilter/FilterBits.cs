using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Unicode;
using System.Threading;

namespace M5.BloomFilter
{
    /// <summary>
    /// Bit storage shared by the built-in filters. Bit i lives in word i / 32 at
    /// bit i % 32, the layout BitArray uses, so exports and persisted bytes are
    /// unchanged from 2.0. Writers and exports take the lock; lookups do not.
    /// </summary>
    internal sealed class FilterBits
    {
        private const int StackPositions = 128;
        private const int StackChars = 85;
        private const int StackBytes = StackChars * 3;

        private readonly Lock _sync = new();
        private readonly int[] _words;
        private readonly HashFunction _hash;
        private readonly int _m;
        private readonly int _k;
        private readonly bool _doubleHash;

        // Writers store here after changing words and readers load it before
        // reading them: a paired volatile write and read on one location, so a
        // lookup that starts after an insertion returned sees its bits.
        private int _published;

        public FilterBits(int m, int k, HashFunction hash)
            : this(new int[WordCount(m)], m, k, hash)
        {
        }

        public FilterBits(BitArray bits, int m, int k, HashFunction hash)
            : this(Import(bits, m), m, k, hash)
        {
        }

        /// <summary>Takes ownership of <paramref name="words"/>.</summary>
        public FilterBits(int[] words, int m, int k, HashFunction hash)
        {
            _words = words;
            _hash = hash;
            _m = m;
            _k = k;
            Span<int> probe = stackalloc int[1];
            SpanHash = hash.TryComputeHash(ReadOnlySpan<byte>.Empty, 1, probe);
            _doubleHash = hash.GetType() == typeof(Murmur3KirschMitzenmacher);
        }

        /// <summary>True when the strategy can hash a span without allocating.</summary>
        public bool SpanHash { get; }

        private static int WordCount(int m) => (int)(((long)m + 31) >> 5);

        private static int[] Import(BitArray bits, int m)
        {
            var words = new int[WordCount(m)];
            bits.CopyTo(words, 0);
            ClearUnusedBits(words, m);
            return words;
        }

        private static void ClearUnusedBits(int[] words, int m)
        {
            int used = m & 31;
            if (used != 0 && words.Length > 0)
                words[^1] &= (int)((1u << used) - 1);
        }

        public bool Contains(byte[] element)
        {
            ArgumentNullException.ThrowIfNull(element, "data");
            if (SpanHash) return Contains(element.AsSpan());
            return Test(_hash.ComputeHash(element, _m, _k));
        }

        // The stack buffers below are fully written before they are read, so the
        // methods that use them skip zero-initialisation.
        [SkipLocalsInit]
        public bool Contains(ReadOnlySpan<byte> key)
        {
            if (_doubleHash) return ContainsDoubleHashed(key);
            if (!SpanHash) return Test(_hash.ComputeHash(key.ToArray(), _m, _k));
            Span<int> positions = _k <= StackPositions ? stackalloc int[_k] : new int[_k];
            _hash.TryComputeHash(key, _m, positions);
            return Test(positions);
        }

        // The default strategy tests each position as it is produced and stops at
        // the first clear bit, without writing the positions anywhere.
        private bool ContainsDoubleHashed(ReadOnlySpan<byte> key)
        {
            int m = _m;
            Murmur3KirschMitzenmacher.First(key, m, out int position, out uint step);
            Volatile.Read(ref _published);
            int[] words = _words;
            for (int i = 0; i < _k; i++)
            {
                if ((words[position >> 5] & (1 << (position & 31))) == 0) return false;
                position = Murmur3KirschMitzenmacher.Next(position, step, m);
            }

            return true;
        }

        [SkipLocalsInit]
        public bool ContainsUtf8(ReadOnlySpan<char> key)
        {
            if (key.Length <= StackChars)
            {
                Span<byte> buffer = stackalloc byte[StackBytes];
                return Contains(buffer[..Encode(key, buffer)]);
            }

            // Long keys are sized exactly; a guess of three bytes per unit could
            // overflow for very large inputs.
            byte[] rented = ArrayPool<byte>.Shared.Rent(Encoding.UTF8.GetByteCount(key));
            try
            {
                return Contains(rented.AsSpan(0, Encoding.UTF8.GetBytes(key, rented)));
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(rented, clearArray: true);
            }
        }

        // UTF-8 with U+FFFD for unpaired surrogates: the bytes Encoding.UTF8
        // produces for a string. Only used with a buffer of three bytes per
        // UTF-16 unit, which is always enough, so the conversion must complete.
        private static int Encode(ReadOnlySpan<char> key, Span<byte> buffer)
        {
            if (Utf8.FromUtf16(key, buffer, out _, out int written) != OperationStatus.Done)
                throw new InvalidOperationException("The key could not be encoded as UTF-8.");
            return written;
        }

        public void Add(byte[] element)
        {
            ArgumentNullException.ThrowIfNull(element, "data");
            if (SpanHash) Add(element.AsSpan());
            else Set(_hash.ComputeHash(element, _m, _k));
        }

        [SkipLocalsInit]
        public void Add(ReadOnlySpan<byte> key)
        {
            if (!SpanHash)
            {
                Set(_hash.ComputeHash(key.ToArray(), _m, _k));
                return;
            }

            Span<int> positions = _k <= StackPositions ? stackalloc int[_k] : new int[_k];
            _hash.TryComputeHash(key, _m, positions);
            Set(positions);
        }

        [SkipLocalsInit]
        public void AddUtf8(ReadOnlySpan<char> key)
        {
            if (key.Length <= StackChars)
            {
                Span<byte> buffer = stackalloc byte[StackBytes];
                Add(buffer[..Encode(key, buffer)]);
                return;
            }

            byte[] rented = ArrayPool<byte>.Shared.Rent(Encoding.UTF8.GetByteCount(key));
            try
            {
                Add(rented.AsSpan(0, Encoding.UTF8.GetBytes(key, rented)));
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(rented, clearArray: true);
            }
        }

        /// <summary>Lock-free. Sees every insertion that completed before the call.</summary>
        public bool Test(ReadOnlySpan<int> positions)
        {
            // Pairs with the volatile write every writer makes before leaving the lock.
            Volatile.Read(ref _published);
            int[] words = _words;
            foreach (int position in positions)
            {
                if ((uint)position >= (uint)_m) throw OutOfRange(position);
                if ((words[position >> 5] & (1 << (position & 31))) == 0)
                    return false;
            }

            return true;
        }

        // Only a custom HashFunction can produce a position outside [0, m).
        private static ArgumentOutOfRangeException OutOfRange(int position)
            => new ArgumentOutOfRangeException(nameof(position), position, "Hash position is outside the filter.");

        public void Set(ReadOnlySpan<int> positions)
        {
            // Validate first, and notice when there is nothing to write: adding a
            // key whose bits are all set changes nothing, so it needs no lock.
            Volatile.Read(ref _published);
            int[] words = _words;
            int allSet = 1;
            foreach (int position in positions)
            {
                if ((uint)position >= (uint)_m) throw OutOfRange(position);
                allSet &= words[position >> 5] >> (position & 31);
            }

            if ((allSet & 1) != 0) return;

            lock (_sync)
            {
                foreach (int position in positions)
                    _words[position >> 5] |= 1 << (position & 31);
                Volatile.Write(ref _published, _published + 1);
            }
        }

        public void Set(List<int> positions) => Set(CollectionsMarshal.AsSpan(positions));

        /// <summary>One result per complete group of k positions, in order.</summary>
        public List<bool> TestGroups(List<int> positions)
        {
            ReadOnlySpan<int> all = CollectionsMarshal.AsSpan(positions);
            // Every position is checked before any answer is produced, so an
            // invalid one always fails the batch, as it did in 2.0.
            foreach (int position in all)
            {
                if ((uint)position >= (uint)_m) throw OutOfRange(position);
            }

            var results = new List<bool>(all.Length / _k);
            for (int start = 0; start + _k <= all.Length; start += _k)
                results.Add(Test(all.Slice(start, _k)));
            return results;
        }

        /// <summary>ORs another filter's words in as one write.</summary>
        public void Or(int[] words)
        {
            lock (_sync)
            {
                for (int i = 0; i < _words.Length; i++)
                    _words[i] |= words[i];
                Volatile.Write(ref _published, _published + 1);
            }
        }

        /// <summary>A consistent copy: the state after some whole number of writes.</summary>
        public int[] CopyWords()
        {
            lock (_sync)
            {
                return (int[])_words.Clone();
            }
        }

        public static int[] CopyWords(BitArray bits, int m) => Import(bits, m);

        public FilterBits Snapshot() => new FilterBits(CopyWords(), _m, _k, _hash);

        public BitArray Export()
        {
            // new BitArray(int[]) rejects the word count needed above
            // 2,147,483,616 bits, so size by logical length and fill the bytes.
            var bits = new BitArray(_m);
            Span<byte> target = CollectionsMarshal.AsBytes(bits);
            lock (_sync)
            {
                if (BitConverter.IsLittleEndian)
                {
                    MemoryMarshal.AsBytes(_words.AsSpan())[..target.Length].CopyTo(target);
                }
                else
                {
                    Span<byte> word = stackalloc byte[4];
                    for (int i = 0; i < _words.Length; i++)
                    {
                        BinaryPrimitives.WriteInt32LittleEndian(word, _words[i]);
                        int offset = i * 4;
                        word[..Math.Min(4, target.Length - offset)].CopyTo(target[offset..]);
                    }
                }
            }

            return bits;
        }

        public int CountSetBits()
        {
            Volatile.Read(ref _published);
            long count = 0;
            foreach (int word in _words)
                count += BitOperations.PopCount((uint)word);
            return (int)count;
        }
    }
}
