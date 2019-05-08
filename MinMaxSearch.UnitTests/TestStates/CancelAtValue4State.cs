using System.Collections.Generic;
using System.Threading;

namespace MinMaxSearch.UnitTests.TestStates
{
    class CancelAtValue4State : IDeterministicState
    {
        public readonly int Value;
        private readonly CancellationTokenSource cancellationTokenSource;

        public CancelAtValue4State(int value, CancellationTokenSource cancellationTokenSource, Player turn)
        {
            Value = value;
            this.cancellationTokenSource = cancellationTokenSource;
            Turn = turn;
        }

        public IEnumerable<IState> GetNeighbors()
        {
            if (Value >= 4)
                cancellationTokenSource.Cancel();

            return new List<IDeterministicState> {new CancelAtValue4State(Value + 1, cancellationTokenSource, Turn.GetReversePlayer())};
        }

        public double Evaluate(int depth, List<IState> passedThroughStates) => Value;
        public Player Turn { get; }

        public override string ToString() => Value.ToString();
    }
}
