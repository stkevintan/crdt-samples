namespace CRDT.Set;

class TwoPSet<T>
{
    private readonly GSet<T> _addSet = new();

    private readonly GSet<T> _removeSet = new();

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