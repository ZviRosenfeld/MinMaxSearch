using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinMaxSearch
{
    class ProbabilisticSearchUtils
    {
        private readonly ThreadManager threadManager;
        private readonly DeterministicSearchUtils deterministicSearchUtils;
        private readonly SearchOptions searchOptions;

        public ProbabilisticSearchUtils(ThreadManager threadManager, DeterministicSearchUtils deterministicSearchUtils, SearchOptions searchOptions)
        {
            this.deterministicSearchUtils = deterministicSearchUtils;
            this.searchOptions = searchOptions;
            this.threadManager = threadManager;
        }

        public SearchResult EvaluateChildren(IProbabilisticState startState, SearchContext searchContext)
        {
            if (!startState.GetNeighbors().Any())
            {
                var evaluation = startState.Evaluate(searchContext.CurrentDepth, searchContext.StatesUpTillNow, searchContext.StartPlayer, searchOptions);
                return new SearchResult(evaluation, new List<IState> {startState}, 1, 0, true);
            }

            var storedStates = new ConcurrentDictionary<IState, double>();
            var results = new List<Tuple<double, Task<SearchResult>>>();
            foreach (var neighbor in startState.GetNeighbors())
            {
                var wrappedState = new ProbablisticStateWrapper(neighbor.Item2, startState);
                var searchResult = threadManager.Invoke(() =>
                    deterministicSearchUtils.EvaluateChildren(wrappedState, searchContext.CloneWithMaxAlphaAndBeta(), storedStates));
                results.Add(new Tuple<double, Task<SearchResult>>(neighbor.Item1, searchResult));
            }
            
            return Reduce(results, startState);
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
