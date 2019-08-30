using MinMaxSearch;

namespace Connect4Tests
{
    public class Connect4TestUtils
    {
        public static SearchEngine GetSearchEngine(int maxDegreeOfParallelism, ParallelismMode parallelismMode) =>
            GetSearchEngineBuilder(maxDegreeOfParallelism, parallelismMode).Build();

        public static SearchEngineBuilder GetSearchEngineBuilder(int maxDegreeOfParallelism, ParallelismMode parallelismMode) =>
            new SearchEngineBuilder()
            {
                MaxDegreeOfParallelism = maxDegreeOfParallelism,
                DieEarly = true,
                MinScore = BoardEvaluator.MinEvaluation + 1,
                MaxScore = BoardEvaluator.MaxEvaluation - 1,
                ParallelismMode = parallelismMode
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
    }
}
