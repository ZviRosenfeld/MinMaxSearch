using System.Collections.Generic;

namespace MinMaxSearch.Pruners
{
    /// <summary>
    /// A pruner that prevents loops.
    /// Note that this pruner will only work if the states implement Equals in a meaningful way.
    /// </summary>
    class PreventLoopPruner : IPruner
    {
        public bool ShouldPrune(IState state, int depth, List<IState> passedThroughStates) =>
            passedThroughStates.Contains(state);

        public override bool Equals(object obj) => obj is PreventLoopPruner;

        public override int GetHashCode() => typeof(PreventLoopPruner).GetHashCode();
    }
}
