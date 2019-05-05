using MinMaxSearch;

namespace Connect4Tests
{
    public static class BoardEvaluator
    {
        public const int MaxEvaluation = 100000;
        public const int MinEvaluation = -100000;

        public static bool IsWin(Player[,] board, Player player)
        {
            return IsWinInDirection(board, 0, 1, player) ||
                IsWinInDirection(board, 1, 0, player) ||
                IsWinInDirection(board, 1, 1, player) ||
                IsWinInDirection(board, -1, 1, player);
        }

        private static bool IsWinInDirection(Player[,] board, int direction1, int direction2, Player player)
        {
            for (int i = 0; i < Connect4State.BoardSize; i++)
            for (int j = 0; j < Connect4State.BoardSize; j++)
                if (PiecesInRow(board, i, j, direction1, direction2, player) == 4)
                    return true;

            return false;
        }

        public static int Evaluate(Player[,] board)
        {
            var maxPoints = EvaluateDirection(board, 0, 1, Player.Max) +
                            EvaluateDirection(board, 1, 0, Player.Max) +
                            EvaluateDirection(board, 1, 1, Player.Max) +
                            EvaluateDirection(board, -1, 1, Player.Max);
            if (maxPoints >= MaxEvaluation)
                return MaxEvaluation;

            var minPoints = EvaluateDirection(board, 0, 1, Player.Min) +
                            EvaluateDirection(board, 1, 0, Player.Min) +
                            EvaluateDirection(board, 1, 1, Player.Min) +
                            EvaluateDirection(board, -1, 1, Player.Min);
            if (minPoints >= MaxEvaluation)
                return MinEvaluation;

            return maxPoints - minPoints;
        }

        private static int EvaluateDirection(Player[,] board, int direction1, int direction2, Player player)
        {
            var sum = 0;
            for (int i = 0; i < Connect4State.BoardSize; i++)
            for (int j = 0; j < Connect4State.BoardSize; j++)
            {
                var piecesInRow = PiecesInRow(board, i, j, direction1, direction2, player);
                if (piecesInRow == 4)
                    return MaxEvaluation;
                if (piecesInRow == 3)
                    sum += 20;
                if (piecesInRow == 2)
                    sum += 10;
                sum += piecesInRow;
            }

            return sum;
        }

        private static int PiecesInRow(Player[,] board, int startX, int startY, int direction1, int direction2, Player player)
        {
            var pieces = 0;
            for (int i = 0; i < 4; i++)
            {
                // out of range
                if (startX + i * direction1 >= Connect4State.BoardSize || startY + i * direction2 >= Connect4State.BoardSize)
                    return 0;
                if (startX + i * direction1 < 0 || startY + i * direction2 < 0)
                    return 0;
                if (board[startX + i * direction1, startY + i * direction2] == player)
                    pieces++;
                else if (board[startX + i * direction1, startY + i * direction2] != Player.Empty)
                    return 0;
            }

            return pieces;
        }
    }
}
