using System.Threading;
using MinMaxSearch;
using TicTacToeTests;

namespace Samples
{
    class IterativeSearchSample
    {
        public SearchResult RunIterativeSearch()
        {
            IDeterministicState startState = Utils.GetEmptyTicTacToeState();
            int startDepth = 2;
            int endDepth = 5;
            CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
            ISearchEngine engine = new SearchEngine();
            IterativeSearchWrapper iterativeEngine = new IterativeSearchWrapper(engine);
            // This will run an IterativeSearch beginning at depth 2, and ending with depth 5 (including)
            SearchResult searchResult = iterativeEngine.IterativeSearch(startState, startDepth, endDepth, CancellationTokenSource.Token);
            return searchResult;
        }
    }
}
