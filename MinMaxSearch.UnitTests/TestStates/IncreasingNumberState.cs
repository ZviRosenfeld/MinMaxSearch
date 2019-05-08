using System.Collections.Generic;

namespace MinMaxSearch.UnitTests.TestStates
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
            new List<IncreasingNumberState>{ new IncreasingNumberState(value + 1, Turn.GetReversePlayer()), new IncreasingNumberState(value + 2, Turn.GetReversePlayer())};

        public double Evaluate(int depth, List<IState> passedThroughStates) => value;
        public Player Turn { get; }
    }
}
