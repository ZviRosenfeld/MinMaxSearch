using System;

namespace MinMaxSearch.Cache
{
    public interface ICacheManager
    {
        void AddExactEvaluation(IState state, double evaluation);

        void AddMinEvaluation(IState state, double minEvaluation);

        void AddMaxEvaluation(IState state, double maxEvaluation);
        
        EvaluationRange GetStateEvaluation(IState state);

        /// <summary>
        /// Clears all states from the cache
        /// </summary>
        void Clear();

        /// <summary>
        /// State will only be deleted if the 'shouldClean' conditon is meat
        /// </summary>
        void Clear(Func<IState, bool> shouldClean);
    }
}
