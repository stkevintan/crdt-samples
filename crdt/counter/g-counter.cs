namespace CRDT.Counter
{
    public class GCounter
    {
        private readonly Dictionary<int, int> _counters;

        public GCounter()
        {
            _counters = [];
        }

        public void Increment(int node)
        {
            if (_counters.TryGetValue(node, out var value))
            {
                _counters[node] = value + 1;
            }
            else
            {
                _counters[node] = 1;
            }
        }

        public int Query()
        {
            return _counters.Values.Sum();
        }

        public void Merge(GCounter other)
        {
            foreach (var kvp in other._counters)
            {
                if (_counters.TryGetValue(kvp.Key, out var value))
                {
                    _counters[kvp.Key] = Math.Max(value, kvp.Value);
                }
                else
                {
                    _counters[kvp.Key] = kvp.Value;
                }
            }
        }
    }
}