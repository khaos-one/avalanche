namespace Khaos.Avalanche;

public abstract class Sink<T> : ISink<T>
{
    protected IAsyncEnumerable<T>? Enumerable;
    
    public void SetEnumerable(IAsyncEnumerable<T> enumerable)
    {
        Enumerable = enumerable;
    }

    public async Task Run(CancellationToken cancellationToken = default)
    {
        await foreach (var element in Enumerable!.WithCancellation(cancellationToken))
        {
            await SinkElement(element, cancellationToken);
        }
    }

    protected abstract Task SinkElement(T element, CancellationToken cancellationToken);
}