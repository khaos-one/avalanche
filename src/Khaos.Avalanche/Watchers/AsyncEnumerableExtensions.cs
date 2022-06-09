namespace Khaos.Avalanche.Watchers;

public static class AsyncEnumerableExtensions
{
    public static IAsyncEnumerable<T> WithWatcher<T>(this IAsyncEnumerable<T> enumerable, IWatcher<T> watcher) =>
        enumerable.Apply(watcher);

    public static IAsyncEnumerable<T> WithSimpleCounter<T>(this IAsyncEnumerable<T> enumerable, string name, out IWatcher<T> watcher)
    {
        watcher = new SimpleCounter<T>(name);
        return enumerable.Apply(watcher);
    }

    public static IAsyncEnumerable<T> WithSimpleCounter<T>(
        this IAsyncEnumerable<T> enumerable,
        string name,
        IList<IWatcher> watchersListToAdd)
    {
        IWatcher<T> watcher;
        var result = WithSimpleCounter(enumerable, name, out watcher);
        
        watchersListToAdd.Add(watcher);
        
        return result;
    }
}