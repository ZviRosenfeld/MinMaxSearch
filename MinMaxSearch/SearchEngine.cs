using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MinMaxSearch.Pruners;

namespace MinMaxSearch
{
    public class SearchEngine
    {
        private readonly int maxDepth;
        private readonly List<IPruner> pruners = new List<IPruner>();
        private readonly IDictionary<IState, Tuple<double, List<IState>>> endStates = new ConcurrentDictionary<IState, Tuple<double, List<IState>>>();

        public void AddPruner(IPruner pruner) => pruners.Add(pruner);

        /// <summary>
        /// At unstable states, we'll continue searching even after we've hit the maxDepth limit
        /// </summary>
        public Func<IState, int, List<IState>, bool> IsUnstableState { get; set; } = ((s, d, l) => false);
        
        /// <summary>
        /// If true, the engine will remember when a state leades to an endState and - when encountering that state again - used the stored evaluation. 
        /// Note that this will only work if you implement Equals and GetHashValue in a meaningful way in the states. 
        /// </summary>
        public bool RemeberDeadEndStates { get; set; } = false;

        /// <summary>
        /// Note that this will only work if you implement Equals and GetHashValue in a meaningful way in the states. 
        /// </summary>
        public bool PreventLoops { get; set; } = false;

        /// <summary>
        /// If two path give the same score, but one is shorter then the other - we'll take the shorter one
        /// </summary>
        public bool FavorShortPaths { get; set; } = true;

        /// <summary>
        /// The search will end once we find a score better then MaxScore for Max or worse then MinScore for Min
        /// </summary>
        public bool DieEarly { get; set; }

        public double MaxScore { get; set; } = double.MaxValue;

        public double MinScore { get; set; } = double.MinValue;

        public SearchEngine(int maxDepth)
        {
            this.maxDepth = maxDepth < 1 ? 1 : maxDepth;
        }

        public SearchResult Evaluate(IState startState, Player player) =>
            Evaluate(startState, player, CancellationToken.None);

        public SearchResult Evaluate(IState startState, Player player, CancellationToken cancellationToken)
        {
            if (PreventLoops)
                AddPruner(new PreventLoopPruner());
            
            if (!startState.GetNeighbors().Any())
                throw new NoNeighborsException(startState);

            if (player == Player.Empty)
                throw new EmptyPlayerException(nameof(player) + " can't be " + nameof(Player.Empty));

            var searchWorker = new SearchWorker(maxDepth, this, pruners, endStates);
            return searchWorker.Evaluate(startState, player, cancellationToken);
        }
    }
}
