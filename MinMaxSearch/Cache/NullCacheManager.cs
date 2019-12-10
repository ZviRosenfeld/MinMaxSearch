using System;
using MinMaxSearch.Exceptions;

namespace MinMaxSearch.Cache
{
    class NullCacheManager : ICacheManager
    {
        public void Add(IState state, double evaluation)
        {
            // Do nothing
        }

        public bool ContainsState(IState state) => false; 

        public double GetStateEvaluation(IState state) =>
            throw new InternalException($"Code 1002 ({nameof(NullCacheManager)}.{nameof(GetStateEvaluation)} was called)");

        public void Clear()
        {
            // Do nothing
        }

        public void Clear(Func<IState, bool> shouldClean)
        {
            // Do nothing
        }
    }
}
