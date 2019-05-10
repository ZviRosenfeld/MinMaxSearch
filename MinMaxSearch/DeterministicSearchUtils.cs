using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MinMaxSearch
{
    class DeterministicSearchUtils
    {
        private readonly SearchOptions searchOptions;
        private readonly SearchWorker searchWorker;
        private readonly ThreadManager threadManager;

        public DeterministicSearchUtils(SearchWorker searchWorker, SearchOptions searchOptions, ThreadManager threadManager)
        {
            this.searchWorker = searchWorker;
            this.searchOptions = searchOptions;
            this.threadManager = threadManager;
        }

        public SearchResult EvaluateChildren(IDeterministicState startState, SearchContext searchContext,
            IDictionary<IState, double> storedStates = null)
        {
            if (!startState.GetNeighbors().Any())
                return new SearchResult(startState.Evaluate(searchContext.CurrentDepth, searchContext.StatesUpTillNow), new List<IState> {startState}, 1, 0, true);
            
            var pruned = false;
            var player = startState.Turn;
            var results = new List<Task<SearchResult>>();
            var cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(searchContext.CancellationToken);
            foreach (var state in startState.GetNeighbors())
            {
                var taskResult = storedStates != null && storedStates.ContainsKey(state)
                    ? Task.FromResult(new SearchResult(storedStates[state], new List<IState> {state}, 1, 0, true))
                    : threadManager.Invoke(() =>
                    {
                        var localSearchContext = new SearchContext(searchContext.MaxDepth,
                            searchContext.CurrentDepth + 1, searchContext.Alpha, searchContext.Bata,
                            cancellationSource.Token, new List<IState>(searchContext.StatesUpTillNow) {startState});
                        return searchWorker.Evaluate(state, localSearchContext);
                    });
                results.Add(taskResult);

                if (taskResult.Status == TaskStatus.RanToCompletion && taskResult.Result != null)
                {
                    var stateEvaluation = taskResult.Result;
                    if (storedStates != null)
                        storedStates[state] = stateEvaluation.Evaluation;
                    if (AlphaBataShouldPrune(searchContext.Alpha, searchContext.Bata, stateEvaluation.Evaluation, player))
                    {
                        pruned = true;
                        cancellationSource.Cancel();
                        break;
                    }
                    var shouldDieEarlly = ShouldDieEarlly(stateEvaluation.Evaluation, player, stateEvaluation.StateSequence.Count);
                    if (shouldDieEarlly.Item1 && shouldDieEarlly.Item2 == 0)
                    {
                        pruned = true;
                        cancellationSource.Cancel();
                        break;
                    }
                    else if (shouldDieEarlly.Item1)
                        searchContext.MaxDepth = shouldDieEarlly.Item2 + searchContext.CurrentDepth;
                    
                    UpdateAlphaAndBata(searchContext, stateEvaluation.Evaluation, player);
                }
            }
            
            return Reduce(results, player, startState, pruned);
        }

        private SearchResult Reduce(List<Task<SearchResult>> results, Player player, IState startState, bool pruned)
        {
            var bestEvaluation = player == Player.Max ? double.MinValue : double.MaxValue;
            SearchResult bestResult = null;
            int leaves = 0, internalNodes = 0;
            var allChildrenAreDeadEnds = true;
            foreach (var result in results)
            {
                var actualResult = result.Result;
                if (actualResult == null) continue;

                leaves += actualResult.Leaves;
                internalNodes += actualResult.InternalNodes;
                allChildrenAreDeadEnds = allChildrenAreDeadEnds && actualResult.AllChildrenAreDeadEnds;
                if (IsBetterThen(actualResult.Evaluation, bestEvaluation, actualResult.StateSequence.Count,
                    bestResult?.StateSequence?.Count, player))
                {
                    bestEvaluation = actualResult.Evaluation;
                    bestResult = actualResult;
                }
            }

            return bestResult?.CloneAndAddStateToTop(startState, leaves, internalNodes + 1, allChildrenAreDeadEnds || pruned);
        }

        private bool AlphaBataShouldPrune(double alpha, double bata, double evaluation, Player player)
        {
            if (player == Player.Min && evaluation < alpha)
                return true;

            if (player == Player.Max && evaluation > bata)
                return true;

            return false;
        }

        private void UpdateAlphaAndBata(SearchContext searchContext, double evaluation, Player player)
        {
            if (player == Player.Max && evaluation > searchContext.Alpha)
                searchContext.Alpha = evaluation;
            if (player == Player.Min && evaluation < searchContext.Bata)
                searchContext.Bata = evaluation;
        }

        private (bool, int) ShouldDieEarlly(double evaluation, Player player, int pathLength)
        {
            if (!searchOptions.DieEarly)
                return (false, 0);
            
            if ((player == Player.Max && evaluation > searchOptions.MaxScore) ||
                (player == Player.Min && evaluation < searchOptions.MinScore))
            {
                if (searchOptions.FavorShortPaths && pathLength > 1)
                    return (true, pathLength - 1);

                return (true, 0);
            }

            return (false, 0);
        }

        private bool IsBetterThen(double firstValue, double secondValue, int firstPathLength, int? secondPathLength, Player player)
        {
            if (searchOptions.FavorShortPaths && BothEvaluationsAreEquallyAcceptable(firstValue, secondValue, player))           
                return player == Player.Min ? firstPathLength > secondPathLength : firstPathLength < secondPathLength;
            
            return player == Player.Min ? firstValue < secondValue : firstValue > secondValue;
        }

        private bool BothEvaluationsAreEquallyAcceptable(double evaluation1, double evaluation2, Player player)
        {
            if (evaluation1 == evaluation2)
                return true;
            if (!searchOptions.FavorShortPaths)
                return false;
            if (player == Player.Max && evaluation1 > searchOptions.MaxScore && evaluation2 > searchOptions.MaxScore)
                return true;
            if (player == Player.Min && evaluation1 < searchOptions.MinScore && evaluation2 < searchOptions.MinScore)
                return true;
            return false;
        }
    }
}
