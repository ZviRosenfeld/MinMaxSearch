using System;
using System.Collections.Generic;
using System.Linq;
using MinMaxSearch;

namespace CheckersTests
{
    class NextMovesGenerator
    {
        private readonly CheckerPiece[,] board;
        private readonly Player turn;

        public NextMovesGenerator(CheckerPiece[,] board, Player turn)
        {
            this.board = board;
            this.turn = turn;
        }

        // For this class we mark cells as [i, j].
        // Cells of type [0, x] will king Min, while cells of type [7, x] will king Max
        // This means that Max is moving up (i is increasing), and Min is moving down (i decreasing)

        public IEnumerable<CheckersState> GenerateNextMoves()
        {
            List<CheckersState> nextMoves = new List<CheckersState>();

            for (int i = 0; i < board.GetLength(0); i++)
            for (int j = 0; j < board.GetLength(1); j++)
            {
                var piece = board[i, j];
                if (!piece.IsSameColor(turn)) continue;

                nextMoves.AddRange(GetMoves(piece, i, j, true));   
            }
            if (nextMoves.Any()) // Only add regualer moves if we can't jump
                return nextMoves;

            for (int i = 0; i < board.GetLength(0); i++)
            for (int j = 0; j < board.GetLength(1); j++)
            {
                var piece = board[i, j];
                if (!piece.IsSameColor(turn)) continue;

                nextMoves.AddRange(GetMoves(piece, i, j, false));
            }

            return nextMoves;
        }

        private List<CheckersState> GetMoves(CheckerPiece piece, int i, int j, bool isJump)
        {
            var nextMoves = new List<CheckersState>();
            if (piece == CheckerPiece.MaxChecker)
                nextMoves.AddRange(MovePieceUp(i, j, piece, isJump));
            if (piece == CheckerPiece.MinChecker)
                nextMoves.AddRange(MovePieceDown(i, j, piece, isJump));
            if (piece == CheckerPiece.MaxKing || piece == CheckerPiece.MinKing)
                nextMoves.AddRange(MovePieceUpAndDown(i, j, piece, isJump));

            return nextMoves;
        }

        private List<CheckersState> MovePieceUpAndDown(int i, int j, CheckerPiece piece, bool isJump)
        {
            var nextMoves = MovePieceUp(i, j, piece, isJump);
            nextMoves.AddRange(MovePieceDown(i, j, piece, isJump));
            return nextMoves;
        }

        private List<CheckersState> MovePieceUp(int i, int j, CheckerPiece piece, bool isJump) =>
            MovePiece(i, j, piece, isJump, 1);

        private List<CheckersState> MovePieceDown(int i, int j, CheckerPiece piece, bool isJump) =>
            MovePiece(i, j, piece, isJump, -1);

        private List<CheckersState> MovePiece(int i, int j, CheckerPiece piece, bool isJump, int direction)
        {
            var moveBy = isJump ? 2 : 1;
            var nextMoves = new List<CheckersState>();
            if (CanMove(i, j, i + direction * moveBy, j + moveBy, isJump))
            {
                var nextBoard = board.Move(piece, i, j, i + direction * moveBy, j + moveBy, isJump);
                nextMoves.Add(nextBoard.ToState(turn.GetReversePlayer()));
                if (isJump) // Add double jumps
                    nextMoves.AddRange(MovePiece(i + direction * moveBy, j + moveBy, piece, isJump, direction));
            }
            if (CanMove(i, j, i + direction * moveBy, j - moveBy, isJump))
            {
                var nextBoard = board.Move(piece, i, j, i + direction * moveBy, j - moveBy, isJump);
                nextMoves.Add(nextBoard.ToState(turn.GetReversePlayer()));
                if (isJump) // Add double jumps
                    nextMoves.AddRange(MovePiece(i + direction * moveBy, j - moveBy, piece, isJump, direction));
            }

            return nextMoves;
        }

        private bool CanMove(int from_i, int from_j, int to_i, int to_j, bool isJump)
        {
            if (!(IsInBoardBounds(to_i) && IsInBoardBounds(to_j) && IsEmpty(to_i, to_j)))
                return false;
            if (!isJump)
                return true;

            var pieceToJumpOver = board[(from_i + to_i) / 2, (from_j + to_j) / 2];
            return pieceToJumpOver.IsSameColor(turn.GetReversePlayer());
        }

        private bool IsInBoardBounds(int value) => value >= 0 && value < 8;
        
        private bool IsEmpty(int i, int j) => board[i, j] == CheckerPiece.Empty;
    }
}
