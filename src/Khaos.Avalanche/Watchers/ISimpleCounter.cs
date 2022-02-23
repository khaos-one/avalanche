namespace Khaos.Avalanche.Watchers;

public interface ISimpleCounter
{
    public string Name { get; }
    public ulong Value { get; }
}