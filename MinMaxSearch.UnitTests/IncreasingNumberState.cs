using System.Collections.Generic;

namespace MinMaxSearch.UnitTests
{
    class IncreasingNumberState : IDeterministicState
    {
        private readonly int value;

        public IncreasingNumberState(int value, Player turn)
        {
            this.value = value;
            Turn = turn;
        }

        public IEnumerable<IState> GetNeighbors() =>
            new List<IncreasingNumberState>{ new IncreasingNumberState(value + 1, Utils.GetReversePlayer(Turn)), new IncreasingNumberState(value + 2, Utils.GetReversePlayer(Turn))};

        public double Evaluate(int depth, List<IState> passedThroughStates) => value;
        public Player Turn { get; }
    }
}
