using System.Collections.Immutable;

namespace Khaos.Avalanche.Tree;

public class RootTreeNode<TK, TV> : TreeNode<TK, TV> 
    where TK : IEquatable<TK>
{
    private readonly List<FlatTreeNode<TK, TV>> _nodesBuffer = new();

    public RootTreeNode(TK key, TV? value = default) : base(key, value)
    { }

    public RootTreeNode() : base(default!, default)
    { }

    public void Append(FlatTreeNode<TK, TV> flatNode)
    {
        TryAppendInternal(flatNode);
        ProcessNodesBuffer();
    }

    private void TryAppendInternal(FlatTreeNode<TK, TV> flatNode)
    {
        var result = TryAppend(flatNode);

        if (!result)
        {
            _nodesBuffer.Add(flatNode);
        }
    }

    public void ProcessNodesBuffer()
    {
        var bufferCopy = _nodesBuffer.ToImmutableArray();

        foreach (var node in bufferCopy)
        {
            _nodesBuffer.Remove(node);
            TryAppendInternal(node);
        }
    }
}