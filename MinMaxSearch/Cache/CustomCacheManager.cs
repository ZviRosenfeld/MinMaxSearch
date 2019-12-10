using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MinMaxSearch.Cache
{
    /// <summary>
    /// This custom cache manager will only remember states that are important.
    /// </summary>
    class CustomCacheManager : ICacheManager
    {
        private readonly ConcurrentDictionary<IState, double> cache = new ConcurrentDictionary<IState, double>();

        public void Add(IState state, double evaluation)
        {
            // only add the state if it's important
            if (IsStateImportant(state))
                cache[state] = evaluation;
        }

        private bool IsStateImportant(IState state)
        {
            // Do some calclations here to determine if the state is important
            return true;
        }

        public bool ContainsState(IState state) => cache.ContainsKey(state);

        public double GetStateEvaluation(IState state) => cache[state];

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
    }
}
