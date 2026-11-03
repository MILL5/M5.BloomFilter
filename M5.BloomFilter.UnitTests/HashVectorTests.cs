using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace M5.BloomFilter.UnitTests
{
    [TestClass]
    public class HashVectorTests
    {
        [DataRow(HashMethod.LCGWithFNV1, "314718,327949,929405,851295,568,964672,628941,935034")]
        [DataRow(HashMethod.LCGWithFNV1a, "429467,476120,757323,743815,476005,484215,170757,587924")]
        [DataRow(HashMethod.LCGModifiedFNV1, "341896,10725,195610,17676,565299,358423,792694,779861")]
        [DataRow(HashMethod.RNGWithFNV1, "136177,418085,372828,608236,467434,645151,604664,122691")]
        [DataRow(HashMethod.RNGWithFNV1a, "383128,654834,940045,325068,259864,400010,982979,583573")]
        [DataRow(HashMethod.RNGModifiedFNV1, "292450,726015,458109,742888,815844,33199,101072,247047")]
        [DataRow(HashMethod.CRC32, "29135,724244,993403,843757,201671,278496,300036,276656")]
        [DataRow(HashMethod.CRC32u, "610073,642308,876381,122785,533857,552,65130,720346")]
        [DataRow(HashMethod.Adler32, "661063,634172,758356,9665,242791,118250,87775,484881")]
        [DataRow(HashMethod.Murmur2, "121489,641483,296559,928681,285779,864584,744686,491166")]
        [DataRow(HashMethod.Murmur3, "469533,273216,546912,93346,412889,731790,276187,163827")]
        [DataRow(HashMethod.Murmur3KirschMitzenmacher, "469533,357959,246385,134811,23237,911666,800092,688518")]
        [DataRow(HashMethod.SHA1, "543088,329227,415290,392655,46926,223895,764892,520349")]
        [DataRow(HashMethod.SHA256, "364431,98891,686650,151527,259490,988965,595586,836772")]
        [DataRow(HashMethod.SHA384, "652656,558310,332550,977693,422436,270813,107018,614297")]
        [DataRow(HashMethod.SHA512, "841962,235158,575301,3888,417159,973027,318279,457586")]
        [TestMethod]
        public void V2PositionsAreFrozen(HashMethod method, string expected)
        {
            var actual = HashFunction.Functions[method].ComputeHash(Encoding.UTF8.GetBytes("Bloom v2 ☃"), 1000003, 8);
            CollectionAssert.AreEqual(expected.Split(',').Select(int.Parse).ToArray(), actual);
        }

        [TestMethod]
        public void EveryMethodValidatesAndPreservesInputs()
        {
            foreach (var function in HashFunction.Functions.Values)
            {
                Assert.ThrowsExactly<ArgumentNullException>(() => function.ComputeHash(null, 10, 3));
                Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => function.ComputeHash(new byte[0], 0, 3));
                Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => function.ComputeHash(new byte[0], -1, 3));
                Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => function.ComputeHash(new byte[0], 10, 0));
                Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => function.ComputeHash(new byte[0], 10, -1));
                foreach (var input in new[] { Array.Empty<byte>(), new byte[] { 0 }, new byte[] { 255, 0, 127, 128, 1 } })
                {
                    var original = (byte[])input.Clone();
                    var positions = function.ComputeHash(input, int.MaxValue, 1024);
                    Assert.AreEqual(1024, positions.Length);
                    Assert.IsTrue(positions.All(p => p >= 0 && p < int.MaxValue));
                    CollectionAssert.AreEqual(positions, function.ComputeHash(input, int.MaxValue, 1024));
                    CollectionAssert.AreEqual(original, input);
                    Assert.IsTrue(function.ComputeHash(input, 1, 1024).All(p => p == 0));
                }
            }
        }

        [TestMethod]
        public void LcgUsesBitsBeyondOriginalBrokenRange()
        {
            foreach (var method in new[] { HashMethod.LCGWithFNV1, HashMethod.LCGWithFNV1a, HashMethod.LCGModifiedFNV1 })
                Assert.IsTrue(HashFunction.Functions[method].ComputeHash(Encoding.UTF8.GetBytes("Bloom v2 ☃"), 1000003, 8).Any(p => p > 16383));
        }

        [TestMethod]
        public void MurmurCoresMatchReferenceVectors()
        {
            Assert.AreEqual(0U, Murmur2.MurmurHash2(0, Array.Empty<byte>()));
            Assert.AreEqual(0xe56129cbU, Murmur2.MurmurHash2(0, Encoding.ASCII.GetBytes("hello")));
            Assert.AreEqual(0U, MurmurProbe.Hash(0, ""));
            Assert.AreEqual(0xf6a5c420U, MurmurProbe.Hash(0, "foo"));
            Assert.AreEqual(0x248bfa47U, MurmurProbe.Hash(0, "hello"));
        }

        [TestMethod]
        public void HashRegistryCannotBeMutated()
        {
            var registry = (IDictionary<HashMethod, HashFunction>)HashFunction.Functions;
            Assert.ThrowsExactly<NotSupportedException>(() => registry.Remove(HashMethod.SHA1));
        }

        private sealed class MurmurProbe : Murmur3
        {
            public static uint Hash(uint seed, string value)
            {
                var bytes = Encoding.ASCII.GetBytes(value);
                return MurmurHash3_32(seed, bytes, 0, bytes.Length);
            }
        }
    }
}
