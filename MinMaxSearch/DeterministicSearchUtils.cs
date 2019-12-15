using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MinMaxSearch.ThreadManagment;

namespace MinMaxSearch
{
    class DeterministicSearchUtils
    {
        private readonly SearchOptions searchOptions;
        private readonly SearchWorker searchWorker;
        private readonly IThreadManager threadManager;

        public DeterministicSearchUtils(SearchWorker searchWorker, SearchOptions searchOptions, IThreadManager threadManager)
        {
            this.searchWorker = searchWorker;
            this.searchOptions = searchOptions;
            this.threadManager = threadManager;
        }

        public SearchResult EvaluateChildren(IDeterministicState startState, SearchContext searchContext,
            IDictionary<IState, double> storedStates = null)
        {
            var neighbors = startState.GetNeighbors().ToArray();
            if (!neighbors.Any())
            {
                var evaluation = startState.Evaluate(searchContext.CurrentDepth, searchContext.StatesUpTillNow, searchOptions);
                return new SearchResult(evaluation, startState);
            }
            
            var player = startState.Turn;
            var results = new List<Task<SearchResult>>();
            var cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(searchContext.CancellationToken);
            searchContext.CancellationToken = cancellationSource.Token;
            foreach (var state in neighbors)
            {
                var taskResult = Evaluate(startState, searchContext, storedStates, state);
                results.Add(taskResult);

                if (ProcessResultAndReturnWhetherWeShouldBreak(taskResult, searchContext, storedStates, state, player))
                {
                    cancellationSource.Cancel();
                    break;
                }
            }
            
            return Reduce(results, player, startState);
        }

        private bool ProcessResultAndReturnWhetherWeShouldBreak(Task<SearchResult> taskResult, SearchContext searchContext, IDictionary<IState, double> storedStates, IState state, Player player)
        {
            if (taskResult.Status == TaskStatus.RanToCompletion)
            {
                var stateEvaluation = taskResult.Result;
                if (storedStates != null)
                    storedStates[state] = stateEvaluation.Evaluation;
                if (AlphaBataShouldPrune(searchContext.Alpha, searchContext.Bata, stateEvaluation.Evaluation, player))
                    return true;
                
                var shouldDieEarlly = ShouldDieEarlly(stateEvaluation.Evaluation, player, stateEvaluation.StateSequence.Count);
                if (shouldDieEarlly.Item1 && shouldDieEarlly.Item2 == 0)
                    return true;
                else if (shouldDieEarlly.Item1)
                {
                    searchContext.MaxDepth = shouldDieEarlly.Item2 + searchContext.CurrentDepth;
                    searchContext.PruneAtMaxDepth = true;
                }

                UpdateAlphaAndBata(searchContext, stateEvaluation.Evaluation, player);
            }
            return false;
        }

        private Task<SearchResult> Evaluate(IDeterministicState startState, SearchContext searchContext,
            IDictionary<IState, double> storedStates, IState state)
        {
            var taskResult = storedStates != null && storedStates.ContainsKey(state)
                ? Task.FromResult(new SearchResult(storedStates[state], state))
                : threadManager.Invoke(() =>
                {
                    var actualStartState = startState is ProbablisticStateWrapper wrapper
                        ? (IState) wrapper.InnerState
                        : startState;
                    return searchWorker.Evaluate(state, searchContext.CloneAndAddState(actualStartState));
                }, searchContext.CurrentDepth);
            return taskResult;
        }

        private SearchResult Reduce(List<Task<SearchResult>> results, Player player, IState startState)
        {
            var bestEvaluation = player == Player.Max ? double.MinValue : double.MaxValue;
            SearchResult bestResult = null;
            int leaves = 0, internalNodes = 0;
            bool allChildrenAreDeadEnds = true, fullTreeSearched = true;
            foreach (var result in results)
            {
                var actualResult = result.Result;
                leaves += actualResult.Leaves;
                internalNodes += actualResult.InternalNodes;
                allChildrenAreDeadEnds = allChildrenAreDeadEnds && actualResult.AllChildrenAreDeadEnds;
                fullTreeSearched = fullTreeSearched && actualResult.FullTreeSearched;
                if (IsBetterThen(actualResult.Evaluation, bestEvaluation, actualResult.StateSequence.Count,
                    bestResult?.StateSequence?.Count, player))
                {
                    bestEvaluation = actualResult.Evaluation;
                    bestResult = actualResult;
                }
            }

            var childrenContainWiningPosition = bestEvaluation >= searchOptions.MaxScore || bestEvaluation <= searchOptions.MinScore;
            return bestResult.CloneAndAddStateToTop(startState, leaves, internalNodes + 1, fullTreeSearched || childrenContainWiningPosition, allChildrenAreDeadEnds || childrenContainWiningPosition);
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

        /// <returns> A tuple of (SouldWeDieEarlly, and if so InHowManyMoves)</returns>
        private (bool, int) ShouldDieEarlly(double evaluation, Player player, int pathLength)
        {
            if (!searchOptions.DieEarly)
                return (false, 0);
            
            if ((player == Player.Max && evaluation >= searchOptions.MaxScore) ||
                (player == Player.Min && evaluation <= searchOptions.MinScore))
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
            if (player == Player.Max && evaluation1 >= searchOptions.MaxScore && evaluation2 >= searchOptions.MaxScore)
                return true;
            if (player == Player.Min && evaluation1 <= searchOptions.MinScore && evaluation2 <= searchOptions.MinScore)
                return true;
            return false;
        }
    }
}
