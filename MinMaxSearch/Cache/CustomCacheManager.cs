using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MinMaxSearch.Cache
{
    /// <summary>
    /// This custom cache manager will only remember states that we think are likely to be repeated many time.
    /// </summary>
    class CustomCacheManager : ICacheManager
    {
        // We use a ConcurrentDictionary since ChacheManager will be accessed by many threads, so it must be thread-safe.
        private readonly ConcurrentDictionary<IState, double> cache = new ConcurrentDictionary<IState, double>();

        public void Add(IState state, double evaluation)
        {
            // only add the state if it's likely to be repeated
            if (IsStateLikelyToBeRepeated(state))
                cache[state] = evaluation;
        }

        private bool IsStateLikelyToBeRepeated(IState state)
        {
            // Do some calclations here to determine if the state is important
            return true;
        }

        public bool ContainsState(IState state) => cache.ContainsKey(state);

        public double GetStateEvaluation(IState state) => cache[state];

        /// <summary>
        /// Clears all states from the cache
        /// </summary>
        public void Clear() => cache.Clear();

        /// <summary>
        /// State will only be deleted if the 'shouldClean' conditon is meat
        /// </summary>
        public void Clear(Func<IState, bool> shouldClean)
        {
            var statesToRemove = new HashSet<IState>();
            foreach (var state in cache.Keys)
                if (shouldClean(state))
                    statesToRemove.Add(state);

            foreach (var state in statesToRemove)
                cache.TryRemove(state, out var _);
        }
    }
}
