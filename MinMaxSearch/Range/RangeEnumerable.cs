using System.Collections;
using System.Collections.Generic;

namespace MinMaxSearch.Range
{
    class RangeEnumerable : IEnumerable<int>
    {
        private readonly int min;
        private readonly int max;

        public RangeEnumerable(int min, int max)
        {
            this.min = min;
            this.max = max;
        }

        public IEnumerator<int> GetEnumerator() => new RangeEnumerator(min, max);

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
