using System.Collections.Generic;
using System.Threading;

namespace MinMaxSearch.UnitTests.TestStates
{
    class SlowState : IDeterministicState
    {
        public SlowState(int value)
        {
            Value = value;
        }

        public int Value { get; }

        public double Evaluate(int depth, List<IState> passedThroughStates) => Value;

        public Player Turn => Player.Max;

        public IEnumerable<IState> GetNeighbors()
        {
            Thread.Sleep(50);
            return new List<IState> {new SlowState(Value - 1), new SlowState(Value), new SlowState(Value + 1)};
        }
    }
}
