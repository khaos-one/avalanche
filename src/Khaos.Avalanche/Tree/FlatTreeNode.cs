namespace Khaos.Avalanche.Tree;

public class FlatTreeNode<TK, TV> 
    where TK : IEquatable<TK>
{
    public TK Key { get; }
    public TK ParentKey { get; }
    public TV? Value { get; }

    public FlatTreeNode(TK key, TK parentKey, TV? value = default)
    {
        Key = key;
        Value = value;
        ParentKey = parentKey;
    }
}