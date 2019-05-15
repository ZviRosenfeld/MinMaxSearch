using System;
using System.Collections.Generic;
using System.Linq;

namespace MinMaxSearch.Benckmarking
{
    public static class CompetitionManager
    {
        /// <summary>
        /// With this method you can simulate a complete game and compare different evaluation-strategies.
        /// Use IAlternateEvaluationState to test different evaluation-strategies. 
        /// If your states implement IAlternateEvaluationState then any time min started the search we'll use the IAlternateEvaluationState.AlternateEvaluation
        /// to evaluate the states rather then the IState.Evaluation method.
        /// </summary>
        public static CompetitionResult Compete(this SearchEngine engine, IDeterministicState startState, int searchDepth, int maxPayDepth) =>
            Compete(engine, engine, startState, searchDepth, searchDepth, maxPayDepth);

        /// <summary>
        /// With this method you can simulate a complete game and compare different engines, search-depths or evaluation-strategies.
        /// Use IAlternateEvaluationState to test different evaluation-strategies. 
        /// If your states implement IAlternateEvaluationState then any time min started the search we'll use the IAlternateEvaluationState.AlternateEvaluation
        /// to evaluate the states rather then the IState.Evaluation method.
        /// </summary>
        /// <param name="maxEngine"> The engine to use for max</param>
        /// <param name="minEngine"> The engine to use for min</param>
        /// <param name="startState"> The starting sate</param>
        /// <param name="maxSearchDepth"> How deep should max search</param>
        /// <param name="minSearchDepth"> How deep should min search</param>
        /// <param name="maxPayDepth"> After how many moves should we terminate the game if no one won</param>
        public static CompetitionResult Compete(SearchEngine maxEngine, SearchEngine minEngine, IDeterministicState startState, int maxSearchDepth, int minSearchDepth, int maxPayDepth)
        {
            var currentState = startState;
            var states = new List<IState>();
            var depth = 0;
            TimeSpan maxTime = TimeSpan.Zero, minTime = TimeSpan.Zero;
            while (ContainsNeigbors(currentState) && depth < maxPayDepth)
            {
                var startTime = DateTime.Now;
                var searchResult = currentState.Turn == Player.Max ? 
                    maxEngine.Search(currentState, maxSearchDepth) :
                    minEngine.Search(currentState, minSearchDepth);
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
