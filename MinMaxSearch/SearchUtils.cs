using System.Collections.Generic;

namespace MinMaxSearch
{
    static class SearchUtils
    {
        public static SearchResult CloneAndAddStateToTop(this SearchResult searchResult, IState state, int leaves, int internalNodes, bool fullTreeSearched, bool allChildrenAreDeadEnds, bool childrenPruned)
        {
            var sequance = new List<IState>(searchResult.StateSequence) {state};
            return new SearchResult(searchResult.Evaluation, sequance, leaves, internalNodes, fullTreeSearched, allChildrenAreDeadEnds, childrenPruned);
        }

        public static SearchContext CloneWithMaxAlphaAndBeta(this SearchContext searchContext) => new SearchContext(
            searchContext.MaxDepth, searchContext.CurrentDepth, searchContext.CancellationToken,
            searchContext.StatesUpTillNow, pruneAtMaxDepth: searchContext.PruneAtMaxDepth);

        public static SearchContext CloneAndAddState(this SearchContext searchContext, IState newState) =>
            new SearchContext(searchContext.MaxDepth, searchContext.CurrentDepth + 1, searchContext.CancellationToken, 
                new List<IState>(searchContext.StatesUpTillNow) {newState}, searchContext.Alpha, 
                searchContext.Bata, searchContext.PruneAtMaxDepth);

        public static double Evaluate(this IState state, int depth, List<IState> passedThroughStates,
            SearchOptions searchOptions)
        {
            return searchOptions.AlternateEvaluation?.Invoke(state, depth, passedThroughStates) ??
                   state.Evaluate(depth, passedThroughStates);
        }
    }
}
