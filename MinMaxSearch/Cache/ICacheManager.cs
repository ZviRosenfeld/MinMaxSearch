using System;

namespace MinMaxSearch.Cache
{
    public interface ICacheManager
    {
        void Add(IState state, double evaluation);

        bool ContainsState(IState state);

        double GetStateEvaluation(IState state);

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
