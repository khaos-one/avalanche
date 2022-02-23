namespace Khaos.Avalanche.Watchers;

public interface IWatcher { }

public interface IWatcher<T> : IWatcher, ITransform<T, T>
{ }