using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using MinMaxSearch.Exceptions;

namespace MinMaxSearch.Cache
{
    /// <summary>
    /// A cache manager in which the key is the state and passed through states
    /// </summary>
    public class StateAndPassedThroughCacheManager : ICacheManager
    {
        private readonly ConcurrentDictionary<StateAndPassedThroughStates, EvaluationRange> cache = new ConcurrentDictionary<StateAndPassedThroughStates, EvaluationRange>();

        public void AddExactEvaluation(IState state, int depth, IList<IState> passedThroughStates, double evaluation) =>
            cache[new StateAndPassedThroughStates(state, passedThroughStates)] = new EvaluationRange(evaluation);

        public void AddMinEvaluation(IState state, int depth, IList<IState> passedThroughStates, double minEvaluation)
        {
            var key = new StateAndPassedThroughStates(state, passedThroughStates);
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
            var key = new StateAndPassedThroughStates(state, passedThroughStates);
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
            var containsState = cache.TryGetValue(new StateAndPassedThroughStates(state, passedThroughStates), out var evaluation);
            return containsState ? evaluation : null;
        }

        public void Clear() => cache.Clear();

        public void Clear(Func<IState, int, IList<IState>, bool> shouldClean)
        {

            var statesToRemove = new HashSet<StateAndPassedThroughStates>();
            foreach (var state in cache.Keys)
                if (shouldClean(state.State, state.PassedThroughStates.Count, state.PassedThroughStates))
                    statesToRemove.Add(state);

            foreach (var state in statesToRemove)
                cache.TryRemove(state, out var _);
        }

        public int Count => cache.Count;

        public EvaluationRange this[IState state, List<IState> passedThroughStates] => 
            cache.TryGetValue(new StateAndPassedThroughStates(state, passedThroughStates), out var evaluation) ? evaluation : null;

        public override string ToString() => 
            $"{nameof(StateAndPassedThroughCacheManager)}; count {cache.Count}";
    }
}
