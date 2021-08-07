using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Shouldly;

namespace M5.BloomFilter.UnitTests
{
    [TestClass]
    public class CalculationTests
    {
        [TestMethod]
        public void LargeCacheCalculateSizeTest()
        {
            const long N_ = 7000000;
            const double P_ = 0.01;
            var s = new Statistics(N_, P_);
            Console.WriteLine($"M={s.M}");
            Console.WriteLine($"K={s.K}");
            Console.WriteLine($"N={s.N}");
            Console.WriteLine($"P={s.P}");
            var difference = Math.Abs(s.P - P_);
            difference.ShouldBeLessThan(0.005);

            s.N.ShouldBe(N_);
        }

        [TestMethod]
        public void SmallCacheCalculateSizeTest()
        {
            const long N_ = 100;
            const double P_ = 0.1;

            var s = new Statistics(N_, P_);

            Console.WriteLine($"M={s.M}");
            Console.WriteLine($"K={s.K}");
            Console.WriteLine($"N={s.N}");
            Console.WriteLine($"P={s.P}");
            var difference = Math.Abs(s.P - P_);
            difference.ShouldBeLessThan(0.005);

            s.N.ShouldBe(N_);
        }

        [TestMethod]
        public void LargeCacheLimitSizeTest()
        {
            const int M_ = 16000000;
            const long N_ = 7000000;
            var s = new Statistics(M_, N_);
            Console.WriteLine($"M={s.M}");
            Console.WriteLine($"K={s.K}");
            Console.WriteLine($"N={s.N}");
            Console.WriteLine($"P={s.P}");

            s.M.ShouldBe(M_);
            s.N.ShouldBe(N_);
        }

        [TestMethod]
        public void LargeCacheLimitSizeWithSmallKTest()
        {
            const int M_ = 16000000;
            const short K_ = 2;

            var s = new Statistics(M_, K_);

            Console.WriteLine($"M={s.M}");
            Console.WriteLine($"K={s.K}");
            Console.WriteLine($"N={s.N}");
            Console.WriteLine($"P={s.P}");

            s.M.ShouldBe(M_);
            s.K.ShouldBe(K_);
        }

        [TestMethod]
        public void LargeCacheLimitSizeWithMediumKTest()
        {
            const int M_ = 16000000;
            const short K_ = 7;

            var s = new Statistics(M_, K_);

            Console.WriteLine($"M={s.M}");
            Console.WriteLine($"K={s.K}");
            Console.WriteLine($"N={s.N}");
            Console.WriteLine($"P={s.P}");

            s.M.ShouldBe(M_);
            s.K.ShouldBe(K_);
        }

        [TestMethod]
        public void LargeCacheLimitSizeWithLargKTest()
        {
            const int M_ = 16000000;
            const short K_ = 13;

            var s = new Statistics(M_, K_);

            Console.WriteLine($"M={s.M}");
            Console.WriteLine($"K={s.K}");
            Console.WriteLine($"N={s.N}");
            Console.WriteLine($"P={s.P}");

            s.M.ShouldBe(M_);
            s.K.ShouldBe(K_);
        }
    }
}
