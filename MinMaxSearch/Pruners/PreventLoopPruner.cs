using System.Collections.Generic;
using System.Linq;

namespace MinMaxSearch.Pruners
{
    /// <summary>
    /// A pruner that prevents loops.
    /// Note that this pruner will only work if the states implement Equals in a meaningfull way.
    /// </summary>
    class PreventLoopPruner : IPruner
    {
        public bool ShouldPrune(IDeterministicState state, int depth, List<IState> passedThroughStates) => 
            Enumerable.Contains(passedThroughStates, state);
    }
}
