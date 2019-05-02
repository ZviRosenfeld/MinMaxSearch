using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MinMaxSearch.Pruners;

namespace MinMaxSearch
{
    class SearchWorker
    {
        private readonly int maxDepth;
        private readonly SearchEngine searchEngine;
        private readonly List<IPruner> pruners;
        private readonly ThreadManager threadManager;

        public SearchWorker(int maxDepth, SearchEngine searchEngine, List<IPruner> pruners)
        {
            this.maxDepth = maxDepth;
            this.searchEngine = searchEngine;
            this.pruners = pruners;
            threadManager = new ThreadManager(searchEngine.MaxDegreeOfParallelism);
        }
        
        public SearchResult Evaluate(IState startState, Player player, int depth, double alpha, double bata, CancellationToken cancellationToken, List<IState> statesUpToNow)
        {
            if (!startState.GetNeighbors().Any())           
                return new SearchResult(startState.Evaluate(depth, statesUpToNow), new List<IState> {startState}, 1, 0);
            
            if (ShouldStop(startState, depth, cancellationToken, statesUpToNow))
                return new SearchResult(startState.Evaluate(depth, statesUpToNow), new List<IState> {startState}, 1, 0);
               
            statesUpToNow = new List<IState>(statesUpToNow) { startState };
            return EvaluateChildren(startState, player, depth, alpha, bata, cancellationToken, statesUpToNow);
        }

        private SearchResult EvaluateChildren(IState startState, Player player, int depth, double alpha, double bata,
            CancellationToken cancellationToken, List<IState> statesUpToNow)
        {
            var results = new List<Task<SearchResult>>();
            var cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            foreach (var state in startState.GetNeighbors())
            {
                var taskResult = threadManager.Invoke(() => Evaluate(state, Utils.GetReversePlayer(player),
                    depth + 1, alpha, bata, cancellationSource.Token, statesUpToNow), cancellationToken);
                results.Add(taskResult);

                if (taskResult.Status == TaskStatus.RanToCompletion)
                {
                    var stateEvaluation = taskResult.Result;
                    if (AlphaBataShouldPrune(alpha, bata, stateEvaluation.Evaluation, player) ||
                        ShouldDieEarlly(stateEvaluation.Evaluation, player, stateEvaluation.StateSequence.Count))
                    {
                        cancellationSource.Cancel();
                        break;
                    }
                    UpdateAlphaAndBata(ref alpha, ref bata, stateEvaluation.Evaluation, player);
                }
            }

            return Reduce(results, player, startState) ?? new SearchResult(startState.Evaluate(depth, statesUpToNow),
                       new List<IState> {startState}, 1, 0);
        }

        private SearchResult Reduce(List<Task<SearchResult>> results, Player player, IState startState)
        {
            var bestEvaluation = player == Player.Max ? double.MinValue : double.MaxValue;
            SearchResult bestResult = null;
            int leaves = 0, internalNodes = 0;
            foreach (var result in results)
            {
                try
                {
                    if (result.IsCanceled) continue;

                    var actualResult = result.Result;
                    leaves += actualResult.Leaves;
                    internalNodes += actualResult.InternalNodes;
                    if (IsBetterThen(actualResult.Evaluation, bestEvaluation, actualResult.StateSequence.Count,
                        bestResult?.StateSequence?.Count, player))
                    {
                        bestEvaluation = actualResult.Evaluation;
                        bestResult = actualResult;
                    }
                }
                catch (AggregateException)
                {
                    // Do nothing - this can happen and if un-needed tasks were canceled
                }
            }

            return bestResult?.CloneAndAddStateToTop(startState, leaves, internalNodes + 1);
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
                return ((int) player) * firstPathLength < ((int) player) * secondPathLength;

            return ((int) player) * firstValue > ((int) player) * secondValue;
        }

        private bool AlphaBataShouldPrune(double alpha, double bata, double evaluation, Player player)
        {
            if (player == Player.Min && evaluation <= alpha)
                return true;

            if (player == Player.Max && evaluation >= bata)
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
    }
}
