// Writes fixtures produced by the published M5.BloomFilter 2.0.1 packages.
// Later versions assert against these files, so the comparison never depends
// on the code under test. Usage: dotnet run -c Release -- <output directory>
//                                dotnet run -c Release -- --read <directory>
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using M5.BloomFilter;

static class Oracle
{
    static readonly int[] Lengths = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 15, 16, 17, 31, 32, 33, 255, 256, 257, 1024 };
    static readonly int[] Sizes = { 97, 1000003, int.MaxValue };

    public static byte[] Input(int length)
    {
        var data = new byte[length];
        for (int i = 0; i < length; i++) data[i] = (byte)((i * 131 + length * 17 + 7) & 0xFF);
        return data;
    }

    public static readonly string[] Strings =
    {
        "", "a", "customer:42", "Bloom v2 ☃", "東京", "\U0001F600", "\uD800", "a\uDC00b",
        new string('x', 300), string.Concat(Enumerable.Repeat("é\U0001F600", 90)),
    };

    static string Bits(BitArray bits)
    {
        var bytes = new byte[(bits.Length + 7) / 8];
        bits.CopyTo(bytes, 0);
        return bits.Length.ToString(CultureInfo.InvariantCulture) + ":" + Convert.ToHexString(bytes);
    }

    static string Join(int[] positions) => string.Join(",", positions);

    static IBloomFilter Populated(int expected, int keys)
    {
        var filter = FilterBuilder.Build(expected, 0.01);
        for (int i = 0; i < keys; i++) filter.Add("key:" + i.ToString(CultureInfo.InvariantCulture));
        return filter;
    }

    static int Main(string[] args)
    {
        if (args.Length == 2 && args[0] == "--read") return Read(args[1]);
        if (args.Length != 1) { Console.Error.WriteLine("usage: V201Oracle <output directory> | --read <directory>"); return 2; }
        Directory.CreateDirectory(args[0]);
        var utf8 = new UTF8Encoding(false);

        // 1. Raw hash positions for every strategy, covering every Murmur tail length.
        var lines = new List<string>();
        foreach (HashMethod method in Enum.GetValues<HashMethod>())
        {
            if (method == HashMethod.Unknown) continue;
            var function = HashFunction.Functions[method];
            foreach (int length in Lengths)
                foreach (int m in Sizes)
                    lines.Add($"{(int)method};{length};{m};{Join(function.ComputeHash(Input(length), m, 8))}");
        }
        File.WriteAllLines(Path.Combine(args[0], "positions.txt"), lines, utf8);

        // 2. Key encodings through the public extensions, default strategy.
        var probe = FilterBuilder.Build(1000, 0.01);
        var keys = new List<string>();
        for (int i = 0; i < Strings.Length; i++) keys.Add($"string;{i};{Join(probe.ComputeHash(Strings[i]))}");
        keys.Add($"byte;7;{Join(probe.ComputeHash((byte)7))}");
        keys.Add($"short;-2;{Join(probe.ComputeHash((short)-2))}");
        keys.Add($"ushort;65535;{Join(probe.ComputeHash((ushort)65535))}");
        keys.Add($"int;42;{Join(probe.ComputeHash(42))}");
        keys.Add($"int;-1;{Join(probe.ComputeHash(-1))}");
        keys.Add($"uint;4294967295;{Join(probe.ComputeHash(uint.MaxValue))}");
        keys.Add($"long;-9223372036854775808;{Join(probe.ComputeHash(long.MinValue))}");
        keys.Add($"ulong;18446744073709551615;{Join(probe.ComputeHash(ulong.MaxValue))}");
        keys.Add($"float;1.5;{Join(probe.ComputeHash(1.5f))}");
        keys.Add($"float;NaN;{Join(probe.ComputeHash(float.NaN))}");
        keys.Add($"double;-0;{Join(probe.ComputeHash(-0.0))}");
        keys.Add($"double;epsilon;{Join(probe.ComputeHash(double.Epsilon))}");
        keys.Add($"datetime;638447616000000000;{Join(probe.ComputeHash(new DateTime(638447616000000000L)))}");
        File.WriteAllLines(Path.Combine(args[0], "keys.txt"), keys, utf8);

        // 3. Persisted snapshots: one above the companion compression threshold
        //    (m = 9586, not a multiple of 32) and one below it (m = 96).
        foreach (var (name, expected, count) in new[] { ("large", 1000, 500), ("small", 10, 5) })
        {
            var snapshot = Populated(expected, count).ToImmutable();
            File.WriteAllText(Path.Combine(args[0], name + ".bits.txt"), Bits(snapshot.HashBits), utf8);
            File.WriteAllText(Path.Combine(args[0], name + ".core.json"),
                Newtonsoft.Json.JsonConvert.SerializeObject(snapshot, new BloomFilterJsonConverter()), utf8);
            File.WriteAllText(Path.Combine(args[0], name + ".newtonsoft.json"),
                Newtonsoft.Json.JsonConvert.SerializeObject(snapshot, new Newtonsoft.Json.BloomFilterConverter()), utf8);
            var options = new System.Text.Json.JsonSerializerOptions();
            options.Converters.Add(new System.Text.Json.Serialization.BloomFilterConverter());
            File.WriteAllText(Path.Combine(args[0], name + ".stj.json"),
                System.Text.Json.JsonSerializer.Serialize(snapshot, options), utf8);
            using var stream = new MemoryStream();
            ProtoBuf.Serializer.Serialize(stream, new M5.BloomFilter.Serialization.BloomFilter(snapshot));
            File.WriteAllText(Path.Combine(args[0], name + ".protobuf.b64"), Convert.ToBase64String(stream.ToArray()), utf8);
        }
        Console.WriteLine($"Wrote {lines.Count} position rows, {keys.Count} key rows and 2 snapshots with M5.BloomFilter 2.0.1.");
        return 0;
    }

    // Reads snapshots written by a newer version and checks them with 2.0.1 code.
    static int Read(string directory)
    {
        int failures = 0;
        foreach (var (name, count) in new[] { ("large", 500), ("small", 5) })
        {
            var options = new System.Text.Json.JsonSerializerOptions();
            options.Converters.Add(new System.Text.Json.Serialization.BloomFilterConverter());
            var loaded = new (string Format, IImmutableBloomFilter Filter)[]
            {
                ("core", Newtonsoft.Json.JsonConvert.DeserializeObject<IImmutableBloomFilter>(
                    File.ReadAllText(Path.Combine(directory, name + ".core.json")), new BloomFilterJsonConverter())),
                ("newtonsoft", Newtonsoft.Json.JsonConvert.DeserializeObject<IImmutableBloomFilter>(
                    File.ReadAllText(Path.Combine(directory, name + ".newtonsoft.json")), new Newtonsoft.Json.BloomFilterConverter())),
                ("stj", System.Text.Json.JsonSerializer.Deserialize<IImmutableBloomFilter>(
                    File.ReadAllText(Path.Combine(directory, name + ".stj.json")), options)),
                ("protobuf", ProtoBuf.Serializer.Deserialize<M5.BloomFilter.Serialization.BloomFilter>(
                    new MemoryStream(Convert.FromBase64String(File.ReadAllText(Path.Combine(directory, name + ".protobuf.b64"))))).AsBloomFilter()),
            };
            foreach (var (format, filter) in loaded)
            {
                int missing = Enumerable.Range(0, count).Count(i => !filter.Contains("key:" + i.ToString(CultureInfo.InvariantCulture)));
                Console.WriteLine($"{name}.{format}: m={filter.Statistics.M} k={filter.Statistics.K} missing={missing}");
                failures += missing;
            }
        }
        return failures == 0 ? 0 : 1;
    }
}
