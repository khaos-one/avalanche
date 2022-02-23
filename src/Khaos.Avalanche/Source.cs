namespace Khaos.Avalanche;

public abstract class Source<T> : ISource<T>
{
    protected abstract IAsyncEnumerable<T> GetEnumerable(CancellationToken cancellationToken = default);

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return GetEnumerable(cancellationToken).GetAsyncEnumerator(cancellationToken);
    }
}