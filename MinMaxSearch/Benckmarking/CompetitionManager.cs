using System;
using System.Collections.Generic;
using System.Linq;

namespace MinMaxSearch.Benckmarking
{
    public static class CompetitionManager
    {
        public static CompetitionResult Compete(this SearchEngine engine, IDeterministicState startState, int searchDepth, int maxPayDepth) =>
            Compete(engine, engine, startState, searchDepth, maxPayDepth);

        public static CompetitionResult Compete(SearchEngine maxEngine, SearchEngine minEngine, IDeterministicState startState, int searchDepth, int maxPayDepth)
        {
            var currentState = startState;
            var states = new List<IState>();
            var depth = 0;
            TimeSpan maxTime = TimeSpan.Zero, minTime = TimeSpan.Zero;
            while (ContainsNeigbors(currentState) && depth < maxPayDepth)
            {
                var startTime = DateTime.Now;
                var searchResult = currentState.Turn == Player.Max ? 
                    maxEngine.Search(currentState, searchDepth) :
                    minEngine.Search(currentState, searchDepth);
                var runTime = DateTime.Now - startTime;
                if (currentState.Turn == Player.Max)
                    maxTime += runTime;
                else
                    minTime += runTime;
                
                states.Add(searchResult.NextMove);
                depth++;

                currentState = (IDeterministicState) searchResult.NextMove;
            }

            return new CompetitionResult(currentState, depth, states, maxTime, minTime);
        }

        private static bool ContainsNeigbors(IState state)
        {
            if (state is IDeterministicState deterministicState && deterministicState.GetNeighbors().Any())
                return true;

            if (state is IProbabilisticState probabilisticState)
                return probabilisticState.GetNeighbors().Any() && probabilisticState.GetNeighbors().Any(neighbor => neighbor.Item2.Any());

            return false;
        }
    }
}
