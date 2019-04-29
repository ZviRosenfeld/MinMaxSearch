using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MinMaxSearch.Pruners;

namespace MinMaxSearch
{
    class SearchWorker
    {
        private readonly int maxDepth;
        private readonly IDictionary<IState, SearchResult> endStates;
        private readonly SearchEngine searchEngine;
        private readonly List<IPruner> pruners;

        public SearchWorker(int maxDepth, SearchEngine searchEngine, List<IPruner> pruners, IDictionary<IState, SearchResult> endStates)
        {
            this.maxDepth = maxDepth;
            this.searchEngine = searchEngine;
            this.pruners = pruners;
            this.endStates = endStates;
        }
        
        public SearchResult Evaluate(IState startState, Player player, CancellationToken cancellationToken)
        {
            var evaluation = Evaluate(startState, player, 0, double.MinValue, double.MaxValue, cancellationToken, new List<IState>());
            evaluation.StateSequence.Reverse();
            // Removeing the top node will make the result "nicer"
            return evaluation.CloneAndRemoveTopNode();
        }

        public SearchResult Evaluate(IState startState, Player player, int depth, double alpha, double bata, CancellationToken cancellationToken, List<IState> statesUpToNow)
        {
            if (!startState.GetNeighbors().Any())           
                return new SearchResult(startState, startState.Evaluate(depth, statesUpToNow), new List<IState> {startState}, 1, 0, true);
            
            if (searchEngine.RemeberDeadEndStates && endStates.ContainsKey(startState))
                return endStates[startState];
            
            if (ShouldStop(startState, depth, cancellationToken, statesUpToNow))
                return new SearchResult(startState, startState.Evaluate(depth, statesUpToNow), new List<IState> {startState}, 1, 0, false);
               
            statesUpToNow = new List<IState>(statesUpToNow) { startState };
            return EvaluateChildren(startState, player, depth, alpha, bata, cancellationToken, statesUpToNow);
        }

        private SearchResult EvaluateChildren(IState startState, Player player, int depth, double alpha, double bata,
            CancellationToken cancellationToken, List<IState> statesUpToNow)
        {
            var bestEvaluation = player == Player.Max ? double.MinValue : double.MaxValue;
            SearchResult bestResult = null;
            var allChildrenAreEndStates = true;
            foreach (var state in startState.GetNeighbors())
            {
                var stateEvaluation = Evaluate(state, Utils.GetReversePlayer(player), depth + 1, alpha, bata, cancellationToken, statesUpToNow);
                allChildrenAreEndStates = allChildrenAreEndStates && stateEvaluation.DeadEnd;
                if (IsBetterThen(stateEvaluation.Evaluation, bestEvaluation, stateEvaluation.StateSequence.Count, bestResult?.StateSequence?.Count, player))
                {
                    bestEvaluation = stateEvaluation.Evaluation;
                    bestResult = stateEvaluation;
                    if (AlphaBataShouldPrune(alpha, bata, stateEvaluation.Evaluation, player))
                        break;
                    if (ShouldDieEarlly(bestEvaluation, player, bestResult.StateSequence.Count))
                        break;
                    UpdateAlphaAndBata(ref alpha, ref bata, stateEvaluation.Evaluation, player);
                }
            }

            var finalResult = bestResult.CloneAndAddStateToTop(startState, allChildrenAreEndStates);
            if (allChildrenAreEndStates && searchEngine.RemeberDeadEndStates)
                 endStates[startState] = finalResult;

            return finalResult;
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
