namespace Khaos.Avalanche.Watchers;

public class SimpleCounter<T> : IWatcher<T>, ISimpleCounter
{
    public string Name { get; }
    public ulong Value { get; private set; }

    public SimpleCounter(string name)
    {
        Name = name;
    }

    public IAsyncEnumerable<T> Expand(IAsyncEnumerable<T> enumerable) =>
        enumerable.Select(
            element =>
            {
                Value++;

                return element;
            });
}