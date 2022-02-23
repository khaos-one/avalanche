namespace Khaos.Avalanche;

public interface ITransform<in TIn, out TOut>
{
    IAsyncEnumerable<TOut> Expand(IAsyncEnumerable<TIn> enumerable);
}