// Dense Idetifier

using ExtendedNumerics;

namespace CRDT.Library.Common
{
    public record Identifier<T>(List<(BigRational, T)> Nodes) : IComparable<Identifier<T>> where T : IComparable<T>
    {
        public T Value => Nodes.Last().Item2;

        public int CompareTo(Identifier<T>? other)
        {
            if (other == null)
            {
                return 1;
            }

            var iter1 = Nodes.GetEnumerator();
            var iter2 = other.Nodes.GetEnumerator();
            while (true)
            {
                var ok = (iter1.Next(), iter2.Next()) switch
                {
                    (Some<(BigRational, T)>(var v1), Some<(BigRational, T)>(var v2)) => v1.CompareTo(v2) switch
                    {
                        // current nodes are equal, continue to compare next node
                        0 => None<int>.Default,
                        var x => new Some<int>(x)
                    },
                    // one of the identifiers has more nodes
                    (None<(BigRational, T)>, Some<(BigRational, T)>) => new Some<int>(-1),
                    (Some<(BigRational, T)>, None<(BigRational, T)>) => new Some<int>(1),
                    // both are None
                    _ => new Some<int>(0)
                };
                if (ok is Some<int>(var v))
                {
                    return v;
                }
            }
        }

        static BigRational RBetween(IOption<BigRational> low, IOption<BigRational> high)
        {
            return (low, high) switch
            {
                (Some<BigRational>(var lowValue), Some<BigRational>(var highValue)) => (lowValue + highValue) / 2,
                (Some<BigRational>(var lowValue), None<BigRational>) => lowValue + 1,
                (None<BigRational>, Some<BigRational>(var highValue)) => highValue - 1,
                _ => BigRational.Zero
            };
        }

        public static Identifier<T> Between(IOption<Identifier<T>> low, IOption<Identifier<T>> high, T marker)
        {
            return (low, high) switch
            {
                (Some<Identifier<T>>(var lowId), Some<Identifier<T>>(var highId)) => lowId.CompareTo(highId) switch
                {
                    > 0 => Between(highId, lowId, marker),
                    < 0 => Between(lowId, highId, marker),
                    0 => highId,
                },
                (Some<Identifier<T>>(var lowId), _) => new Identifier<T>(
                [
                    (RBetween(new Some<BigRational>(lowId.Nodes[0].Item1), None<BigRational>.Default), marker)
                ]),
                (_, Some<Identifier<T>>(var highId)) => new Identifier<T>(
                [
                    (RBetween(None<BigRational>.Default, new Some<BigRational>(highId.Nodes[0].Item1)), marker)
                ]),
                _ => new Identifier<T>(
                [
                    (RBetween(None<BigRational>.Default, None<BigRational>.Default), marker)
                ])
            };
        }

        public static Identifier<T> Between(Identifier<T> low, Identifier<T> high, T marker)
        {
            var nodes = new List<(BigRational, T)>();

            var lowIter = low.Nodes.GetEnumerator();
            var highIter = high.Nodes.GetEnumerator();
            while (true)
            {
                var curLow = lowIter.Next();
                var curHigh = highIter.Next();
                if (curLow is Some<(BigRational, T)>(var lowPair) && curHigh is Some<(BigRational, T)>(var highPair) &&
                    lowPair.Item1 == highPair.Item1)
                {
                    if (lowPair.Item2.CompareTo(marker) < 0 && marker.CompareTo(highPair.Item2) < 0)
                    {
                        // the marker fits between the low and high marker
                        nodes.Add((highPair.Item1, marker));
                        break;
                    }
                    else if (lowPair.Item2.CompareTo(highPair.Item2) == 0)
                    {
                        // we are on a common prefix of the two paths
                        nodes.Add((highPair.Item1, highPair.Item2));
                    }
                    else
                    {
                        // Otherwise, the two paths have diverged.
                        // Choose one path and clear out the other
                        nodes.Add((highPair.Item1, highPair.Item2));
                        lowIter = new List<(BigRational, T)>().GetEnumerator(); // Empty enumerator
                    }
                }
                else
                {
                    var lowId = curLow switch
                    {
                        Some<(BigRational, T)>((var Id, _)) => Id,
                        _ => BigRational.Zero
                    };
                    var highId = curHigh switch
                    {
                        Some<(BigRational, T)>((var Id, _)) => Id,
                        _ => BigRational.Zero
                    };

                    nodes.Add((RBetween(
                                new Some<BigRational>(lowId),
                                new Some<BigRational>(highId)),
                            marker)
                    );
                    break;
                }
            }

            return new Identifier<T>(nodes);
        }
    }
}