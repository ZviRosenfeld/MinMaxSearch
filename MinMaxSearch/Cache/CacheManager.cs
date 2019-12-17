using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using MinMaxSearch.Exceptions;

namespace MinMaxSearch.Cache
{
    public class CacheManager : ICacheManager
    {
        private readonly ConcurrentDictionary<IState, EvaluationRange> cache = new ConcurrentDictionary<IState, EvaluationRange>();

        public void AddExactEvaluation(IState state, double evaluation) =>
            cache[state] = new EvaluationRange(evaluation);

        public void AddMinEvaluation(IState state, double minEvaluation)
        {
            var containsValue = cache.TryGetValue(state, out var evaluation);
            if (containsValue && evaluation.MinEvaluation < minEvaluation)
            {
                if (evaluation.MaxEvaluation < minEvaluation)
                    throw new UnknownProblemException($"cache's {nameof(minEvaluation)} is greater than {nameof(evaluation.MaxEvaluation)}");
                cache[state].MinEvaluation = minEvaluation;
            }
            else if (!containsValue)
                cache[state] = new EvaluationRange(minEvaluation, int.MaxValue);
        }

        public void AddMaxEvaluation(IState state, double maxEvaluation)
        {
            var containsValue = cache.TryGetValue(state, out var evaluation);
            if (containsValue && evaluation.MaxEvaluation > maxEvaluation)
            {
                if (evaluation.MinEvaluation > maxEvaluation)
                    throw new UnknownProblemException($"cache's {nameof(maxEvaluation)} is smaller than {nameof(evaluation.MinEvaluation)}");
                cache[state].MaxEvaluation = maxEvaluation;
            }
            else if (!containsValue)
                cache[state] = new EvaluationRange(int.MinValue, maxEvaluation);
        }

        public EvaluationRange GetStateEvaluation(IState state)
        {
            var containsState = cache.TryGetValue(state, out var evaluation);
            return containsState ? evaluation : null;
        }

        public void Clear() => cache.Clear();

        public void Clear(Func<IState, bool> shouldClean)
        {
            var statesToRemove = new HashSet<IState>();
            foreach (var state in cache.Keys)
                if (shouldClean(state))
                    statesToRemove.Add(state);

            foreach (var state in statesToRemove)
                cache.TryRemove(state, out var _);
        }

        public int Count => cache.Count;

        public EvaluationRange this[IState state] => 
            cache.TryGetValue(state, out var evaluation) ? evaluation : null;
    }
}
