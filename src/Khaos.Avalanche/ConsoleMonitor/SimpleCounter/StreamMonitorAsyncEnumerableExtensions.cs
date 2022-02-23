namespace Khaos.Avalanche.ConsoleMonitor.SimpleCounter;

public static class StreamMonitorAsyncEnumerableExtensions
{
    public static IAsyncEnumerable<T> WithSimpleCounter<T>(
        this IAsyncEnumerable<T> enumerable,
        SimpleCounter counter) =>
        enumerable.Apply(new StreamMonitor<T>(counter));
}