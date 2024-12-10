using CRDT.Counter;

namespace CRDT.Common;

internal class VTime : GCounter
{
    public ICollection<int> Keys => _counters.Keys;

    public int GetOrDefault(int key)
    {
        return _counters.Find(key) switch
        {
            Some<int>(var v) => v,
            _ => 0
        };
    }
}

public enum Ord
{
    Lt = -1,
    Eq = 0,
    Gt = 1,
    Cc = 2,
}

public class VClock
{
    private readonly VTime vTime = new();

    public void Increment(int node)
    {
        vTime.Increment(node);
    }

    public void Merge(VClock other)
    {
        vTime.Merge(other.vTime);
    }

    public Ord Compare(VClock other)
    {
        return vTime.Keys.Union(other.vTime.Keys).Aggregate(Ord.Eq, (acc, k) =>
        {
            var value1 = vTime.GetOrDefault(k);
            var value2 = other.vTime.GetOrDefault(k);
            return acc switch
            {
                Ord.Eq when value1 > value2 => Ord.Gt,
                Ord.Eq when value1 < value2 => Ord.Lt,
                Ord.Lt when value1 > value2 => Ord.Cc,
                Ord.Gt when value1 < value2 => Ord.Cc,
                _ => acc
            };
        });
    }

    public static bool HappenedBefore(VClock v1, VClock v2)
    {
        return v1.Compare(v2) == Ord.Lt;
    }
}