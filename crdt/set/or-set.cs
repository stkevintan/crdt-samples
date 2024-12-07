namespace CRDT.Set
{
    class VClock : GCounter
    {
    }

    class ORSet<T>
    {
        private readonly Dictionary<T, HashSet<int>> _addSet = new();
        private readonly Dictionary<T, HashSet<int>> _removeSet = new();

        public void Add(T element, int node)
        {
            if (!_addSet.ContainsKey(element))
            {
                _addSet[element] = new HashSet<int>();
            }
            _addSet[element].Add(node);
        }

        public void Remove(T element, int node)
        {
            if (!_removeSet.ContainsKey(element))
            {
                _removeSet[element] = new HashSet<int>();
            }
            _removeSet[element].Add(node);
        }

        public bool Contains(T element)
        {
            if (!_addSet.ContainsKey(element))
            {
                return false;
            }
            if (_removeSet.ContainsKey(element))
            {
                return _addSet[element].Except(_removeSet[element]).Any();
            }
            return true;
        }

        public void Merge(ORSet<T> other)
        {
            foreach (var kvp in other._addSet)
            {
                if (_addSet.ContainsKey(kvp.Key))
                {
                    _addSet[kvp.Key].UnionWith(kvp.Value);
                }
                else
                {
                    _addSet[kvp.Key] = new HashSet<int>(kvp.Value);
                }
            }
            foreach (var kvp in other._removeSet)
            {
                if (_removeSet.ContainsKey(kvp.Key))
                {
                    _removeSet[kvp.Key].UnionWith(kvp.Value);
                }
                else
                {
                    _removeSet[kvp.Key] = new HashSet<int>(kvp.Value);
                }
            }
        }

        public IEnumerable<T> Query()
        {
            return _addSet.Where(kvp =>
            {
                if (_removeSet.ContainsKey(kvp.Key))
                {
                    return kvp.Value.Except(_removeSet[kvp.Key]).Any();
                }
                return true;
            }).Select(kvp => kvp.Key);
        }
    }
}