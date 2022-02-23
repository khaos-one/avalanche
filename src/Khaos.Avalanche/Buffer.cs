namespace Khaos.Avalanche;

public class Buffer<T> : ITransform<T, T>
{
    private readonly int _bufferSize;

    public Buffer(int bufferSize)
    {
        _bufferSize = bufferSize;
    }

    public IAsyncEnumerable<T> Expand(IAsyncEnumerable<T> enumerable) =>
        enumerable
            .Batch(_bufferSize)
            .Flatten();
}