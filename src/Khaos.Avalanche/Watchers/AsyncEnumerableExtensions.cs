namespace Khaos.Avalanche.Watchers;

public static class AsyncEnumerableExtensions
{
    public static IAsyncEnumerable<T> WithWatcher<T>(this IAsyncEnumerable<T> enumerable, IWatcher<T> watcher) =>
        enumerable.Apply(watcher);
}