using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MinMaxSearch
{
    class DeterministicSearchUtils
    {
        private readonly SearchEngine searchEngine;
        private readonly SearchWorker searchWorker;
        private readonly ThreadManager threadManager;

        public DeterministicSearchUtils(SearchWorker searchWorker, SearchEngine searchEngine, ThreadManager threadManager)
        {
            this.searchWorker = searchWorker;
            this.searchEngine = searchEngine;
            this.threadManager = threadManager;
        }

        public SearchResult EvaluateChildren(IDeterministicState startState, int depth, double alpha, double bata,
            CancellationToken cancellationToken, List<IState> statesUpToNow, IDictionary<IState, double> storedStates = null)
        {
            if (!startState.GetNeighbors().Any())
                return new SearchResult(startState.Evaluate(depth, statesUpToNow), new List<IState> {startState}, 1, 0);

            var player = startState.Turn;
            var results = new List<Task<SearchResult>>();
            var cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            foreach (var state in startState.GetNeighbors())
            {
                var taskResult = storedStates != null && storedStates.ContainsKey(state)
                    ? Task.FromResult(new SearchResult(storedStates[state], new List<IState> {state}, 1, 0))
                    : threadManager.Invoke(() =>
                        searchWorker.Evaluate(state, depth + 1, alpha, bata, cancellationSource.Token, new List<IState>(statesUpToNow) {startState}));
                results.Add(taskResult);

                if (taskResult.Status == TaskStatus.RanToCompletion && taskResult.Result != null)
                {
                    var stateEvaluation = taskResult.Result;
                    if (storedStates != null)
                        storedStates[state] = stateEvaluation.Evaluation;
                    if (AlphaBataShouldPrune(alpha, bata, stateEvaluation.Evaluation, player) ||
                        ShouldDieEarlly(stateEvaluation.Evaluation, player, stateEvaluation.StateSequence.Count))
                    {
                        cancellationSource.Cancel();
                        break;
                    }
                    UpdateAlphaAndBata(ref alpha, ref bata, stateEvaluation.Evaluation, player);
                }
            }

            return Reduce(results, player, startState);
        }

        private SearchResult Reduce(List<Task<SearchResult>> results, Player player, IState startState)
        {
            var bestEvaluation = player == Player.Max ? double.MinValue : double.MaxValue;
            SearchResult bestResult = null;
            int leaves = 0, internalNodes = 0;
            foreach (var result in results)
            {
                var actualResult = result.Result;
                if (actualResult == null) continue;

                leaves += actualResult.Leaves;
                internalNodes += actualResult.InternalNodes;
                if (IsBetterThen(actualResult.Evaluation, bestEvaluation, actualResult.StateSequence.Count,
                    bestResult?.StateSequence?.Count, player))
                {
                    bestEvaluation = actualResult.Evaluation;
                    bestResult = actualResult;
                }
            }
            
            return bestResult?.CloneAndAddStateToTop(startState, leaves, internalNodes + 1);
        }

        private bool AlphaBataShouldPrune(double alpha, double bata, double evaluation, Player player)
        {
            if (player == Player.Min && evaluation < alpha)
                return true;

            if (player == Player.Max && evaluation > bata)
                return true;

            return false;
        }

        private void UpdateAlphaAndBata(ref double alpha, ref double bata, double evaluation, Player player)
        {
            if (player == Player.Max && evaluation > alpha)
                alpha = evaluation;
            if (player == Player.Min && evaluation < bata)
                bata = evaluation;
        }

        private bool ShouldDieEarlly(double evaluation, Player player, int pathLength)
        {
            if (!searchEngine.DieEarly)
                return false;
            if (searchEngine.FavorShortPaths && pathLength > 1)
                return false;

            if (player == Player.Min && evaluation > searchEngine.MaxScore)
                return true;
            if (player == Player.Max && evaluation < searchEngine.MinScore)
                return true;

            return false;
        }

        private bool IsBetterThen(double firstValue, double secondValue, int firstPathLength, int? secondPathLength, Player player)
        {
            if (searchEngine.FavorShortPaths && firstValue == secondValue)
            {
                if (player == Player.Min)
                    return firstPathLength > secondPathLength;
                return firstPathLength < secondPathLength;
            }

            if (player == Player.Min)
                return firstValue < secondValue;
            return firstValue > secondValue;
        }
    }
}
