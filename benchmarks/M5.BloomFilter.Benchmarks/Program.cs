using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using M5.BloomFilter;

BenchmarkSwitcher.FromAssembly(typeof(LookupBenchmarks).Assembly).Run(args);

/// <summary>Single-key lookups on a filter holding one million keys at a 1% target.</summary>
[MemoryDiagnoser]
public class LookupBenchmarks
{
    private const int Keys = 1_000_000;
    private const int Mask = 1023;
    private IBloomFilter _filter;
    private IImmutableBloomFilter _snapshot;
    private string[] _present;
    private string[] _absent;
    private byte[][] _presentUtf8;
    private long[] _ids;
    private int _next;

    [GlobalSetup]
    public void Setup()
    {
        _filter = FilterBuilder.Build(Keys * 2, 0.01);
        for (int i = 0; i < Keys; i++) _filter.Add("user:" + i);
        for (long id = 0; id < Keys; id++) _filter.Add(id * 7919);
        _snapshot = _filter.ToImmutable();
        _present = Enumerable.Range(0, Mask + 1).Select(i => "user:" + (i * 977 % Keys)).ToArray();
        _absent = Enumerable.Range(0, Mask + 1).Select(i => "guest:" + i).ToArray();
        _presentUtf8 = _present.Select(Encoding.UTF8.GetBytes).ToArray();
        _ids = Enumerable.Range(0, Mask + 1).Select(i => (long)(i * 977 % Keys) * 7919).ToArray();
    }

    [Benchmark(Description = "Contains(string), present")]
    public bool ContainsStringHit() => _filter.Contains(_present[_next++ & Mask]);

    [Benchmark(Description = "Contains(string), absent")]
    public bool ContainsStringMiss() => _filter.Contains(_absent[_next++ & Mask]);

    [Benchmark(Description = "Contains(long), present")]
    public bool ContainsLongHit() => _filter.Contains(_ids[_next++ & Mask]);

    [Benchmark(Description = "Contains(byte[]), present")]
    public bool ContainsBytesHit() => _filter.Contains(_presentUtf8[_next++ & Mask]);

    [Benchmark(Description = "snapshot Contains(string), present")]
    public bool SnapshotContainsStringHit() => _snapshot.Contains(_present[_next++ & Mask]);

#if !BASELINE
    [Benchmark(Description = "ContainsKey(ReadOnlySpan<char>), present")]
    public bool ContainsKeyCharsHit() => _filter.ContainsKey(_present[_next++ & Mask].AsSpan(0));
#endif
}

/// <summary>
/// Insertions. "New keys" fills an empty filter sized for one million keys with
/// one million distinct keys per iteration. "Already present" adds keys the
/// filter already holds, which changes no bits.
/// </summary>
[MemoryDiagnoser]
[WarmupCount(6)]
[IterationCount(24)]
[InvocationCount(Keys)]
public class AddNewBenchmarks
{
    private const int Keys = 1_000_000;
    private IBloomFilter _filter;
    private string[] _keys;
    private int _next;

    [GlobalSetup]
    public void Setup()
    {
        _keys = Enumerable.Range(0, Keys).Select(i => "user:" + i).ToArray();
    }

    [IterationSetup]
    public void EmptyFilter()
    {
        _filter = FilterBuilder.Build(Keys, 0.01);
        _next = 0;
    }

    [Benchmark(Description = "Add(string), new keys")]
    public void AddString() => _filter.Add(_keys[_next++]);

    [Benchmark(Description = "Add(long), new keys")]
    public void AddLong() => _filter.Add((long)_next++ * 7919);
}

[MemoryDiagnoser]
public class AddPresentBenchmarks
{
    private const int Mask = 65535;
    private IBloomFilter _filter;
    private string[] _keys;
    private int _next;

    [GlobalSetup]
    public void Setup()
    {
        _filter = FilterBuilder.Build(1_000_000, 0.01);
        _keys = Enumerable.Range(0, Mask + 1).Select(i => "user:" + i).ToArray();
        foreach (var key in _keys) _filter.Add(key);
    }

    [Benchmark(Description = "Add(string), already present")]
    public void AddString() => _filter.Add(_keys[_next++ & Mask]);
}

/// <summary>Eight threads looking up present keys in one shared filter.</summary>
[MemoryDiagnoser]
public class ParallelLookupBenchmarks
{
    private const int Threads = 8;
    private const int PerThread = 250_000;
    private const int Mask = 1023;
    private IBloomFilter _filter;
    private string[] _present;

    [GlobalSetup]
    public void Setup()
    {
        _filter = FilterBuilder.Build(2_000_000, 0.01);
        for (int i = 0; i < 1_000_000; i++) _filter.Add("user:" + i);
        _present = Enumerable.Range(0, Mask + 1).Select(i => "user:" + (i * 977 % 1_000_000)).ToArray();
    }

    [Benchmark(Description = "Contains(string) on 8 threads", OperationsPerInvoke = Threads * PerThread)]
    public int ContainsFromEightThreads()
    {
        int found = 0;
        Parallel.For(0, Threads, new ParallelOptions { MaxDegreeOfParallelism = Threads }, thread =>
        {
            int local = 0;
            for (int i = 0; i < PerThread; i++)
                if (_filter.Contains(_present[(i + thread) & Mask])) local++;
            System.Threading.Interlocked.Add(ref found, local);
        });
        return found;
    }
}
