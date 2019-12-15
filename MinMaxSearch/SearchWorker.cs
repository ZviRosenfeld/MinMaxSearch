using System.Collections.Generic;
using System.Linq;
using MinMaxSearch.Cache;
using MinMaxSearch.Exceptions;
using MinMaxSearch.ThreadManagment;

namespace MinMaxSearch
{
    class SearchWorker
    {
        private readonly SearchOptions searchOptions;
        private readonly DeterministicSearchUtils deterministicSearchUtils;
        private readonly ProbabilisticSearchUtils probabilisticSearchUtils;
        private readonly ICacheManager cache;
        
        public SearchWorker(SearchOptions searchOptions, IThreadManager threadManager, ICacheManager cache)
        {
            this.searchOptions = searchOptions;
            this.cache = cache;
            deterministicSearchUtils = new DeterministicSearchUtils(this, searchOptions, threadManager);
            probabilisticSearchUtils = new ProbabilisticSearchUtils(threadManager, deterministicSearchUtils, searchOptions);
        }

        public SearchResult Evaluate(IState startState, SearchContext searchContext)
        {
            if (startState.Turn == Player.Empty)
                throw new EmptyPlayerException(nameof(startState.Turn) + " can't be " + nameof(Player.Empty));

            if (searchContext.CurrentDepth > 0)
            {
                var evaluation = cache.GetStateEvaluation(startState);
                if (evaluation != null && (evaluation.MinEvaluation >= searchOptions.MaxScore || evaluation.MaxEvaluation <= searchOptions.MinScore || evaluation.MinEvaluation >= searchContext.Bata || evaluation.MaxEvaluation <= searchContext.Alpha))
                    return new SearchResult(startState.Turn == Player.Max ? evaluation.MinEvaluation : evaluation.MaxEvaluation, startState);
            }

            if (searchOptions.Pruners.Any(pruner => pruner.ShouldPrune(startState, searchContext.CurrentDepth, searchContext.StatesUpTillNow)))
            {
                var evaluation = startState.Evaluate(searchContext.CurrentDepth, searchContext.StatesUpTillNow, searchOptions);
                return new SearchResult(evaluation, startState, false);
            }

            if (ShouldStop(startState, searchContext))
            {
                var stoppedDueToPrune = searchContext.PruneAtMaxDepth && searchContext.MaxDepth == searchContext.CurrentDepth;
                var evaluation = startState.Evaluate(searchContext.CurrentDepth, searchContext.StatesUpTillNow, searchOptions);
                return new SearchResult(evaluation, new List<IState> {startState}, 1, 0, stoppedDueToPrune);
            }

            SearchResult result;
            switch (startState)
            {
                case IDeterministicState deterministicState:
                    result = deterministicSearchUtils.EvaluateChildren(deterministicState, searchContext);
                    break;
                case IProbabilisticState probabilisticState:
                    result = probabilisticSearchUtils.EvaluateChildren(probabilisticState, searchContext);
                    break;
                default:
                    throw new BadStateTypeException($"State must implement {nameof(IDeterministicState)} or {nameof(IProbabilisticState)}");
            }

            if (result.AllChildrenAreDeadEnds)
            {
                if (startState.Turn == Player.Max)
                    cache.AddMinEvaluation(startState, result.Evaluation);
                else 
                    cache.AddMaxEvaluation(startState, result.Evaluation);
            }
            return result;
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
