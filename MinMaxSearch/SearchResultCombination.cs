using System.Collections.Generic;
using System.Linq;

namespace MinMaxSearch
{
    public static class SearchResultCombination
    {
        public static SearchResult CloneAndAddStateToTop(this SearchResult searchResult, IState state, bool allChildrenAreDeadEnds, int leaves, int internalNodes)
        {
            var sequance = new List<IState>(searchResult.StateSequence) {state};
            return new SearchResult(searchResult.Evaluation, sequance, leaves, internalNodes, allChildrenAreDeadEnds);
        }
    }
}
