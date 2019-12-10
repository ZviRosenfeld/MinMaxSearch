using System.Collections.Concurrent;

namespace MinMaxSearch.Cache
{
    public class CacheManager : ICacheManager
    {
        private readonly ConcurrentDictionary<IState, double> cache = new ConcurrentDictionary<IState, double>();

        public void Add(IState state, double evaluation) => cache[state] = evaluation;

        public bool ContainsState(IState state) => cache.ContainsKey(state);

        public double GetStateEvaluation(IState state) => cache[state];

        public void Clear() => cache.Clear();
    }
}
