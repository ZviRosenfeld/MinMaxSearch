using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MinMaxSearch.Cache
{
    public class CacheManager : ICacheManager
    {
        private readonly ConcurrentDictionary<IState, double> cache = new ConcurrentDictionary<IState, double>();

        public void Add(IState state, double evaluation) => cache[state] = evaluation;

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
