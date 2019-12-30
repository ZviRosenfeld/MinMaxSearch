using System.Collections.Generic;
using MinMaxSearch.Pruners;

namespace MinMaxSearch.Samples
{
    class SamplePruner : IPruner
    {
        public bool ShouldPrune(IState state, int depth, List<IState> passedThroughStates)
        {
            // Some logic here to decide if we should prune
            return false;
        }
    }

    class UsePruner
    {
        public ISearchEngine GetEngineWithPruner()
        {
            SearchEngine engine = new SearchEngine();
            engine.AddPruner(new SamplePruner());
            return engine;
        }
    }
}
