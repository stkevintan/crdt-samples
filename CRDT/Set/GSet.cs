namespace CRDT.Set;

public class GSet<T>
{
    private readonly HashSet<T> _set = [];

    public GSet(params T[] items)
    {
        foreach (var item in items)
        {
            Add(item);}
    }

    public GSet(GSet<T> gSet)
    {
        this._set = [..gSet._set];
    }
    
    public void Add(T element)
    {
        _set.Add(element);
    }

    public bool Contains(T element)
    {
        return _set.Contains(element);
    }

    public void Merge(GSet<T> other)
    {
        _set.UnionWith(other._set);
    }

    public IEnumerable<T> Query()
    {
        return _set;
    }
}