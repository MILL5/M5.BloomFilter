# Serialization and version 2 migration

Version 2.1.0 does not change hashing, key encoding or any persisted format.
Snapshots written by 2.0.x load in 2.1.0 and the reverse; the test suite checks
both directions against fixtures written by the published 2.0.1 packages. A
union made with `Union` or `UnionWith` serializes like any other filter.

A saved filter needs its bits, hash identity, sizing metadata and hash format.
Serializing bits alone does not preserve query behavior. Companion packages are
`M5.BloomFilter.Serialization.Json` and `M5.BloomFilter.Serialization.ProtoBuf`;
standalone bit-array packages are also available.

## System.Text.Json

```csharp
using M5.BloomFilter;
using System.Text.Json;

var filter = FilterBuilder.Build(1000);
filter.Add("saved");
IImmutableBloomFilter snapshot = filter.ToImmutable();
var options = new JsonSerializerOptions();
options.Converters.Add(new System.Text.Json.Serialization.BloomFilterConverter());
var json = JsonSerializer.Serialize(snapshot, options);
var restored = JsonSerializer.Deserialize<IImmutableBloomFilter>(json, options);
```

The nested bit-array converter is registered by property annotations. Concrete
ReadOnlyFilter and interface IImmutableBloomFilter both work. Options are not
mutated by the converter. A top-level null round-trips as null.

## Newtonsoft.Json

```csharp
var converter = new Newtonsoft.Json.BloomFilterConverter();
var json = Newtonsoft.Json.JsonConvert.SerializeObject(snapshot, converter);
var restored = Newtonsoft.Json.JsonConvert.DeserializeObject<IImmutableBloomFilter>(json, converter);
```

The core package also contains `M5.BloomFilter.BloomFilterJsonConverter` for use
without the companion serializer. It stores hb as raw base64 bytes rather than
Brotli-compressing them. Select the matching writer and reader; these two JSON
bit representations are not interchangeable above the compression threshold.

## Protocol Buffers

```csharp
using ProtoBuf;
using var stream = new MemoryStream();
Serializer.Serialize(stream, new M5.BloomFilter.Serialization.BloomFilter(snapshot));
stream.Position = 0;
var payload = Serializer.Deserialize<M5.BloomFilter.Serialization.BloomFilter>(stream);
var restored = payload.AsBloomFilter();
```

## Wire contract and validation

JSON fields: hb (bits), hm (HashMethod number), m (bit capacity), k (hash count),
n (expected keys), p (calculated estimate), v (format version, exactly 2).
The hb object contains b (base64 bytes) and l (logical bit length). Bit i is stored
at byte i/8, bit i%8. Companion serializers use Brotli for l >=257; smaller arrays
are raw. Protobuf retains tags 1 through 6 and adds format version at tag 7.

Readers reject negative lengths, mismatched m and bit length, wrong byte counts,
truncated or extra compressed input, and unsupported hash format versions. Decoded
bytes must exactly equal ceil(l/8); empty standalone bit arrays use zero bytes.
These structural checks are not authentication or a checksum: arbitrary bit
corruption that preserves shape cannot always be detected. Use authenticated
storage/transport when integrity matters.

## Upgrade safely

1. Keep original keys; bit arrays cannot recover them.
2. Upgrade all writers and readers to version 2.0.0 and .NET 10.
3. Rebuild each filter from its original keys and persist a new snapshot.
4. Verify membership of all source keys before switching readers.
5. Keep legacy and v2 stores separate during cutover and rollback.

Missing v, v=0, v=1, and unknown future values are rejected with a rebuild message.
Changing only the version number creates false negatives. Old readers may ignore
v, so the field does not protect consumers running old code.

The direct ReadOnlyFilter(bits, method, m, n, k, p) constructor assumes current
format. When loading persisted metadata, use the overload with formatVersion.
Generic scalar calls Add<T>(value), Contains<T>(value) and ComputeHash<T>(value)
become non-generic calls; IEnumerable<T> batch overloads remain generic.
