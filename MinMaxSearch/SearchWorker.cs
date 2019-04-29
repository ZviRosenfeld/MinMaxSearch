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
        private int leaves;
        private int internalNodes;
        private readonly IDictionary<IState, Tuple<double, List<IState>>> endStates;
        private readonly SearchEngine searchEngine;
        private readonly List<IPruner> pruners;

        public SearchWorker(int maxDepth, SearchEngine searchEngine, List<IPruner> pruners, IDictionary<IState, Tuple<double, List<IState>>> endStates)
        {
            this.maxDepth = maxDepth;
            this.searchEngine = searchEngine;
            this.pruners = pruners;
            this.endStates = endStates;
        }
        
        public SearchResult Evaluate(IState startState, Player player, CancellationToken cancellationToken)
        {
            var evaluation = Evaluate(startState, player, 0, double.MinValue, double.MaxValue, cancellationToken, new List<IState>());
            evaluation.Item2.Reverse();
            return new SearchResult(evaluation.Item2.First(), evaluation.Item1, evaluation.Item2, leaves, internalNodes);
        }

        public (double, List<IState>, bool) Evaluate(IState startState, Player player, int depth, double alpha, double bata, CancellationToken cancellationToken, List<IState> statesUpToNow)
        {
            if (!startState.GetNeighbors().Any())
            {
                leaves++;
                return (startState.Evaluate(depth, statesUpToNow), new List<IState>(), true);
            }
            if (searchEngine.RemeberDeadEndStates && endStates.ContainsKey(startState))
            {
                leaves++;
                return (endStates[startState].Item1, endStates[startState].Item2, true);
            }
            if (ShouldStop(startState, depth, cancellationToken, statesUpToNow))
            {
                leaves++;
                return (startState.Evaluate(depth, statesUpToNow), new List<IState>(), false);
            }
            
            statesUpToNow = new List<IState>(statesUpToNow) { startState };
            internalNodes++;
            return EvaluateChildren(startState, player, depth, alpha, bata, cancellationToken, statesUpToNow);
        }

        private (double, List<IState>, bool) EvaluateChildren(IState startState, Player player, int depth, double alpha, double bata,
            CancellationToken cancellationToken, List<IState> statesUpToNow)
        {
            var bestEvaluation = player == Player.Max ? double.MinValue : double.MaxValue;
            List<IState> stateSequence = null;
            var allChildrenAreEndStates = true;
            foreach (var state in startState.GetNeighbors())
            {
                var stateEvaluation = Evaluate(state, Utils.GetReversePlayer(player), depth + 1, alpha, bata, cancellationToken, statesUpToNow);
                allChildrenAreEndStates = allChildrenAreEndStates && stateEvaluation.Item3;
                if (IsBetterThen(stateEvaluation.Item1, bestEvaluation, stateEvaluation.Item2.Count, stateSequence?.Count, player))
                {
                    bestEvaluation = stateEvaluation.Item1;
                    stateSequence = stateEvaluation.Item2;
                    stateSequence.Add(state);
                    if (AlphaBataShouldPrune(alpha, bata, stateEvaluation.Item1, player))
                        break;
                    if (ShouldDieEarlly(bestEvaluation, player, stateSequence.Count))
                        break;
                    UpdateAlphaAndBata(ref alpha, ref bata, stateEvaluation.Item1, player);
                }
            }

            if (allChildrenAreEndStates && searchEngine.RemeberDeadEndStates)
                 endStates[startState] = new Tuple<double, List<IState>>(bestEvaluation, stateSequence.ToList());

            return (bestEvaluation, stateSequence, allChildrenAreEndStates);
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
