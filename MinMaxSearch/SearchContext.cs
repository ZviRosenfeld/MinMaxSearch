using System.Collections.Generic;
using System.Threading;

namespace MinMaxSearch
{
    public class SearchContext
    {
        public SearchContext(int maxDepth, int currentDepth, CancellationToken cancellationToken,
            List<IState> statesUpToNow = null, double alpha = double.MinValue, double bata = double.MaxValue,
            bool pruneAtMaxDepth = false)
        {
            MaxDepth = maxDepth;
            CurrentDepth = currentDepth;
            Alpha = alpha;
            Bata = bata;
            CancellationToken = cancellationToken;
            StatesUpTillNow = statesUpToNow ?? new List<IState>();
            PruneAtMaxDepth = pruneAtMaxDepth;
        }

        public int MaxDepth { get; set; }
        public int CurrentDepth { get; set; }
        public double Alpha { get; set; }
        public double Bata { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public List<IState> StatesUpTillNow { get; set; }
        public bool PruneAtMaxDepth { get; set; }
    }
}
