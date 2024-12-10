using System.Diagnostics.CodeAnalysis;
using CRDT.Common;

namespace CRDT.Sequence;

public class GSequence<T> where T : notnull, IComparable<T>
{
    private readonly SortedSet<Identifier<T>> list = new();

    public int Count => list.Count;

    public IEnumerable<T> Query()
    {
        return list.Select(e => e.Value);
    }

    public bool TryGetValue(int index, [MaybeNullWhen(false)] out T value)
    {
        if (index < 0 || index >= list.Count)
        {
            value = default;
            return false;
        }

        value = list.ElementAt(index).Value;
        return true;
    }

    public IOption<Identifier<T>> TryGetIdentifier(int index)
    {
        if (index < 0 || index >= list.Count)
        {
            return None<Identifier<T>>.Default;
        }

        return new Some<Identifier<T>>(list.ElementAt(index));
    }

    public Identifier<T> Insert(int index, T value)
    {
        if (index < 0 || index > list.Count)
        {
            throw new IndexOutOfRangeException();
        }

        if (index == 0)
        {
            return InsertBefore(TryGetIdentifier(index), value);
        }

        return InsertAfter(TryGetIdentifier(index - 1), value);
    }

    public Identifier<T> InsertBefore(IOption<Identifier<T>> highId, T value)
    {
        var lowId = highId switch
        {
            Some<Identifier<T>>(var hid) => list.GetViewBetween(list.Min, hid).Reverse()
                .FirstOrDefault(id => id?.CompareTo(hid) < 0, null),
            _ => null
        };
        return Identifier<T>.Between(IOption<Identifier<T>>.From(lowId), highId, value);
    }

    public Identifier<T> InsertAfter(IOption<Identifier<T>> lowId, T value)
    {
        var highId = lowId switch
        {
            Some<Identifier<T>>(var lid) => list.GetViewBetween(list.Min, lid)
                .FirstOrDefault(id => id?.CompareTo(lid) < 0, null),
            _ => null
        };
        return Identifier<T>.Between(lowId, IOption<Identifier<T>>.From(highId), value);
    }
}