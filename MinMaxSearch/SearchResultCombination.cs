using System.Collections.Generic;

namespace MinMaxSearch
{
    public static class SearchResultCombination
    {
        public static SearchResult CloneAndAddStateToTop(this SearchResult searchResult, IState state, int leaves, int internalNodes)
        {
            var sequance = new List<IState>(searchResult.StateSequence) {state};
            return new SearchResult(searchResult.Evaluation, sequance, leaves, internalNodes);
        }
    }
}
