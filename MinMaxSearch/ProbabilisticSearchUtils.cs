﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MinMaxSearch
{
    class ProbabilisticSearchUtils
    {
        private readonly ThreadManager threadManager;
        private readonly DeterministicSearchUtils deterministicSearchUtils;

        public ProbabilisticSearchUtils(ThreadManager threadManager, DeterministicSearchUtils deterministicSearchUtils)
        {
            this.deterministicSearchUtils = deterministicSearchUtils;
            this.threadManager = threadManager;
        }

        public SearchResult EvaluateChildren(IProbabilisticState startState, int depth, CancellationToken cancellationToken, List<IState> statesUpToNow)
        {
            if (!startState.GetNeighbors().Any())
                return new SearchResult(startState.Evaluate(depth, statesUpToNow), new List<IState> {startState}, 1, 0);

            var storedStates = new ConcurrentDictionary<IState, double>();
            var results = new List<Tuple<double, Task<SearchResult>>>();
            foreach (var neighbor in startState.GetNeighbors())
            {
                var wrappedState = new ProbablisticStateWrapper(neighbor.Item2, startState);       
                var searchResult = threadManager.Invoke(() =>
                    deterministicSearchUtils.EvaluateChildren(wrappedState, depth, double.MinValue, double.MaxValue, cancellationToken, statesUpToNow, storedStates));
                results.Add(new Tuple<double, Task<SearchResult>>(neighbor.Item1, searchResult));
            }

            return Reduce(results, startState);
        }

        private SearchResult Reduce(List<Tuple<double, Task<SearchResult>>> results, IState startState)
        {
            double sum = 0;
            int leaves = 0, internalNodes = 0;
            foreach (var result in results)
            {
                var searchResult = result.Item2.Result;
                sum += result.Item1 * searchResult.Evaluation;
                leaves += searchResult.Leaves;
                internalNodes += searchResult.InternalNodes;
            }

            return new SearchResult(sum, new List<IState>() {startState}, leaves, internalNodes);
        }
    }
}