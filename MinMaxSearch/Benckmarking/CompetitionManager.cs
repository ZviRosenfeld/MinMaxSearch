using System;
using System.Collections.Generic;
using System.Linq;

namespace MinMaxSearch.Benckmarking
{
    public static class CompetitionManager
    {
        /// <summary>
        /// With this method you can simulate a complete game and compare different evaluation-strategies.
        /// maxAlternateEvaluation will be used to evaluate the board on max's turn in stead of the state's regaler Evaluate method
        /// minAlternateEvaluation will be used to evaluate the board on min's turn in stead of the state's regaler Evaluate method 
        /// </summary>
        public static CompetitionResult Compete(this SearchEngine engine, IDeterministicState startState,
            int searchDepth, Func<IState, int, List<IState>, double> maxAlternateEvaluation = null,
            Func<IState, int, List<IState>, double> minAlternateEvaluation = null, int maxPayDepth = int.MaxValue)
        {
            if (maxAlternateEvaluation == null && minAlternateEvaluation == null)
                throw new ArgumentException($"At least one of {nameof(maxAlternateEvaluation)} or {nameof(minAlternateEvaluation)} shouldn't be null");
            
            return Compete(engine, engine, startState, searchDepth, searchDepth, maxPayDepth, maxAlternateEvaluation, minAlternateEvaluation);
        }

        /// <summary>
        /// With this method you can simulate a complete game and compare different evaluation-strategies.
        /// maxAlternateEvaluation will be used to evaluate the board on max's turn in stead of the state's regaler Evaluate method
        /// minAlternateEvaluation will be used to evaluate the board on min's turn in stead of the state's regaler Evaluate method 
        /// </summary>
        public static CompetitionResult Compete(this SearchEngine engine, IDeterministicState startState,
            int playerMaxSearchDepth, int playerMinSearchDepth, Func<IState, int, List<IState>, double> maxAlternateEvaluation = null,
            Func<IState, int, List<IState>, double> minAlternateEvaluation = null, int maxPayDepth = int.MaxValue)
        {
            if (maxAlternateEvaluation == null && minAlternateEvaluation == null)
                throw new ArgumentException($"At least one of {nameof(maxAlternateEvaluation)} or {nameof(minAlternateEvaluation)} shouldn't be null");

            return Compete(engine, engine, startState, playerMaxSearchDepth, playerMinSearchDepth, maxPayDepth, maxAlternateEvaluation, minAlternateEvaluation);
        }

        /// <summary>
        /// With this method you can simulate a complete game and compare different engines, search-depths or evaluation-strategies.
        /// </summary>
        /// <param name="maxEngine"> The engine to use for max</param>
        /// <param name="minEngine"> The engine to use for min</param>
        /// <param name="startState"> The starting sate</param>
        /// <param name="playerMaxSearchDepth"> How deep should max search</param>
        /// <param name="playerMinSearchDepth"> How deep should min search</param>
        /// <param name="maxPayDepth"> After how many moves should we terminate the game if no one won</param>
        /// <param name="maxAlternateEvaluation"> Will be used to evaluate the board on max's turn in stead of the state's regaler Evaluate method</param>
        /// <param name="minAlternateEvaluation"> Will be used to evaluate the board on min's turn in stead of the state's regaler Evaluate method</param>
        public static CompetitionResult Compete(SearchEngine maxEngine, SearchEngine minEngine,
            IDeterministicState startState, int playerMaxSearchDepth, int playerMinSearchDepth, 
            int maxPayDepth = int.MaxValue, Func<IState, int, List<IState>, double> maxAlternateEvaluation = null,
            Func<IState, int, List<IState>, double> minAlternateEvaluation = null)
        {
            maxEngine.MaxAlternateEvaluation = maxAlternateEvaluation;
            minEngine.MinAlternateEvaluation = minAlternateEvaluation;

            var currentState = startState;
            var states = new List<IState>();
            var depth = 0;
            TimeSpan maxTime = TimeSpan.Zero, minTime = TimeSpan.Zero;
            while (currentState != null && ContainsNeigbors(currentState) && depth < maxPayDepth)
            {
                var startTime = DateTime.Now;
                var searchResult = currentState.Turn == Player.Max
                    ? maxEngine.Search(currentState, playerMaxSearchDepth)
                    : minEngine.Search(currentState, playerMinSearchDepth);
                var runTime = DateTime.Now - startTime;
                if (currentState.Turn == Player.Max)
                    maxTime += runTime;
                else
                    minTime += runTime;

                states.Add(searchResult.NextMove);
                depth++;

                currentState = GetNextState(searchResult.NextMove);
            }

            return new CompetitionResult(depth, states, maxTime, minTime);
        }

        private static IDeterministicState GetNextState(IState state)
        {
            if (state is IDeterministicState deterministicState)
                return deterministicState;
            if (state is IProbabilisticState probabilisticState)
                return ChooseNeigbors(probabilisticState);

            throw new BadStateTypeException($"State must implement {nameof(IDeterministicState)} or {nameof(IProbabilisticState)}");
        }

        private static ProbablisticStateWrapper ChooseNeigbors(IProbabilisticState probabilisticState)
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

        private static double GetRandomNumberUpTo(double end)
        {
            var random = new Random();
            var num = random.NextDouble();
            return num * end;
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
