using System;

namespace MinMaxSearch.Cache
{
    public interface ICacheManager
    {
        void Add(IState state, double evaluation);
        bool ContainsState(IState state);
        double GetStateEvaluation(IState state);
        void Clear();

        /// <param name="shouldClean"> State will only be deleted if the conditon is meat</param>
        void Clear(Func<IState, bool> shouldClean);
    }
}
