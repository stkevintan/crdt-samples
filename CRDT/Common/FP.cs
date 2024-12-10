using System.Diagnostics.CodeAnalysis;

namespace CRDT.Common;

public interface IOption<A> where A : notnull
{
    static IOption<A> From(A? value)
    {
        if (value == null)
        {
            return None<A>.Default;
        }

        return new Some<A>(value);
    }
}

public class Some<A> : IOption<A> where A : notnull
{
    public readonly A Value;

    internal Some(A value) =>
        Value = value;

    public void Deconstruct(out A Value) =>
        Value = this.Value;
}

public class None<A> : IOption<A> where A : notnull
{
    internal static IOption<A> Default = new None<A>();

    internal None()
    {
    }
}

internal static class DictionaryExtensions
{
    public static IOption<TValue> Find<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        where TKey : notnull where TValue : notnull
    {
        if (dictionary.TryGetValue(key, out var value))
        {
            return new Some<TValue>(value);
        }

        return None<TValue>.Default;
    }

    public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key,
        Func<TValue> factory)
        where TKey : notnull where TValue : notnull
    {
        if (dictionary.TryGetValue(key, out var value))
        {
            return value;
        }

        return dictionary[key] = factory();
    }

    public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        where TKey : notnull where TValue : notnull, new()
    {
        return GetOrDefault(dictionary, key, () => new TValue());
    }
}

internal static class EnumerableExtensions
{
    public static IOption<T> Next<T>(this IEnumerator<T> enumerator) where T : notnull
    {
        return enumerator.MoveNext() ? new Some<T>(enumerator.Current) : None<T>.Default;
    }
}