using System.Collections;

namespace CRDT.Set;

public class TwoPSet<T>
{
    private readonly GSet<T> _addSet = new();

    private readonly GSet<T> _removeSet = new();

    public TwoPSet(params T[] items)
    {
        foreach (var item in items)
        {
            Add(item);
        }
    }

    public TwoPSet(TwoPSet<T> twoPSet)
    {
        _addSet = new GSet<T>(twoPSet._addSet);
        _removeSet = new GSet<T>(twoPSet._removeSet);
    }

    public void Add(T element)
    {
        _addSet.Add(element);
    }

    public void Remove(T element)
    {
        _removeSet.Add(element);
    }

    public bool Contains(T element)
    {
        return _addSet.Contains(element) && !_removeSet.Contains(element);
    }

    public void Merge(TwoPSet<T> other)
    {
        _addSet.Merge(other._addSet);
        _removeSet.Merge(other._removeSet);
    }

    public IEnumerable<T> Query()
    {
        return _addSet.Query().Where(e => !_removeSet.Contains(e));
    }
}