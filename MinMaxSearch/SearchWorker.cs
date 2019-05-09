﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MinMaxSearch
{
    class SearchWorker
    {
        private readonly int maxDepth;

        private readonly SearchOptions searchOptions;
        private readonly ThreadManager threadManager;
        private readonly DeterministicSearchUtils deterministicSearchUtils;
        private readonly ProbabilisticSearchUtils probabilisticSearchUtils;
        
        public SearchWorker(int maxDepth, SearchOptions searchOptions)
        {
            this.maxDepth = maxDepth;
            this.searchOptions = searchOptions;
            threadManager = new ThreadManager(searchOptions.MaxDegreeOfParallelism);
            deterministicSearchUtils = new DeterministicSearchUtils(this, searchOptions, threadManager);
            probabilisticSearchUtils = new ProbabilisticSearchUtils(this, threadManager, deterministicSearchUtils);
        }

        public SearchResult Evaluate(IState startState, int depth, double alpha, double bata,
            CancellationToken cancellationToken, List<IState> statesUpToNow)
        {
            if (startState.Turn == Player.Empty)
                throw new EmptyPlayerException(nameof(startState.Turn) + " can't be " + nameof(Player.Empty));

            if (searchOptions.Pruners.Any(pruner => pruner.ShouldPrune(startState, depth, statesUpToNow)))
                return new SearchResult(startState.Evaluate(depth, statesUpToNow), new List<IState> { startState }, 1, 0, true);

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
            if (depth >= maxDepth && !searchOptions.IsUnstableState(state, depth, passedStates))
                return true;
            if (cancellationToken.IsCancellationRequested)
                return true;
            
            return false;
        }
    }
}
