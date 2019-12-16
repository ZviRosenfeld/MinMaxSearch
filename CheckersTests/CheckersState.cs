using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinMaxSearch;

namespace CheckersTests
{
    class CheckersState : IDeterministicState
    {
        public const int MAX_WIN = 100;
        public const int MIN_WIN = -100;

        public CheckersState(CheckerPiece[,] board, Player turn)
        {
            Turn = turn;
            Board = board;
        }

        /// <summary>
        /// This is a very simple evaluation method. 
        /// It probablly won't work well against a human player, but it's very good for tests.
        /// </summary>
        public double Evaluate(int depth, List<IState> passedThroughStates)
        {
            var sum = 0;
            bool isMaxWin = true, isMinWin = true;
            foreach (var cell in Board)
            {
                if (cell == CheckerPiece.MaxChecker)
                {
                    sum += 1;
                    isMinWin = false;
                }
                if (cell == CheckerPiece.MinChecker)
                {
                    sum -= 1;
                    isMaxWin = false;
                }
                if (cell == CheckerPiece.MaxKing)
                {
                    sum += 3;
                    isMinWin = false;
                }
                if (cell == CheckerPiece.MinKing)
                {
                    sum -= 3;
                    isMaxWin = false;
                }
            }

            if (isMinWin)
                return -100;
            if (isMaxWin)
                return 100;
            return sum;
        }

        private bool IsWin(Player player) => 
            Board.Cast<CheckerPiece>().All(cell => !cell.IsSameColor(player));

        public Player Turn { get; }

        public CheckerPiece[,] Board { get; }

        public IEnumerable<IState> GetNeighbors()
        {
            if (IsWin(Turn.GetReversePlayer()))
                return new List<IState>();

            return new NextMovesGenerator(Board, Turn).GenerateNextMoves();
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            for (int i = 0; i < Board.GetLength(0); i++)
            {
                for (int j = 0; j < Board.GetLength(1); j++)
                    builder.Append(Utils.ToString(Board[i, j]) + " ");
                builder.Append("#" + Environment.NewLine);
            }

            return builder.ToString();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CheckersState checkersState))
                return false;

            if (Turn != checkersState.Turn)
                return false;

            for (var i = 0; i < Board.GetLength(0); i++)
            for (var j = 0; j < Board.GetLength(1); j++)
                if (Board[i, j] != checkersState.Board[i, j])
                    return false;

            return true;
        }

        public override int GetHashCode()
        {
            int sum = 0;

            for (var i = 0; i < Board.GetLength(0); i++)
            for (var j = 0; j < Board.GetLength(1); j++)
                sum += GetIntValue(Board[i, j]) * (int)Math.Pow(5, i + j);

            return sum + (int)Turn * (int)Math.Pow(3, Board.GetLength(0) * Board.GetLength(1));
        }

        public int GetIntValue(CheckerPiece piece)
        {
            switch (piece)
            {
                case CheckerPiece.Empty: return 0;
                case CheckerPiece.MaxChecker: return 1;
                case CheckerPiece.MaxKing: return 2;
                case CheckerPiece.MinChecker: return 3;
                case CheckerPiece.MinKing: return 4;
            }

            throw new Exception("We should never get here!");
        }
    }
}
