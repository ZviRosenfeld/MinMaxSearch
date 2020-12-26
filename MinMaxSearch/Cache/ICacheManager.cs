using System;
using System.Collections.Generic;

namespace MinMaxSearch.Cache
{
    public interface ICacheManager
    {
        void AddExactEvaluation(IState state, int depth, IList<IState> passedThroughStates, double evaluation);

        void AddMinEvaluation(IState state, int depth, IList<IState> passedThroughStates, double minEvaluation);

        void AddMaxEvaluation(IState state, int depth, IList<IState> passedThroughStates, double maxEvaluation);
        
        EvaluationRange GetStateEvaluation(IState state, int depth, IList<IState> passedThroughStates);

        /// <summary>
        /// Clears all states from the cache
        /// </summary>
        void Clear();

        /// <summary>
        /// State will only be deleted if the 'shouldClean' condition is meat.
        /// The shouldClean Func expects 3 parameters: the state, it's depths, and a list of the passed through states.
        /// Depth and passed through states will be 0 and empty if the cache doesn't remember them.
        /// </summary>
        void Clear(Func<IState, int, IList<IState>, bool> shouldClean);
    }
}
