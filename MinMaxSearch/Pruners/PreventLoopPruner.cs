using System.Collections.Generic;
using System.Linq;

namespace MinMaxSearch.Pruners
{
    /// <summary>
    /// A pruner that prevents loops.
    /// Note that this pruner will only work if: 
    /// A) option RecordPassThroughStates is set to true on the SearchEngine, and 
    /// B) the states implement Equals in a meaningfull way.
    /// </summary>
    class PreventLoopPruner : IPruner
    {
        public bool ShouldPrune(IState state, int depth, List<IState> passedThroughStates) => 
            Enumerable.Contains(passedThroughStates, state);
    }
}
