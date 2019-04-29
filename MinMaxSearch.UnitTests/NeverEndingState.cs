using System;
using System.Collections.Generic;
using System.Text;

namespace MinMaxSearch.UnitTests
{
    class NeverEndingState : IState
    {
        public readonly int Value;

        public NeverEndingState(int value)
        {
            Value = value == 1 ? 1 : 0;
        }

        public IEnumerable<IState> GetNeighbors() =>
            new List<IState> {new NeverEndingState(Value == 1 ? 0: 1)};

        public double Evaluate(int depth, List<IState> passedThroughStates) => 
            depth > 3? throw new Exception("Shouldn't have gotten so far into the search") : Value;

        public override bool Equals(object obj) =>
            obj is NeverEndingState neverEnding && neverEnding.Value == Value;

        public override int GetHashCode() => Value;

        public override string ToString() => Value.ToString();
    }
}
