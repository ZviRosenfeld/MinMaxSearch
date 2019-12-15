using System;
using MinMaxSearch;

namespace CheckersTests
{
    static class Utils
    {
        public static CheckerPiece[,] GetStartBoard()
        {
            return new[,]
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
        }

        public static CheckerPiece[,] Move(this CheckerPiece[,] board, CheckerPiece pieceToMove, int i_from, int j_from, int i_to, int j_to, bool isJump)
        {
            var newBoard = (CheckerPiece[,]) board.Clone();
            newBoard[i_from, j_from] = CheckerPiece.Empty;
            newBoard[i_to, j_to] = pieceToMove;
            if (isJump)
                newBoard[(i_from + i_to) / 2, (j_from + j_to) / 2] = CheckerPiece.Empty;

            // King pieces if needed
            if (pieceToMove == CheckerPiece.MaxChecker && j_to == newBoard.GetLength(1))
                newBoard[i_to, j_to] = CheckerPiece.MaxKing;
            if (pieceToMove == CheckerPiece.MinChecker && j_to == 0)
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
    }
}
