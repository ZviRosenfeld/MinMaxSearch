using System.Collections.Generic;
using System.Threading;

namespace MinMaxSearch
{
    class SearchContext
    {
        public SearchContext(int maxDepth, int currentDepth, double alpha, double bata, CancellationToken cancellationToken,
            List<IState> statesUpToNow)
        {
            MaxDepth = maxDepth;
            CurrentDepth = currentDepth;
            Alpha = alpha;
            Bata = bata;
            CancellationToken = cancellationToken;
            StatesUpTillNow = statesUpToNow;
        }

        public int MaxDepth { get; set; }
        public int CurrentDepth { get; set; }
        public double Alpha { get; set; }
        public double Bata { get; set; }
        public CancellationToken CancellationToken { get; set; }
        public List<IState> StatesUpTillNow { get; set; }
    }
}
