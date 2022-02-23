namespace Khaos.Avalanche;

public interface ISink<in T> : IPipeline
{
    void SetEnumerable(IAsyncEnumerable<T> enumerable);
}