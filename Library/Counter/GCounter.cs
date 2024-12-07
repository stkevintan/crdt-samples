using CRDT.Library.Common;

namespace CRDT.Library.Counter
{
    public class GCounter
    {
        protected readonly Dictionary<int, int> _counters;

        public GCounter()
        {
            _counters = [];
        }

        public void Increment(int node)
        {
            _counters[node] = _counters.Find(node) switch
            {
                Some<int>(var v) => v + 1,
                _ => 1
            };
        }

        public int Query()
        {
            return _counters.Values.Sum();
        }

        public void Merge(GCounter other)
        {
            foreach (var kvp in other._counters)
            {
                _counters[kvp.Key] = _counters.Find(kvp.Key) switch
                {
                    Some<int>(var v) => Math.Max(v, kvp.Value),
                    _ => kvp.Value
                };
            }
        }

    }
}