﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MinMaxSearch.Benckmarking
{
    public static class CompetitionManager
    {
        /// <summary>
        /// With this method you can simulate a complete game and compare different evaluation-strategies.
        /// </summary>
        /// <param name="engine"> The engine to use</param>
        /// <param name="startState"> The starting sate</param>
        /// <param name="searchDepth"> How deep should we search</param>
        /// <param name="maxPayDepth"> After how many moves should we terminate the game if no one won</param>
        /// <param name="maxAlternateEvaluation"> Will be used to evaluate the board on max's turn in stead of the state's regaler Evaluate method</param>
        /// <param name="minAlternateEvaluation"> Will be used to evaluate the board on min's turn in stead of the state's regaler Evaluate method</param>
        public static CompetitionResult Compete(this SearchEngine engine, IDeterministicState startState,
            int searchDepth, Func<IState, int, List<IState>, double> maxAlternateEvaluation = null,
            Func<IState, int, List<IState>, double> minAlternateEvaluation = null, int maxPayDepth = int.MaxValue,
            CancellationToken? cancellationToken = null)
        {
            if (maxAlternateEvaluation == null && minAlternateEvaluation == null)
                throw new ArgumentException($"At least one of {nameof(maxAlternateEvaluation)} or {nameof(minAlternateEvaluation)} shouldn't be null");
            
            return Compete(engine, new SearchEngine(engine), startState, searchDepth, searchDepth, maxPayDepth, maxAlternateEvaluation, minAlternateEvaluation, cancellationToken);
        }

        /// <summary>
        /// With this method you can simulate a complete game and compare different search-depth or evaluation-strategies.
        /// </summary>
        /// <param name="engine"> The engine to use</param>
        /// <param name="startState"> The starting sate</param>
        /// <param name="playerMaxSearchDepth"> How deep should max search</param>
        /// <param name="playerMinSearchDepth"> How deep should min search</param>
        /// <param name="maxPayDepth"> After how many moves should we terminate the game if no one won</param>
        /// <param name="maxAlternateEvaluation"> Will be used to evaluate the board on max's turn in stead of the state's regaler Evaluate method</param>
        /// <param name="minAlternateEvaluation"> Will be used to evaluate the board on min's turn in stead of the state's regaler Evaluate method</param>
        public static CompetitionResult Compete(this SearchEngine engine, IDeterministicState startState,
            int playerMaxSearchDepth, int playerMinSearchDepth, Func<IState, int, List<IState>, double> maxAlternateEvaluation = null,
            Func<IState, int, List<IState>, double> minAlternateEvaluation = null, int maxPayDepth = int.MaxValue,
            CancellationToken? cancellationToken = null)
        {
            return Compete(engine, new SearchEngine(engine), startState, playerMaxSearchDepth, playerMinSearchDepth, maxPayDepth, maxAlternateEvaluation, minAlternateEvaluation, cancellationToken);
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
            Func<IState, int, List<IState>, double> minAlternateEvaluation = null, CancellationToken? cancellationToken = null)
        {
            maxEngine.AlternateEvaluation = maxAlternateEvaluation;
            minEngine.AlternateEvaluation = minAlternateEvaluation;

            var currentState = startState;
            var resultFactory = new CompetitionResultFactory();
            for (int i = 0; ContainsNeigbors(currentState) && i < maxPayDepth; i++)
            {
                if (cancellationToken.HasValue && cancellationToken.Value.IsCancellationRequested)
                    break;

                var startTime = DateTime.Now;
                var searchResult = currentState.Turn == Player.Max
                    ? maxEngine.Search(currentState, playerMaxSearchDepth, cancellationToken ?? CancellationToken.None)
                    : minEngine.Search(currentState, playerMinSearchDepth, cancellationToken ?? CancellationToken.None);
                var runTime = DateTime.Now - startTime;
                resultFactory.AddState(searchResult.NextMove);
                resultFactory.AddTime(runTime, currentState.Turn);

                currentState = GetNextState(searchResult.NextMove);
            }

            return resultFactory.GetCompetitionResult();
        }

        private static IDeterministicState GetNextState(IState state)
        {
            if (state is IDeterministicState deterministicState)
                return deterministicState;
            if (state is IProbabilisticState probabilisticState)
                return probabilisticState.GetNextState();

            throw new BadStateTypeException($"State must implement {nameof(IDeterministicState)} or {nameof(IProbabilisticState)}");
        }
        
        private static bool ContainsNeigbors(IState state)
        {
            if (state is IDeterministicState deterministicState && deterministicState.GetNeighbors().Any())
                return true;

            if (state is IProbabilisticState probabilisticState)
                return probabilisticState.GetNeighbors().Any() &&
                       probabilisticState.GetNeighbors().Any(neighbor => neighbor.Item2.Any());

            return false;
        }
    }
}
