namespace M5.BloomFilter
{
    public interface IChecksum
    {
        void Reset();

        long Value { get; }

        void Update(int b);

        void Update(byte[] buffer);

        void Update(byte[] buffer, int offset, int count);
    }
}