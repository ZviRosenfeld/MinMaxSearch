using MinMaxSearch;
using MinMaxSearch.Cache;

namespace Connect4Tests
{
    public class Connect4TestUtils
    {
        public static SearchEngine GetSearchEngine(int maxDegreeOfParallelism, ParallelismMode parallelismMode, int levelOfParallelism = 1, CacheMode cacheMode = CacheMode.NewCache) =>
            new SearchEngine(cacheMode, CacheKeyType.StateOnly)
            {
                MaxDegreeOfParallelism = maxDegreeOfParallelism,
                MaxLevelOfParallelism = levelOfParallelism,
                DieEarly = true,
                MinScore = BoardEvaluator.MinEvaluation,
                MaxScore = BoardEvaluator.MaxEvaluation,
                ParallelismMode = parallelismMode,
                SkipEvaluationForFirstNodeSingleNeighbor = false,
                StateDefinesDepth = true
            };

        public static Player[,] GetEmptyBoard() =>
            new[,]
            {
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
            };

        /// <summary>
        /// Gets an almost full board that we will use for testing.
        /// </summary>
        public static Player[,] GetHalfFullBoard() =>
            new[,]
            {
                {Player.Max, Player.Min, Player.Max, Player.Min, Player.Max, Player.Min},
                {Player.Max, Player.Max, Player.Max, Player.Min, Player.Max, Player.Max},
                {Player.Min, Player.Min, Player.Min, Player.Empty, Player.Min, Player.Min},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
            };

        public static Connect4State GetMaxFiveMovesAwayFromWinningState() =>
            new Connect4State(new[,]
            {
                {Player.Empty, Player.Min, Player.Max, Player.Empty, Player.Max, Player.Empty},
                {Player.Empty, Player.Max, Player.Min, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Max, Player.Max, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
            }, Player.Max);
    }
}
