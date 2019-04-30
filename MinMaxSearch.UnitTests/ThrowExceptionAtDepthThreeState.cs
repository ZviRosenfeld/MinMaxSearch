using System;
using System.Collections.Generic;

namespace MinMaxSearch.UnitTests
{
    class ThrowExceptionAtDepthThreeState : IState
    {
        public readonly int Value;

        public ThrowExceptionAtDepthThreeState(int value)
        {
            Value = value == 1 ? 1 : 0;
        }

        public IEnumerable<IState> GetNeighbors() =>
            new List<IState> {new ThrowExceptionAtDepthThreeState(Value == 1 ? 0: 1)};

        public double Evaluate(int depth, List<IState> passedThroughStates) => 
            depth > 3? throw new Exception("Shouldn't have gotten so far into the search") : Value;

        public override bool Equals(object obj) =>
            obj is ThrowExceptionAtDepthThreeState neverEnding && neverEnding.Value == Value;

        public override int GetHashCode() => Value;

        public override string ToString() => Value.ToString();
    }
}
