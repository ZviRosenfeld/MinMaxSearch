using System;
using System.Collections.Generic;
using System.Text;
using MinMaxSearch;

namespace TicTacToeTests
{
    public class TicTacToeState : IDeterministicState 
    {
        public Player[,] Board { get; }
        public Player Turn { get; }

        public const int MaxValue = 1;
        public const int MinValue = -1;

        public TicTacToeState(Player[,] board, Player turn)
        {
            Board = board;
            Turn = turn;
        }

        public double Evaluate(int depth, List<IState> passedThroughStates)
        {
            if (IsDiagonalWin(Player.Max) || IsStraightWin(Player.Max))
                return MaxValue;

            if (IsDiagonalWin(Player.Min) || IsStraightWin(Player.Min))
                return MinValue;

            return 0;
        }

        public virtual IEnumerable<IState> GetNeighbors()
        {
            var stateEvaluation = Evaluate(0, new List<IState>());
            if (stateEvaluation >= MaxValue || stateEvaluation <= MinValue)
                return new List<IDeterministicState>();

            var neighbors = new List<TicTacToeState>();
            for (var i = 0; i < 3; i++)
            for (var j = 0; j < 3; j++)
                if (Board[i, j] == Player.Empty)
                {
                    var newBoard = (Player[,]) Board.Clone();
                    newBoard[i, j] = Turn;
                    neighbors.Add(new TicTacToeState(newBoard, Turn.GetReversePlayer()));
                }
            return neighbors;
        }

        private bool IsDiagonalWin(Player player)
        {
            if (Board[0, 0] == player && Board[1, 1] == player && Board[2, 2] == player)
                return true;
            if (Board[0, 2] == player && Board[1, 1] == player && Board[2, 0] == player)
                return true;

            return false;
        }

        private bool IsStraightWin(Player player)
        {
            for (var i = 0; i < 3; i++)
            {
                if (Board[0, i] == player && Board[1, i] == player && Board[2, i] == player)
                    return true;
                if (Board[i, 0] == player && Board[i, 1] == player && Board[i, 2] == player)
                    return true;
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TicTacToeState ticTacToeState))
                return false;

            if (Turn != ticTacToeState.Turn)
                return false;

            for (var i = 0; i < 3; i++)
            for (var j = 0; j < 3; j++)
                if (Board[i, j] != ticTacToeState.Board[i, j])
                    return false;

            return true;
        }

        public override int GetHashCode()
        {
            int sum = 0;

            for (var i = 0; i < 3; i++)
            for (var j = 0; j < 3; j++)
                sum += GetValue(Board[i, j]) * (int) Math.Pow(3, i + j * 3);

            return sum + (int)Turn * (int) Math.Pow(3, 9);
        }

        private int GetValue(Player player)
        {
            switch (player)
            {
                case Player.Min:
                    return 1;
                case Player.Max:
                    return 2;
                default:
                    return 0;
            }
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                    builder.Append(GetValue(Board[i, j]) + " ");
                builder.Append("#" + Environment.NewLine);
            }

            return builder.ToString();
        }
    }
}
