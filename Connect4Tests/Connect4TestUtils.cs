﻿using MinMaxSearch;

namespace Connect4Tests
{
    public class Connect4TestUtils
    {
        public static SearchEngine GetSearchEngine(int maxDegreeOfParallelism, ParallelismMode parallelismMode, int levelOfParallelism = 1) =>
            new SearchEngine()
            {
                MaxDegreeOfParallelism = maxDegreeOfParallelism,
                MaxLevelOfParallelism = levelOfParallelism,
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

        /// <summary>
        /// Gets an almost full board that we will use for testing.
        /// </summary>
        public static Player[,] GetHalfFullBoard() =>
            new[,]
            {
                {Player.Min, Player.Max, Player.Min, Player.Max, Player.Min, Player.Max},
                {Player.Max, Player.Min, Player.Max, Player.Min, Player.Max, Player.Min},
                {Player.Max, Player.Max, Player.Max, Player.Min, Player.Max, Player.Max},
                {Player.Min, Player.Min, Player.Min, Player.Empty, Player.Min, Player.Min},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},      
            };
    }
}
