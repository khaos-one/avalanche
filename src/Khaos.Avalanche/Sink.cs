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
        await Begin(cancellationToken);
        
        await foreach (var element in Enumerable!.WithCancellation(cancellationToken))
        {
            await SinkElement(element, cancellationToken);
        }

        await End(cancellationToken);
    }

    protected virtual Task Begin(CancellationToken cancellationToken) => Task.CompletedTask;
    protected abstract Task SinkElement(T element, CancellationToken cancellationToken);
    protected virtual Task End(CancellationToken cancellationToken) => Task.CompletedTask;
}