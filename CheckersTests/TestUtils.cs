using System;
using System.Collections.Generic;
using System.Linq;
using MinMaxSearch;
using MinMaxSearch.Cache;

namespace CheckersTests
{
    static class TestUtils
    {
        public static CheckerPiece[,] GetStartBoard() => new[,]
        {
            { CheckerPiece.Empty, CheckerPiece.MaxChecker, CheckerPiece.Empty, CheckerPiece.MaxChecker, CheckerPiece.Empty, CheckerPiece.MaxChecker, CheckerPiece.Empty, CheckerPiece.MaxChecker},
            { CheckerPiece.MaxChecker, CheckerPiece.Empty, CheckerPiece.MaxChecker, CheckerPiece.Empty, CheckerPiece.MaxChecker, CheckerPiece.Empty, CheckerPiece.MaxChecker, CheckerPiece.Empty},
            { CheckerPiece.Empty, CheckerPiece.MaxChecker, CheckerPiece.Empty, CheckerPiece.MaxChecker, CheckerPiece.Empty, CheckerPiece.MaxChecker, CheckerPiece.Empty, CheckerPiece.MaxChecker},
            { CheckerPiece.Empty, CheckerPiece.Empty, CheckerPiece.Empty, CheckerPiece.Empty, CheckerPiece.Empty, CheckerPiece.Empty, CheckerPiece.Empty, CheckerPiece.Empty},
            { CheckerPiece.Empty, CheckerPiece.Empty, CheckerPiece.Empty, CheckerPiece.Empty, CheckerPiece.Empty, CheckerPiece.Empty, CheckerPiece.Empty, CheckerPiece.Empty},
            { CheckerPiece.MinChecker, CheckerPiece.Empty, CheckerPiece.MinChecker, CheckerPiece.Empty, CheckerPiece.MinChecker, CheckerPiece.Empty, CheckerPiece.MinChecker, CheckerPiece.Empty},
            { CheckerPiece.Empty, CheckerPiece.MinChecker, CheckerPiece.Empty, CheckerPiece.MinChecker, CheckerPiece.Empty, CheckerPiece.MinChecker, CheckerPiece.Empty, CheckerPiece.MinChecker},
            { CheckerPiece.MinChecker, CheckerPiece.Empty, CheckerPiece.MinChecker, CheckerPiece.Empty, CheckerPiece.MinChecker, CheckerPiece.Empty, CheckerPiece.MinChecker, CheckerPiece.Empty},
        };
        
        public static CheckerPiece[,] GetStartBoardFullOfKings() => new[,]
        {
            { CheckerPiece.Empty, CheckerPiece.MaxKing, CheckerPiece.Empty, CheckerPiece.MinKing, CheckerPiece.Empty, CheckerPiece.MaxKing, CheckerPiece.Empty, CheckerPiece.MaxKing},
            { CheckerPiece.MaxKing, CheckerPiece.Empty, CheckerPiece.MaxKing, CheckerPiece.Empty, CheckerPiece.MaxKing, CheckerPiece.Empty, CheckerPiece.MaxKing, CheckerPiece.Empty},
            { CheckerPiece.Empty, CheckerPiece.MaxKing, CheckerPiece.Empty, CheckerPiece.MinKing, CheckerPiece.Empty, CheckerPiece.MaxKing, CheckerPiece.Empty, CheckerPiece.MaxKing},
            { CheckerPiece.Empty, CheckerPiece.Empty, CheckerPiece.Empty, CheckerPiece.Empty, CheckerPiece.Empty, CheckerPiece.Empty, CheckerPiece.Empty, CheckerPiece.Empty},
            { CheckerPiece.Empty, CheckerPiece.Empty, CheckerPiece.Empty, CheckerPiece.Empty, CheckerPiece.Empty, CheckerPiece.Empty, CheckerPiece.Empty, CheckerPiece.Empty},
            { CheckerPiece.MinKing, CheckerPiece.Empty, CheckerPiece.MinKing, CheckerPiece.Empty, CheckerPiece.MinKing, CheckerPiece.Empty, CheckerPiece.MinKing, CheckerPiece.Empty},
            { CheckerPiece.Empty, CheckerPiece.MinKing, CheckerPiece.Empty, CheckerPiece.MinKing, CheckerPiece.Empty, CheckerPiece.MinKing, CheckerPiece.Empty, CheckerPiece.MinKing},
            { CheckerPiece.MinKing, CheckerPiece.Empty, CheckerPiece.MinKing, CheckerPiece.Empty, CheckerPiece.MinKing, CheckerPiece.Empty, CheckerPiece.MinKing, CheckerPiece.Empty},
        };

        public static CheckerPiece[,] GetEmptyBoard() => GetEmptyBoard(8);

        public static CheckerPiece[,] GetEmptyBoard(int size) => new CheckerPiece[size, size];

        public static SearchEngine GetCheckersSearchEngine(int maxDegreeOfParallelism = 1, ParallelismMode parallelismMode = ParallelismMode.NonParallelism, int levelOfParallelism = 1, bool dieEarly = true, CacheMode cacheMode = CacheMode.NewCache) =>
            new SearchEngine(cacheMode)
            {
                MaxDegreeOfParallelism = maxDegreeOfParallelism,
                MaxLevelOfParallelism = levelOfParallelism,
                DieEarly = dieEarly,
                MinScore = CheckersState.MIN_WIN,
                MaxScore = CheckersState.MAX_WIN,
                ParallelismMode = parallelismMode,
                SkipEvaluationForFirstNodeSingleNeighbor = false,
            };
        
        public static bool AreEquale(CheckerPiece[,] board1, CheckerPiece[,] board2)
        {
            if (board1.GetLength(0) != board2.GetLength(0) || board1.GetLength(1) != board2.GetLength(1))
                return false;

            for (int i = 0; i < board1.GetLength(0); i++)
            for (int j = 0; j < board1.GetLength(1); j++)
                if (board1[i, j] != board2[i, j])
                    return false;

            return true;
        }

        public static bool Contains(this IEnumerable<IState> boards, CheckerPiece[,] board) =>
            boards.Any(s => AreEquale(s.GetBoard(), board));

        public static CheckerPiece[,] GetBoard(this IState state) =>
            ((CheckersState) state).Board;

        public static CheckerPiece ToKing(this Player player) =>
            player == Player.Max ? CheckerPiece.MaxKing : CheckerPiece.MinKing;
    }
}
