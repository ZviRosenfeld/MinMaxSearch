using System;
using System.Collections.Generic;
using System.Text;
using MinMaxSearch;

namespace Connect4Tests
{
    public class Connect4State : IDeterministicState
    {
        public const int BoardSize = 6;

        public Connect4State(Player[,] board, Player turn)
        {
            Board = board;
            Turn = turn;
        }

        public Player[,] Board { get; }
        public Player Turn { get; }

        public IEnumerable<IState> GetNeighbors()
        {
            if (BoardEvaluator.IsWin(Board, Utils.GetReversePlayer(Turn)))
                return new List<IDeterministicState>();

            var result = new List<Connect4State>();
            for (int i = 0; i < BoardSize; i++)
            {
                var newState = AddPieceTo(i);
                if (newState != null)
                    result.Add(newState);
            }

            return result;
        }

        public double Evaluate(int depth, List<IState> passedThroughStates) =>
            BoardEvaluator.Evaluate(Board);

        public Connect4State AddPieceTo(int i)
        {
            var newBoard = (Player[,]) Board.Clone();
            for (int j = 0; j < BoardSize; j ++)
                if (newBoard[j, i] == Player.Empty)
                {
                    newBoard[j, i] = Turn;
                    return new Connect4State(newBoard, Utils.GetReversePlayer(Turn));
                }

            return null;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Connect4State ticTacToeState))
                return false;

            if (Turn != ticTacToeState.Turn)
                return false;

            for (var i = 0; i < BoardSize; i++)
            for (var j = 0; j < BoardSize; j++)
                if (Board[i, j] != ticTacToeState.Board[i, j])
                    return false;

            return true;
        }

        public override int GetHashCode()
        {
            int sum = 0;

            for (var i = 0; i < BoardSize; i++)
            for (var j = 0; j < BoardSize; j++)
                sum += GetValue(Board[i, j]) * (int)Math.Pow(3, i + j);

            return sum + (int) Turn * (int) Math.Pow(3, BoardSize * BoardSize);
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

            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                    builder.Append(GetValue(Board[i,j]) + " ");
                builder.Append("#" + Environment.NewLine);
            }

            return builder.ToString();
        }
    }
}
