// Builds the snapshot the GitHub Page queries: a Bloom filter over the ENABLE
// word list, written by this library in its core JSON format.
// Usage: dotnet run -c Release --project tools/SiteFilter -- <enable1.txt> <words.json>
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using M5.BloomFilter;

const string ExpectedSha256 = "3f16130220645692ed49c7134e24a18504c2ca55b3c012f7290e3e77c63b1a89";
const int ExpectedWords = 172_823;
const double TargetErrorRate = 0.01;
const int BitsPerCard = 80 * 12;

if (args.Length != 2)
{
    Console.Error.WriteLine("usage: SiteFilter <enable1.txt> <words.json>");
    return 2;
}

byte[] source = File.ReadAllBytes(args[0]);
string sha256 = Convert.ToHexString(SHA256.HashData(source)).ToLowerInvariant();
if (sha256 != ExpectedSha256)
{
    Console.Error.WriteLine($"Word list SHA-256 is {sha256}; expected {ExpectedSha256}. See README.md for the source.");
    return 1;
}

string[] words = Encoding.ASCII.GetString(source)
    .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    .Distinct(StringComparer.Ordinal)
    .ToArray();
if (words.Length != ExpectedWords || words.Any(word => word.Any(letter => letter < 'a' || letter > 'z')))
{
    Console.Error.WriteLine($"Expected {ExpectedWords} distinct lower-case entries; found {words.Length}.");
    return 1;
}

var filter = FilterBuilder.Build(words.Length, TargetErrorRate);
foreach (string word in words) filter.Add(word);
var snapshot = filter.ToImmutable();

int missing = words.Count(word => !snapshot.Contains(word));
if (missing != 0)
{
    Console.Error.WriteLine($"{missing} list entries test absent. Refusing to write a snapshot with false negatives.");
    return 1;
}

string json = Newtonsoft.Json.JsonConvert.SerializeObject(snapshot, new BloomFilterJsonConverter());
File.WriteAllText(args[1], json, new UTF8Encoding(false));

var statistics = snapshot.Statistics;
var occupancy = snapshot.GetOccupancy();
var invariant = CultureInfo.InvariantCulture;
Console.WriteLine($"words            {words.Length.ToString("N0", invariant)}");
Console.WriteLine($"list bytes       {source.Length.ToString("N0", invariant)}");
Console.WriteLine($"m (bits)         {statistics.M.ToString("N0", invariant)}");
Console.WriteLine($"k (hashes)       {statistics.K}");
Console.WriteLine($"design p         {statistics.P.ToString("0.000000", invariant)}");
Console.WriteLine($"filter bytes     {((statistics.M + 7) / 8).ToString("N0", invariant)}");
Console.WriteLine($"json bytes       {new FileInfo(args[1]).Length.ToString("N0", invariant)}");
Console.WriteLine($"cards (960 bits) {((statistics.M + BitsPerCard - 1) / BitsPerCard).ToString("N0", invariant)}");
Console.WriteLine($"set bits         {occupancy.SetBits.ToString("N0", invariant)}");
Console.WriteLine($"fill ratio       {occupancy.FillRatio.ToString("0.000000", invariant)}");
Console.WriteLine($"estimated p      {occupancy.EstimatedFalsePositiveRate.ToString("0.000000", invariant)}");
Console.WriteLine($"estimated count  {occupancy.EstimatedCount.ToString("N0", invariant)}");

// A fixed sample of strings that are not list entries (they contain a colon).
const int Probes = 1_000_000;
int falsePositives = 0;
for (int i = 0; i < Probes; i++)
    if (snapshot.Contains("not-a-word:" + i.ToString(invariant))) falsePositives++;
Console.WriteLine($"false positives  {falsePositives.ToString("N0", invariant)} of {Probes.ToString("N0", invariant)} fixed non-words");

// Vectors asserted by SiteVectorTests and by the page's start-up self-check.
Console.WriteLine("vectors (word -> positions):");
foreach (string word in new[] { "bloom", "filter", "zyzzyva", "", "naïve", "\U0001F600", "definitelynotaword" })
    Console.WriteLine($"  \"{word}\" {(snapshot.Contains(word) ? "present" : "absent")} [{string.Join(",", snapshot.ComputeHash(word))}]");
// Sizing vectors for the page's calculator, a port of Statistics(long, double).
Console.WriteLine("sizes (n, p -> m, k):");
foreach (var (n, p) in new (long, double)[]
{
    (1, 0.5), (1000, 0.01), (12345, 0.05), (172823, 0.01), (1000000, 0.1), (1000000, 0.01), (1000000, 0.001),
    (1000000, 0.0001), (1000000, 0.000001), (50000000, 0.0001), (223000000, 0.01), (2147483647, 0.1), (250000000, 0.000001),
})
{
    try
    {
        var sized = new Statistics(n, p);
        Console.WriteLine($"  {{ n: {n}, p: {p.ToString("R", invariant)}, m: {sized.M}, k: {sized.K} }},");
    }
    catch (ArgumentOutOfRangeException)
    {
        Console.WriteLine($"  {{ n: {n}, p: {p.ToString("R", invariant)}, m: null, k: null }},");
    }
}
return 0;
