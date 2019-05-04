using System;
using System.Collections.Generic;
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

        public SearchResult EvaluateChildren(IProbabilisticState startState, int depth, double alpha, double bata,
            CancellationToken cancellationToken, List<IState> statesUpToNow)
        {
            var results = new List<Tuple<double, Task<SearchResult>>>();
            foreach (var neighbor in startState.GetNeighbors())
            {
                var wrappedState = new ProbablisticStateWrapper(startState.Turn, neighbor.Item2);
                var searchResult = threadManager.Invoke(() =>
                    deterministicSearchUtils.EvaluateChildren(wrappedState, depth, alpha, bata, cancellationToken, statesUpToNow));
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
