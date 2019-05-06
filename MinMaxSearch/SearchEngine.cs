﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MinMaxSearch.Pruners;

namespace MinMaxSearch
{
    public class SearchEngine
    {
        private readonly List<IPruner> pruners = new List<IPruner>();
        private readonly PreventLoopPruner preventLoopPruner = new PreventLoopPruner();
        private readonly IDictionary<IState, Tuple<double, List<IState>>> deadEndStates = new ConcurrentDictionary<IState, Tuple<double, List<IState>>>();

        public void AddPruner(IPruner pruner) => pruners.Add(pruner);

        /// <summary>
        /// At unstable states, we'll continue searching even after we've hit the maxDepth limit
        /// </summary>
        public Func<IState, int, List<IState>, bool> IsUnstableState { get; set; } = ((s, d, l) => false);
        
        /// <summary>
        /// Note that this will only work if you implement Equals and GetHashValue in a meaningful way in the states. 
        /// </summary>
        public bool PreventLoops {
            get => pruners.Contains(preventLoopPruner);
            set
            {
                if (value && !pruners.Contains(preventLoopPruner))
                    pruners.Add(preventLoopPruner);
                if (!value && pruners.Contains(preventLoopPruner))
                    pruners.Remove(preventLoopPruner);
            }
        }

        /// <summary>
        /// If two path give the same score, but one is shorter then the other - we'll take the shorter one
        /// </summary>
        public bool FavorShortPaths { get; set; } = true;

        /// <summary>
        /// The search will end once we find a score better then MaxScore for Max or worse then MinScore for Min
        /// </summary>
        public bool DieEarly { get; set; }

        /// <summary>
        /// If true, the engine will remember states from which all children lead to endStates, so that it won't need to re-calculate their search-tree. 
        /// This can save a lot of time in some games.
        /// Note that this will only work if the state overrides object's Equals and GetHashCode methods in a meaningful way.
        /// </summary>
        public bool RememberDeadEndStates { get; set; }

        public double MaxScore { get; set; } = double.MaxValue;

        public double MinScore { get; set; } = double.MinValue;

        private int maxDegreeOfParallelism = 1;
        public int MaxDegreeOfParallelism
        {
            get => maxDegreeOfParallelism;
            set => maxDegreeOfParallelism = value > 0
                ? value
                : throw new BadDegreeOfParallelismException("DegreeOfParallelism must be at least one");
        }
        
        public SearchResult Search(IDeterministicState startState, int maxDepth) =>
            Search(startState, maxDepth, CancellationToken.None);

        public Task<SearchResult> SearchAsync(IDeterministicState startState, int maxDepth, CancellationToken cancellationToken) => 
            Task.Run(() => Search(startState, maxDepth, cancellationToken));

        public SearchResult Search(IDeterministicState startState, int maxDepth, CancellationToken cancellationToken)
        {
            if (!startState.GetNeighbors().Any())
                throw new NoNeighborsException("start state has no nighbors " + startState);

            var searchWorker = new SearchWorker(maxDepth, this, pruners, deadEndStates);
            var evaluation = searchWorker.Evaluate(startState, 0, double.MinValue, double.MaxValue, cancellationToken, new List<IState>());
            evaluation.StateSequence.Reverse();
            evaluation.StateSequence.RemoveAt(0); // Removing the top node will make the result "nicer"
            return evaluation;
        }

        public SearchResult IterativeSearch(IDeterministicState startState, int startDepth, int maxDepth, CancellationToken cancellationToken)
        {
            if (startDepth >= maxDepth)
                throw new Exception($"{nameof(startDepth)} (== {startDepth}) must be bigger than {nameof(maxDepth)} ( == {maxDepth})");

            SearchResult bestResultSoFar = null;
            for (int i = startDepth; i < maxDepth; i++)
            { 
                var result = Search(startState, i, cancellationToken);
                if (!cancellationToken.IsCancellationRequested || bestResultSoFar == null)
                    bestResultSoFar = result;
                if (cancellationToken.IsCancellationRequested)
                    break;
            }
            return bestResultSoFar;
        }

    }
}
