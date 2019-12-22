using System;
using System.Linq;

namespace MinMaxSearch
{
    public static class GameUtils
    {
        public static Player GetReversePlayer(this Player player)
        {
            if (player == Player.Empty)
                throw new ArgumentException("No reverse for " + nameof(Player.Empty));

            return player == Player.Max ? Player.Min : Player.Max;
        }

        public static ProbablisticStateWrapper GetNextState(this IProbabilisticState probabilisticState)
        {
            var sumOfAllPossibilities = probabilisticState.GetNeighbors().Select(t => t.Item1).Sum();
            var num = GetRandomNumberUpTo(sumOfAllPossibilities);
            var sum = 0.0;
            foreach (var neighbor in probabilisticState.GetNeighbors())
            {
                sum += neighbor.Item1;
                if (sum >= num)
                    return new ProbablisticStateWrapper(neighbor.Item2, probabilisticState);
            }

            return null;
        }

        private static double GetRandomNumberUpTo(double max)
        {
            var random = new Random();
            var num = random.NextDouble();
            return num * max;
        }
    }
}
