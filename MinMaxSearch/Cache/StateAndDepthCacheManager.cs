using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using MinMaxSearch.Exceptions;

namespace MinMaxSearch.Cache
{
    public class StateAndDepthCacheManager : ICacheManager
    {
        private readonly ConcurrentDictionary<StateAndDepth, EvaluationRange> cache = new ConcurrentDictionary<StateAndDepth, EvaluationRange>();

        public void AddExactEvaluation(IState state, int depth, IList<IState> passedThroughStates, double evaluation) =>
            cache[new StateAndDepth(state, depth)] = new EvaluationRange(evaluation);

        public void AddMinEvaluation(IState state, int depth, IList<IState> passedThroughStates, double minEvaluation)
        {
            var key = new StateAndDepth(state, depth);
            var containsValue = cache.TryGetValue(key, out var evaluation);
            if (containsValue && evaluation.MinEvaluation < minEvaluation)
            {
                if (evaluation.MaxEvaluation < minEvaluation)
                    throw new UnknownProblemException($"cache's {nameof(minEvaluation)} is greater than {nameof(evaluation.MaxEvaluation)}");
                cache[key].MinEvaluation = minEvaluation;
            }
            else if (!containsValue)
                cache[key] = new EvaluationRange(minEvaluation, int.MaxValue);
        }

        public void AddMaxEvaluation(IState state, int depth, IList<IState> passedThroughStates, double maxEvaluation)
        {
            var key = new StateAndDepth(state, depth);
            var containsValue = cache.TryGetValue(key, out var evaluation);
            if (containsValue && evaluation.MaxEvaluation > maxEvaluation)
            {
                if (evaluation.MinEvaluation > maxEvaluation)
                    throw new UnknownProblemException($"cache's {nameof(maxEvaluation)} is smaller than {nameof(evaluation.MinEvaluation)}");
                cache[key].MaxEvaluation = maxEvaluation;
            }
            else if (!containsValue)
                cache[key] = new EvaluationRange(int.MinValue, maxEvaluation);
        }

        public EvaluationRange GetStateEvaluation(IState state, int depth, IList<IState> passedThroughStates)
        {
            var containsState = cache.TryGetValue(new StateAndDepth(state, depth), out var evaluation);
            return containsState ? evaluation : null;
        }

        public void Clear() => cache.Clear();

        public void Clear(Func<IState, int, IList<IState>, bool> shouldClean)
        {
            var statesToRemove = new HashSet<StateAndDepth>();
            foreach (var state in cache.Keys)
                if (shouldClean(state.State, state.Depth, new List<IState>()))
                    statesToRemove.Add(state);

            foreach (var state in statesToRemove)
                cache.TryRemove(state, out var _);
        }

        public int Count => cache.Count;

        public EvaluationRange this[IState state, int depth] => 
            cache.TryGetValue(new StateAndDepth(state, depth), out var evaluation) ? evaluation : null;

        public override string ToString() => 
            $"{nameof(StateAndDepthCacheManager)}; count {cache.Count}";
    }
}
