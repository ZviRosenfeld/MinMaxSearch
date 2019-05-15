using System.Collections.Generic;

namespace MinMaxSearch
{
    public static class Utils
    {
        public static Player GetReversePlayer(this Player player) =>
            player == Player.Max ? Player.Min : Player.Max;

        public static SearchResult CloneAndAddStateToTop(this SearchResult searchResult, IState state, int leaves, int internalNodes, bool allChildrenAreDeadEnds)
        {
            var sequance = new List<IState>(searchResult.StateSequence) {state};
            return new SearchResult(searchResult.Evaluation, sequance, leaves, internalNodes, allChildrenAreDeadEnds);
        }

        public static SearchContext CloneWithMaxAlphaAndBeta(this SearchContext searchContext) => new SearchContext(
            searchContext.MaxDepth, searchContext.CurrentDepth, searchContext.CancellationToken, searchContext.StartPlayer,
            searchContext.StatesUpTillNow, pruneAtMaxDepth: searchContext.PruneAtMaxDepth);

        public static SearchContext CloneAndAddState(this SearchContext searchContext, IState newState) =>
            new SearchContext(searchContext.MaxDepth, searchContext.CurrentDepth + 1, searchContext.CancellationToken, 
                searchContext.StartPlayer, new List<IState>(searchContext.StatesUpTillNow) {newState}, searchContext.Alpha, 
                searchContext.Bata, searchContext.PruneAtMaxDepth);

        public static double Evaluate(this IState state, int depth, List<IState> passedThroughStates, Player startPlayer, SearchOptions searchOptions)
        {
            if (startPlayer == Player.Max && searchOptions.MaxAlternateEvaluation != null)
                return searchOptions.MaxAlternateEvaluation(state, depth, passedThroughStates);

            if (startPlayer == Player.Min && searchOptions.MinAlternateEvaluation != null)
                return searchOptions.MinAlternateEvaluation(state, depth, passedThroughStates);

            return state.Evaluate(depth, passedThroughStates);
        }
    }
}
