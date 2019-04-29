using System.Collections.Generic;

namespace MinMaxSearch.UnitTests
{
    class IncreasingNumberState : IState
    {
        private readonly int value;

        public IncreasingNumberState(int value)
        {
            this.value = value;
        }

        public IEnumerable<IState> GetNeighbors() =>
            new List<IncreasingNumberState>{ new IncreasingNumberState(value + 1), new IncreasingNumberState(value + 2)};

        public double Evaluate(int depth, List<IState> passedThroughStates) => value;
    }
}
