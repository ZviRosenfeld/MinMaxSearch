using System;
using System.Collections.Generic;
using System.Linq;
using MinMaxSearch;

namespace CheckersTests
{
    static class Utils
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

        public static SearchEngine GetCheckersSearchEngine(int maxDegreeOfParallelism = 1, ParallelismMode parallelismMode = ParallelismMode.NonParallelism, int levelOfParallelism = 1, bool dieEarly = true) =>
            new SearchEngine()
            {
                MaxDegreeOfParallelism = maxDegreeOfParallelism,
                MaxLevelOfParallelism = levelOfParallelism,
                DieEarly = dieEarly,
                MinScore = CheckersState.MIN_WIN,
                MaxScore = CheckersState.MAX_WIN,
                ParallelismMode = parallelismMode,
                SkipEvaluationForFirstNodeSingleNeighbor = false
            };

        public static CheckerPiece[,] Move(this CheckerPiece[,] board, CheckerPiece pieceToMove, int i_from, int j_from, int i_to, int j_to, bool isJump)
        {
            var newBoard = (CheckerPiece[,]) board.Clone();
            newBoard[i_from, j_from] = CheckerPiece.Empty;
            newBoard[i_to, j_to] = pieceToMove;
            if (isJump)
                newBoard[(i_from + i_to) / 2, (j_from + j_to) / 2] = CheckerPiece.Empty;

            // King pieces if needed
            if (pieceToMove == CheckerPiece.MaxChecker && i_to == newBoard.GetLength(0) - 1)
                newBoard[i_to, j_to] = CheckerPiece.MaxKing;
            if (pieceToMove == CheckerPiece.MinChecker && i_to == 0)
                newBoard[i_to, j_to] = CheckerPiece.MinKing;

            return newBoard;
        }

        public static string ToString(CheckerPiece piece)
        {
            switch (piece)
            {
                case CheckerPiece.Empty: return "Empty";
                case CheckerPiece.MaxChecker: return "MaxC";
                case CheckerPiece.MaxKing: return "MaxK";
                case CheckerPiece.MinChecker: return "MinC";
                case CheckerPiece.MinKing: return "MinK";
            }

            throw new Exception("We should never get here!");
        }

        public static CheckersState ToState(this CheckerPiece[,] board, Player turn) =>
            new CheckersState(board, turn);

        public static bool IsSameColor(this CheckerPiece piece, Player player) =>
            (player == Player.Max && (piece == CheckerPiece.MaxKing || piece == CheckerPiece.MaxChecker)) ||
            (player == Player.Min && (piece == CheckerPiece.MinKing || piece == CheckerPiece.MinChecker));

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
