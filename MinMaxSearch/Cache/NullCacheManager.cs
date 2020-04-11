using System;
using System.Collections.Generic;

namespace MinMaxSearch.Cache
{
    class NullCacheManager : ICacheManager
    {
        public void AddExactEvaluation(IState state, int depth, IList<IState> passedThroughStates, double evaluation)
        {
            // Do nothing
        }

        public void AddMinEvaluation(IState state, int depth, IList<IState> passedThroughStates, double minEvaluation)
        {
            // Do nothing
        }

        public void AddMaxEvaluation(IState state, int depth, IList<IState> passedThroughStates, double maxEvaluation)
        {
            // Do nothing
        }

        EvaluationRange ICacheManager.GetStateEvaluation(IState state, int depth, IList<IState> passedThroughStates) => null;
        
        public void Clear()
        {
            // Do nothing
        }

        public void Clear(Func<IState, int , IList<IState>, bool> shouldClean)
        {
            // Do nothing
        }
    }
}
