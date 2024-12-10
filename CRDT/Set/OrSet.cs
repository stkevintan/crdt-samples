using System.Collections;
using CRDT.Common;

namespace CRDT.Set;

public class OrSet<T> : IEnumerable<T> where T : notnull
{
    private readonly Dictionary<T, VClock> _addSet = new();
    private readonly Dictionary<T, VClock> _removeSet = new();

    public void Add(T element, int node)
    {
        _addSet.GetOrDefault(element).Increment(node);
        // safe to remove element, because we refreshed the record in _addSet
        _removeSet.Remove(element);
    }

    public void Remove(T element, int node)
    {
        _removeSet.GetOrDefault(element).Increment(node);
        // safe to remove element, because we refreshed the record in _removeSet
        _addSet.Remove(element);
    }

    public bool Contains(T element)
    {
        return (_addSet.Find(element), _removeSet.Find(element)) switch
        {
            (Some<VClock>(var add), Some<VClock>(var rem)) => VClock.HappenedBefore(rem, add),
            (Some<VClock> add, _) => true,
            _ => false
        };
    }

    public void Merge(OrSet<T> other)
    {
        // merge add and remove sets
        foreach (var kvp in other._addSet)
        {
            _addSet.GetOrDefault(kvp.Key).Merge(kvp.Value);
        }

        foreach (var kvp in other._removeSet)
        {
            _removeSet.GetOrDefault(kvp.Key).Merge(kvp.Value);
        }

        // Optimization: kick out elements that are lost
        foreach (var (key, add) in _addSet)
        {
            _ = _removeSet.Find(key) switch
            {
                Some<VClock>(var rem) when VClock.HappenedBefore(rem, add) => _removeSet.Remove(key),
                _ => false
            };
        }

        foreach (var (key, rem) in _removeSet)
        {
            _ = _addSet.Find(key) switch
            {
                Some<VClock>(var add) when VClock.HappenedBefore(rem, add) => false,
                _ => _addSet.Remove(key)
            };
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _addSet.Where(kvp => _removeSet.Find(kvp.Key) switch
        {
            Some<VClock>(var rem) => VClock.HappenedBefore(rem, kvp.Value),
            _ => true
        }).Select(kvp => kvp.Key).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}