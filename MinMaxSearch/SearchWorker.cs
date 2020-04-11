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
                var cachedResult = CheckCacheForEvaluation(startState, searchContext);
                if (cachedResult != null)
                    return cachedResult;
            }

            if (searchOptions.Pruners.Any(pruner => pruner.ShouldPrune(startState, searchContext.CurrentDepth, searchContext.StatesUpTillNow)))
            {
                var evaluation = startState.Evaluate(searchContext.CurrentDepth, searchContext.StatesUpTillNow, searchOptions);
                return new SearchResult(evaluation, startState, true, true, false);
            }

            if (ShouldStop(startState, searchContext))
            {
                var evaluation = startState.Evaluate(searchContext.CurrentDepth, searchContext.StatesUpTillNow, searchOptions);
                return new SearchResult(evaluation, new List<IState> { startState }, 1, 0, false, false, false);
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

            AddResultToCach(startState, searchContext.CurrentDepth, searchContext.StatesUpTillNow, result);
            return result;
        }

        private void AddResultToCach(IState state, int depth, IList<IState> statesUpTillNow, SearchResult result)
        {
            if (result.AllChildrenAreDeadEnds)
                cache.AddExactEvaluation(state, depth, statesUpTillNow, result.Evaluation);
            else if (searchOptions.StateDefinesDepth && searchOptions.IsUnstableState == null && searchOptions.CacheMode != CacheMode.ReuseCache && !result.ChildrenPruned)
                cache.AddExactEvaluation(state, depth, statesUpTillNow, result.Evaluation);
            else if (result.FullTreeSearchedOrPruned)
            {
                if (state.Turn == Player.Max)
                    cache.AddMinEvaluation(state, depth, statesUpTillNow, result.Evaluation);
                else
                    cache.AddMaxEvaluation(state, depth, statesUpTillNow, result.Evaluation);
            }
        }

        private SearchResult CheckCacheForEvaluation(IState startState, SearchContext searchContext)
        {
            var evaluation = cache.GetStateEvaluation(startState, searchContext.CurrentDepth, searchContext.StatesUpTillNow);
            if (evaluation == null)
                return null;

            if (evaluation.MaxEvaluation == evaluation.MinEvaluation)
                return new SearchResult(evaluation.MaxEvaluation, startState);

            if (evaluation.MinEvaluation >= searchOptions.MaxScore)
                return new SearchResult(evaluation.MinEvaluation, startState);

            if (evaluation.MaxEvaluation <= searchOptions.MinScore)
                return new SearchResult(evaluation.MaxEvaluation, startState);

            if (evaluation.MinEvaluation >= searchContext.Bata || evaluation.MaxEvaluation <= searchContext.Alpha)
                return new SearchResult(startState.Turn == Player.Max ? evaluation.MinEvaluation : evaluation.MaxEvaluation, startState, true, true, false);

            return null;
        }

        private bool ShouldStop(IState state, SearchContext searchContext)
        {
            if (searchContext.CurrentDepth >= searchContext.MaxDepth)
            {
                if (searchContext.PruneAtMaxDepth)
                    return true;
                if (searchOptions.IsUnstableState == null || !searchOptions.IsUnstableState(state, searchContext.CurrentDepth, searchContext.StatesUpTillNow))
                    return true;
            }
            if (searchContext.CancellationToken.IsCancellationRequested)
                return true;
            
            return false;
        }
    }
}
