using MinMaxSearch;

namespace Connect4Tests
{
    public class Connect4TestUtils
    {
        public static SearchEngine GetSearchEngine(int maxDegreeOfParallelism, ParallelismMode parallelismMode) =>
            new SearchEngine()
            {
                MaxDegreeOfParallelism = maxDegreeOfParallelism,
                DieEarly = true,
                MinScore = BoardEvaluator.MinEvaluation,
                MaxScore = BoardEvaluator.MaxEvaluation,
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
