namespace Khaos.Avalanche.ConsoleMonitor.SimpleCounter;

public class StreamMonitor<TIn> : ITransform<TIn, TIn>
{
    private readonly SimpleCounter _counter;

    public StreamMonitor(SimpleCounter counter)
    {
        _counter = counter;
    }

    public IAsyncEnumerable<TIn> Expand(IAsyncEnumerable<TIn> enumerable) =>
        enumerable
            .Select(
                element =>
                {
                    _counter.Increment();

                    return element;
                });
}