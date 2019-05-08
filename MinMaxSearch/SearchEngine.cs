using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MinMaxSearch.Pruners;

namespace MinMaxSearch
{
    public class SearchEngine
    {
        private readonly List<IPruner> pruners = new List<IPruner>();
        private IDictionary<IState, Tuple<double, List<IState>>> deadEndStates = new ConcurrentDictionary<IState, Tuple<double, List<IState>>>();

        /// <summary>
        /// Clears the remembered DeadEndStates.
        /// </summary>
        public void Clear() => deadEndStates = new ConcurrentDictionary<IState, Tuple<double, List<IState>>>();

        /// <summary>
        /// Clears the remembered DeadEndStates that meat the shouldClear Func criteria. 
        /// This method can be used in the background while waiting for the user's move.
        /// </summary>
        /// <param name="shouldClear"> If this Func returns true, the state will be cleared</param>
        /// <param name="cancellationToken"></param>
        public void SmartClear(Func<IState, bool> shouldClear, CancellationToken cancellationToken)
        {
            for (int i = 0; i < deadEndStates.Count; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                var state = deadEndStates.ElementAt(i).Key;
                if (shouldClear(state))
                {
                    deadEndStates.Remove(state);
                    i--;
                }
            }
        }

        public void AddPruner(IPruner pruner) => pruners.Add(pruner);

        /// <summary>
        /// At unstable states, we'll continue searching even after we've hit the maxDepth limit
        /// </summary>
        public Func<IState, int, List<IState>, bool> IsUnstableState { get; set; } = ((s, d, l) => false);
        
        /// <summary>
        /// Note that this will only work if you implement Equals and GetHashValue in a meaningful way in the states. 
        /// </summary>
        public bool PreventLoops { get; set; }

        /// <summary>
        /// If two path give the same score, but one is shorter then the other - we'll take the shorter one
        /// </summary>
        public bool FavorShortPaths { get; set; } = true;

        /// <summary>
        /// The search will end once we find a score better then MaxScore for Max or worse then MinScore for Min
        /// </summary>
        public bool DieEarly { get; set; }

        /// <summary>
        /// Tells the engine whether to remember states from which all children lead to endStates, so that it won't need to re-calculate their search-tree. 
        /// This can save a lot of time in some games.
        /// You can use SearchEngine methods Clear or SmartClear methods to clear the remembered dead-end states if they're taking up too much memory.
        /// Note that this will only work if the state overrides object's Equals and GetHashCode methods in a meaningful way.
        /// </summary>
        public RememberStatesMode RememberDeadEndStates { get; set; } = RememberStatesMode.InSameSearch;

        public double MaxScore { get; set; } = double.MaxValue;

        public double MinScore { get; set; } = double.MinValue;

        private int maxDegreeOfParallelism = 1;
        public int MaxDegreeOfParallelism
        {
            get => maxDegreeOfParallelism;
            set => maxDegreeOfParallelism = value > 0
                ? value
                : throw new BadDegreeOfParallelismException("DegreeOfParallelism must be at least one. Tried to set it to " + maxDegreeOfParallelism);
        }
        
        public SearchResult Search(IDeterministicState startState, int maxDepth) =>
            Search(startState, maxDepth, CancellationToken.None);

        public Task<SearchResult> SearchAsync(IDeterministicState startState, int maxDepth, CancellationToken cancellationToken) => 
            Task.Run(() => Search(startState, maxDepth, cancellationToken));

        public SearchResult Search(IDeterministicState startState, int maxDepth, CancellationToken cancellationToken)
        {
            if (!startState.GetNeighbors().Any())
                throw new NoNeighborsException("start state has no nighbors " + startState);
            
            var searchWorker = new SearchWorker(maxDepth, CreateSearchOptions(), GetStoredStatesDictionary());
            var evaluation = searchWorker.Evaluate(startState, 0, double.MinValue, double.MaxValue, cancellationToken, new List<IState>());
            evaluation.StateSequence.Reverse();
            evaluation.StateSequence.RemoveAt(0); // Removing the top node will make the result "nicer"
            return evaluation;
        }

        private IDictionary<IState, Tuple<double, List<IState>>> GetStoredStatesDictionary()
        {
            switch (RememberDeadEndStates)
            {
                case RememberStatesMode.Never: return new NullDictionary<IState, Tuple<double, List<IState>>>();
                case RememberStatesMode.InSameSearch: return new ConcurrentDictionary<IState, Tuple<double, List<IState>>>();
                case RememberStatesMode.BetweenSearches: return deadEndStates;
                default:
                    throw new ArgumentException(
                        $"{nameof(RememberDeadEndStates)} is of type {RememberDeadEndStates}, which isn't valid");
            }
        }

        private SearchOptions CreateSearchOptions() => new SearchOptions(pruners, IsUnstableState, PreventLoops,
            FavorShortPaths, DieEarly, MaxScore, MinScore, MaxDegreeOfParallelism);

        public SearchResult IterativeSearch(IDeterministicState startState, int startDepth, int maxDepth, TimeSpan timeout) =>
            IterativeSearch(startState, startDepth, maxDepth, new CancellationTokenSource(timeout).Token);

        public SearchResult IterativeSearch(IDeterministicState startState, int startDepth, int maxDepth, CancellationToken cancellationToken)
        {
            if (startDepth >= maxDepth)
                throw new Exception($"{nameof(startDepth)} (== {startDepth}) must be bigger than {nameof(maxDepth)} ( == {maxDepth})");

            SearchResult bestResultSoFar = null;
            for (int i = startDepth; i < maxDepth; i++)
            { 
                var result = Search(startState, i, cancellationToken);
                if (!cancellationToken.IsCancellationRequested || bestResultSoFar == null)
                    bestResultSoFar = result;
                if (cancellationToken.IsCancellationRequested)
                    break;
                if (result.AllChildrenAreDeadEnds)
                    break; // No point searching any deeper
            }
            return bestResultSoFar;
        }
    }
}
