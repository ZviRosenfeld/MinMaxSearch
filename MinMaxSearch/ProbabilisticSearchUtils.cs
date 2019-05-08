using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MinMaxSearch
{
    class ProbabilisticSearchUtils
    {
        private readonly SearchOptions searchOptions;
        private readonly SearchWorker searchWorker;
        private readonly ThreadManager threadManager;
        private readonly DeterministicSearchUtils deterministicSearchUtils;

        public ProbabilisticSearchUtils(SearchWorker searchWorker, SearchOptions searchOptions, ThreadManager threadManager, DeterministicSearchUtils deterministicSearchUtils)
        {
            this.searchWorker = searchWorker;
            this.searchOptions = searchOptions;
            this.deterministicSearchUtils = deterministicSearchUtils;
            this.threadManager = threadManager;
        }

        public SearchResult EvaluateChildren(IProbabilisticState startState, int depth, CancellationToken cancellationToken, List<IState> statesUpToNow)
        {
            if (!startState.GetNeighbors().Any())
                return new SearchResult(startState.Evaluate(depth, statesUpToNow), new List<IState> {startState}, 1, 0, true);

            if (searchWorker.DeadEndStates.ContainsKey(startState))
                return new SearchResult(searchWorker.DeadEndStates[startState].Item1, new List<IState> {startState}, 1, 0, true);


            var storedStates = new ConcurrentDictionary<IState, double>();
            var results = new List<Tuple<double, Task<SearchResult>>>();
            foreach (var neighbor in startState.GetNeighbors())
            {
                var wrappedState = new ProbablisticStateWrapper(neighbor.Item2, startState);       
                var searchResult = threadManager.Invoke(() =>
                    deterministicSearchUtils.EvaluateChildren(wrappedState, depth, double.MinValue, double.MaxValue, cancellationToken, statesUpToNow, storedStates));
                results.Add(new Tuple<double, Task<SearchResult>>(neighbor.Item1, searchResult));
            }

            var result = Reduce(results, startState);
            if (result.AllChildrenAreDeadEnds)
                searchWorker.DeadEndStates[startState] = new Tuple<double, List<IState>>(result.Evaluation, new List<IState>(result.StateSequence));

            return result;
        }

        private SearchResult Reduce(List<Tuple<double, Task<SearchResult>>> results, IState startState)
        {
            double sum = 0;
            int leaves = 0, internalNodes = 0;
            var allChildrenAreDeadEnds = true;
            foreach (var result in results)
            {
                var searchResult = result.Item2.Result;
                sum += result.Item1 * searchResult.Evaluation;
                leaves += searchResult.Leaves;
                internalNodes += searchResult.InternalNodes;
                allChildrenAreDeadEnds = allChildrenAreDeadEnds && searchResult.AllChildrenAreDeadEnds;
            }

            return new SearchResult(sum, new List<IState>() {startState}, leaves, internalNodes, allChildrenAreDeadEnds);
        }
    }
}
