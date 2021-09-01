using System;

namespace M5.BloomFilter
{
    public static class HashFunctionExtensions
    {
        public static HashMethod ToHashMethod(this HashFunction hashFunction)
        {
            var hashMethod = HashMethod.Unknown;

            foreach (var f in HashFunction.Functions)
            {
                if (f.Value == hashFunction)
                {
                    hashMethod = f.Key;
                    break;
                }
            }

            return hashMethod;
        }
    }
}
