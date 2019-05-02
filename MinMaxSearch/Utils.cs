using System.Collections.Generic;

namespace MinMaxSearch
{
    public static class Utils
    {
        public static Player GetReversePlayer(Player player) =>
            (Player) ((int) player * -1);

        public static SearchResult CloneAndAddStateToTop(this SearchResult searchResult, IState state, int leaves, int internalNodes)
        {
            var sequance = new List<IState>(searchResult.StateSequence) {state};
            return new SearchResult(searchResult.Evaluation, sequance, leaves, internalNodes);
        }

    }
}
