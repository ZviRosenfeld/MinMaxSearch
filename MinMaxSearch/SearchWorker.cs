﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MinMaxSearch.Pruners;

namespace MinMaxSearch
{
    class SearchWorker
    {
        private readonly int maxDepth;
        private readonly SearchEngine searchEngine;
        private readonly List<IPruner> pruners;
        private readonly ThreadManager threadManager;
        private readonly DeterministicSearchUtils deterministicSearchUtils;
        private readonly ProbabilisticSearchUtils probabilisticSearchUtils;

        public IDictionary<IState, Tuple<double, List<IState>>> DeadEndStates { get; }

        public SearchWorker(int maxDepth, SearchEngine searchEngine, List<IPruner> pruners, IDictionary<IState, Tuple<double, List<IState>>> deadEndStates)
        {
            DeadEndStates = deadEndStates;
            this.maxDepth = maxDepth;
            this.searchEngine = searchEngine;
            this.pruners = pruners;
            threadManager = new ThreadManager(searchEngine.MaxDegreeOfParallelism);
            deterministicSearchUtils = new DeterministicSearchUtils(this, searchEngine, threadManager);
            probabilisticSearchUtils = new ProbabilisticSearchUtils(this, searchEngine, threadManager, deterministicSearchUtils);
        }

        public SearchResult Evaluate(IState startState, int depth, double alpha, double bata,
            CancellationToken cancellationToken, List<IState> statesUpToNow)
        {
            if (startState.Turn == Player.Empty)
                throw new EmptyPlayerException(nameof(startState.Turn) + " can't be " + nameof(Player.Empty));
            
            if (ShouldStop(startState, depth, cancellationToken, statesUpToNow))
                return new SearchResult(startState.Evaluate(depth, statesUpToNow), new List<IState> {startState}, 1, 0, false);
            
            if (startState is IDeterministicState deterministicState)
                return deterministicSearchUtils.EvaluateChildren(deterministicState, depth, alpha, bata, cancellationToken, statesUpToNow);

            if (startState is IProbabilisticState probabilisticState)
                return probabilisticSearchUtils.EvaluateChildren(probabilisticState, depth, cancellationToken, statesUpToNow);
            
            throw new BadStateTypeException($"State must implement {nameof(IDeterministicState)} or {nameof(IProbabilisticState)}");
        }
        
        private bool ShouldStop(IState state, int depth, CancellationToken cancellationToken, List<IState> passedStates)
        {
            if (depth >= maxDepth && !searchEngine.IsUnstableState(state, depth, passedStates))
                return true;
            if (cancellationToken.IsCancellationRequested)
                return true;
            if (pruners.Any(pruner => pruner.ShouldPrune(state, depth, passedStates)))
                return true;

            return false;
        }
    }
}
