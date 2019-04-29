using System.Collections.Generic;
using System.Linq;

namespace MinMaxSearch
{
    public static class SearchResultCombination
    {
        public static SearchResult CloneAndAddStateToTop(this SearchResult searchResult, IState state, bool allChildrenAreDeadEnds, int leaves, int internalNodes)
        {
            var sequance = new List<IState>(searchResult.StateSequence) {state};
            return new SearchResult(state, searchResult.Evaluation, sequance, leaves, internalNodes, allChildrenAreDeadEnds);
        }

        public static SearchResult CloneAndRemoveTopNode(this SearchResult searchResult)
        {
            var sequance = new List<IState>(searchResult.StateSequence);
            sequance.RemoveAt(0);
            return new SearchResult(sequance.First(), searchResult.Evaluation, sequance, searchResult.Leaves, searchResult.IntarnalNodes, searchResult.DeadEnd);
        }
    }
}
