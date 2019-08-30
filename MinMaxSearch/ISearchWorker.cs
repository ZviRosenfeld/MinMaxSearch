namespace MinMaxSearch
{
    public interface ISearchWorker
    {
        SearchResult Evaluate(IState startState, SearchContext searchContext);
    }
}