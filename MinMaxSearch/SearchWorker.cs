using System;
using System.Collections.Generic;
using System.Linq;

namespace MinMaxSearch
{
    class SearchWorker
    {
        private readonly SearchOptions searchOptions;
        private readonly ThreadManager threadManager;
        private readonly DeterministicSearchUtils deterministicSearchUtils;
        private readonly ProbabilisticSearchUtils probabilisticSearchUtils;
        
        public SearchWorker(SearchOptions searchOptions)
        {
            this.searchOptions = searchOptions;
            threadManager = new ThreadManager(searchOptions.MaxDegreeOfParallelism);
            deterministicSearchUtils = new DeterministicSearchUtils(this, searchOptions, threadManager);
            probabilisticSearchUtils = new ProbabilisticSearchUtils(threadManager, deterministicSearchUtils);
        }

        public SearchResult Evaluate(IState startState, SearchContext searchContext)
        {
            if (startState.Turn == Player.Empty)
                throw new EmptyPlayerException(nameof(startState.Turn) + " can't be " + nameof(Player.Empty));

            if (searchOptions.Pruners.Any(pruner => pruner.ShouldPrune(startState, searchContext.CurrentDepth, searchContext.StatesUpTillNow)))
            {
                var evaluation = startState.Evaluate(searchContext.CurrentDepth, searchContext.StatesUpTillNow, searchContext.StartPlayer);
                return new SearchResult(evaluation, new List<IState> { startState }, 1, 0, true);
            }

            if (ShouldStop(startState, searchContext))
            {
                var stoppedDueToPrune = searchContext.PruneAtMaxDepth && searchContext.MaxDepth == searchContext.CurrentDepth;
                var evaluation = startState.Evaluate(searchContext.CurrentDepth, searchContext.StatesUpTillNow, searchContext.StartPlayer);
                return new SearchResult(evaluation, new List<IState> {startState}, 1, 0, stoppedDueToPrune);
            }

            if (startState is IDeterministicState deterministicState)
                return deterministicSearchUtils.EvaluateChildren(deterministicState, searchContext);

            if (startState is IProbabilisticState probabilisticState)
                return probabilisticSearchUtils.EvaluateChildren(probabilisticState, searchContext);
            
            throw new BadStateTypeException($"State must implement {nameof(IDeterministicState)} or {nameof(IProbabilisticState)}");
        }
        
        private bool ShouldStop(IState state, SearchContext searchContext)
        {
            if (searchContext.CurrentDepth >= searchContext.MaxDepth)
            {
                if (searchContext.PruneAtMaxDepth)
                    return true;
                if (!searchOptions.IsUnstableState(state, searchContext.CurrentDepth, searchContext.StatesUpTillNow))
                    return true;
            }
            if (searchContext.CancellationToken.IsCancellationRequested)
                return true;
            
            return false;
        }
    }
}
