namespace Khaos.Avalanche;

public interface ISource<out T> : IAsyncEnumerable<T>
{ }