namespace CRDT.Library.Counter
{
    public class PNCounter
    {
        private readonly GCounter _positive;

        // tombstone
        private readonly GCounter _negative;

        public PNCounter()
        {
            _positive = new GCounter();
            _negative = new GCounter();
        }

        public void Increment(int node)
        {
            _positive.Increment(node);
        }

        public void Decrement(int node)
        {
            _negative.Increment(node);
        }

        public int Query()
        {
            return _positive.Query() - _negative.Query();
        }

        public void Merge(PNCounter other)
        {
            _positive.Merge(other._positive);
            _negative.Merge(other._negative);
        }
    }
}