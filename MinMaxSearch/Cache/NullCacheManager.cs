using System;
using MinMaxSearch.Exceptions;

namespace MinMaxSearch.Cache
{
    class NullCacheManager : ICacheManager
    {
        public void AddExactEvaluation(IState state, double evaluation)
        {
            // Do nothing
        }

        public void AddMinEvaluation(IState state, double minEvaluation)
        {
            // Do nothing
        }

        public void AddMaxEvaluation(IState state, double maxEvaluation)
        {
            // Do nothing
        }

        EvaluationRange ICacheManager.GetStateEvaluation(IState state) => null;
        
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
