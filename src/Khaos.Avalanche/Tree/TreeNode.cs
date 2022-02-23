namespace Khaos.Avalanche.Tree;

public class TreeNode<TK, TV>
    where TK : IEquatable<TK>
{
    public TK Key { get; }
    public TV? Value { get; set; }

    public ICollection<TreeNode<TK, TV>> Children { get; } = new List<TreeNode<TK, TV>>();

    private HashSet<TK> _childrenIdsCache = new();

    public TreeNode(TK key, TV? value = default)
    {
        Key = key;
        Value = value;
    }

    public TreeNode(FlatTreeNode<TK, TV> flatNode)
        : this(flatNode.Key, flatNode.Value)
    { }

    public bool TryAppend(FlatTreeNode<TK, TV> flatNode)
    {
        if (Equals(Key, flatNode.ParentKey))
        {
            Children.Add(new TreeNode<TK, TV>(flatNode));
            _childrenIdsCache.Add(flatNode.Key);

            return true;
        }

        if (!_childrenIdsCache.Contains(flatNode.ParentKey!))
        {
            return false;
        }

        foreach (var child in Children)
        {
            var result = child.TryAppend(flatNode);

            if (result)
            {
                _childrenIdsCache.Add(flatNode.Key);

                return true;
            }
        }

        return false;
    }

    public T? TryApplyToNode<T>(TK nodeKey, Func<TreeNode<TK, TV>, T> fn)
    {
        if (nodeKey.Equals(Key))
        {
            return fn(this);
        }

        if (!_childrenIdsCache.Contains(nodeKey))
        {
            return default;
        }

        foreach (var child in Children)
        {
            var result = child.TryApplyToNode(nodeKey, fn);

            if (result is not null && !result.Equals(default))
            {
                return result;
            }
        }

        return default;
    }

    public void ApplyForEach(Action<TreeNode<TK, TV>> fn)
    {
        fn(this);

        foreach (var child in Children)
        {
            child.ApplyForEach(fn);
        }
    }

    public void ApplyForEachBackward(Action<TreeNode<TK, TV>> fn)
    {
        foreach (var child in Children)
        {
            child.ApplyForEachBackward(fn);
        }

        fn(this);
    }

    public IEnumerable<T> Extract<T>(Func<TreeNode<TK, TV>, T> fn)
    {
        yield return fn(this);

        foreach (var child in Children)
        {
            foreach (var result in child.Extract(fn))
            {
                yield return result;
            }
        }
    }

    public IEnumerable<FlatTreeNode<TK, TV>> ToFlat()
    {
        foreach (var child in Children)
        {
            foreach (var flatSubChild in child.ToFlat())
            {
                yield return flatSubChild;
            }

            yield return new FlatTreeNode<TK, TV>(child.Key, Key, child.Value);
        }
    }

    public TreeNode<TK, TV>? TryGetNodeByKey(TK key) => TryApplyToNode(key, node => node);

    public TreeNode<TK, TV>? TryFindDeepest(Func<TreeNode<TK, TV>, bool> criteria)
    {
        if (!criteria(this))
        {
            return null;
        }

        foreach (var child in Children)
        {
            var result = child.TryFindDeepest(criteria);

            if (result is not null)
            {
                return result;
            }
        }

        if (!Children.Any())
        {
            return this;
        }

        return null;
    }
}