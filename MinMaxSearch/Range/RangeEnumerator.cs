using System;
using System.Collections;
using System.Collections.Generic;

namespace MinMaxSearch.Range
{
    class RangeEnumerator : IEnumerator<int>
    {
        private readonly int min;
        private readonly int max;

        public RangeEnumerator(int min, int max)
        {
            if (min >= max)
                throw new Exception($"{nameof(max)} (== {max}) must be bigger than {nameof(min)} ( == {min})");

            this.min = min;
            this.max = max;
            Current = min - 1;
        }

        public bool MoveNext()
        {
            Current++;
            return Current <= max;
        }

        public void Reset()
        {
            Current = min - 1;
        }

        public int Current { get; private set; }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            // Do nothing
        }
    }
}
