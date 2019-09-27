using System.Threading;

namespace MinMaxSearch
{
    public interface ISearchEngine
    {
        SearchResult Search(IDeterministicState startState, int maxDepth, CancellationToken cancellationToken);
    }
}
