using System.Collections.Generic;
using System.Threading;

namespace MinMaxSearch
{
    public class SearchContext
    {
        private int maxDepth;

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

        public int MaxDepth
        {
            get => maxDepth;
            set
            {
                MaxDepthChanged = true;
                maxDepth = value;
            }
        }

        public int CurrentDepth { get; }
        public double Alpha { get; set; }
        public double Bata { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public List<IState> StatesUpTillNow { get; }
        public bool PruneAtMaxDepth { get; set; }
        public bool MaxDepthChanged { get; private set; } = false;
    }
}
