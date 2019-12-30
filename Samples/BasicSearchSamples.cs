using System.Threading;
using MinMaxSearch;
using TicTacToeTests;

namespace Samples
{
    class BasicSearchSamples
    {
        public SearchResult Example1()
        {
            IDeterministicState startState = Utils.GetEmptyTicTacToeState();
            int searchDepth = 5;
            SearchEngine engine = new SearchEngine();
            SearchResult searchResult = engine.Search(startState, searchDepth);
            return searchResult;
        }

        public SearchResult Example2()
        {
            IDeterministicState startState = Utils.GetEmptyTicTacToeState();
            int searchDepth = 5;
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            SearchEngine engine = new SearchEngine()
            {
                FavorShortPaths = true,
                DieEarly = true,
                MaxScore = 99,
                MinScore = -99
            };
            SearchResult searchResult = engine.Search(startState, searchDepth, cancellationTokenSource.Token);
            return searchResult;
        }
    }
}
