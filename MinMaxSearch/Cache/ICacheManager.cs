namespace MinMaxSearch.Cache
{
    public interface ICacheManager
    {
        void Add(IState state, double evaluation);
        bool ContainsState(IState state);
        double GetStateEvaluation(IState state);
        void Clear();
    }
}
