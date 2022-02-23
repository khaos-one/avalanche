namespace Khaos.Avalanche;

public class FunctionalSink<T> : ISink<T>
{
    private readonly Func<T, Task> _action;
    private IAsyncEnumerable<T>? _enumerable;

    public FunctionalSink(Func<T, Task> action)
    {
        _action = action;
    }

    public void SetEnumerable(IAsyncEnumerable<T> enumerable)
    {
        _enumerable = enumerable;
    }

    public async Task Run(CancellationToken cancellationToken = default)
    {
        await foreach (var element in _enumerable!.WithCancellation(cancellationToken))
        {
            await _action(element);
        }
    }
}