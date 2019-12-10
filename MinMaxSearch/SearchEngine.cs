using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MinMaxSearch.Cache;
using MinMaxSearch.Exceptions;
using MinMaxSearch.Pruners;
using MinMaxSearch.ThreadManagment;

namespace MinMaxSearch
{
    public class SearchEngine : ISearchEngine
    {
        private readonly List<IPruner> pruners = new List<IPruner>();

        public SearchEngine AddPruner(IPruner pruner)
        {
            pruners.Add(pruner);
            return this;
        }

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
        /// The search will end once we find a score that is bigger then or equal to SearchEngine.MaxScore for Max or smaller or equal to SearchEngine.MinScore for Min.
        /// </summary>
        public bool DieEarly { get; set; }

        /// <summary>
        /// Any score equal to or bigger than MaxScore is considered a win for Max
        /// </summary>
        public double MaxScore { get; set; } = double.MaxValue;

        /// <summary>
        /// Any score equal to or smaller than MinScore is considered a win for Min
        /// </summary>
        public double MinScore { get; set; } = double.MinValue;
        
        /// <summary>
        /// Note that this will only have an effect if ParallelismMode is set to TotalParallelism
        /// </summary>
        public int MaxDegreeOfParallelism { get; set; } = 1;

        /// <summary>
        /// Note that this will only have an effect if ParallelismMode is set to ParallelismByLevel
        /// </summary>
        public int MaxLevelOfParallelism { get; set; } = 1;

        public ParallelismMode ParallelismMode { get; set; } = ParallelismMode.FirstLevelOnly;

        public Func<IState, int, List<IState>, double> AlternateEvaluation { get; set; }

        private SearchOptions CreateSearchOptions() => new SearchOptions(pruners, IsUnstableState, PreventLoops,
            FavorShortPaths, DieEarly, MaxScore, MinScore, AlternateEvaluation);

        /// <summary>
        /// In some search domains, remembering states that lead to wins losses or draw can improve performance
        /// </summary>
        public CacheMode CacheMode { get; set; } = CacheMode.NewCache;

        private ICacheManager cacheManager = new CacheManager();

        /// <summary>
        /// Note that the cache will only be filled in the first place if CacheMode is set to ReuseCache
        /// </summary>
        public void ClearCache() => cacheManager.Clear();

        private IThreadManager GetThreadManager(int searchDepth)
        {
            if (ParallelismMode == ParallelismMode.FirstLevelOnly)
                return new LevelParallelismThreadManager(1);

            if (ParallelismMode == ParallelismMode.ParallelismByLevel)
                return new LevelParallelismThreadManager(MaxLevelOfParallelism);

            if (ParallelismMode == ParallelismMode.NonParallelism || MaxDegreeOfParallelism == 1)
                return new SequencelThreadManager();

            return new TotalParallelismThreadManager(MaxDegreeOfParallelism, searchDepth);
        }

        public SearchEngine Clone()
        {
            var newEngine = new SearchEngine()
            {
                AlternateEvaluation = AlternateEvaluation,
                DieEarly = DieEarly,
                FavorShortPaths = FavorShortPaths,
                IsUnstableState = IsUnstableState,
                MaxDegreeOfParallelism = MaxDegreeOfParallelism,
                MaxScore = MaxScore,
                MinScore = MinScore,
                PreventLoops = PreventLoops,
            };
            foreach (var pruner in pruners)
                newEngine.AddPruner(pruner);

            return newEngine;
        }
        
        /// <summary>
        /// Runs a search.
        /// </summary>
        /// <param name="startState"> The state that the search will start from</param>
        /// <param name="maxDepth"> The search will be terminated after maxDepth</param>
        public SearchResult Search(IDeterministicState startState, int maxDepth) =>
            Search(startState, maxDepth, CancellationToken.None);

        /// <summary>
        /// Runs a search asynchronously.
        /// </summary>
        /// <param name="startState"> The state that the search will start from</param>
        /// <param name="maxDepth"> The search will be terminated after maxDepth</param>
        /// <param name="cancellationToken"> Used to cancel the search</param>
        public Task<SearchResult> SearchAsync(IDeterministicState startState, int maxDepth, CancellationToken cancellationToken) => 
            Task.Run(() => Search(startState, maxDepth, cancellationToken));

        /// <summary>
        /// Runs a search.
        /// </summary>
        /// <param name="startState"> The state that the search will start from</param>
        /// <param name="maxDepth"> The search will be terminated after maxDepth</param>
        /// <param name="cancellationToken"> Used to cancel the search</param>
        public SearchResult Search(IDeterministicState startState, int maxDepth, CancellationToken cancellationToken)
        {
            if (!startState.GetNeighbors().Any())
                throw new NoNeighborsException("start state has no neighbors " + startState);
            
            if (maxDepth < 1)
                throw new ArgumentException($"{nameof(maxDepth)} must be at least 1. Was {maxDepth}");
            
            var searchContext = new SearchContext(maxDepth, 0, cancellationToken);
            ICacheManager cacheManager;
            if (CacheMode == CacheMode.NoCache)
                cacheManager = new NullCacheManager();
            else if (CacheMode == CacheMode.NewCache)
                cacheManager = new CacheManager();
            else
                cacheManager = this.cacheManager;
            var searchWorker = new SearchWorker(CreateSearchOptions(), GetThreadManager(maxDepth), cacheManager);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = searchWorker.Evaluate(startState, searchContext);
            stopwatch.Stop();
            result.StateSequence.Reverse();
            result.StateSequence.RemoveAt(0); // Removing the top node will make the result "nicer"
            return new SearchResult(result, stopwatch.Elapsed, maxDepth, !cancellationToken.IsCancellationRequested);
        }

        [Obsolete("Please use " + nameof(IterativeSearchWrapper) + " for iterative searches")]
        public SearchResult IterativeSearch(IDeterministicState startState, int startDepth, int maxDepth, TimeSpan timeout) =>
            IterativeSearch(startState, startDepth, maxDepth, new CancellationTokenSource(timeout).Token);

        [Obsolete("Please use " + nameof(IterativeSearchWrapper) + " for iterative searches")]
        public SearchResult IterativeSearch(IDeterministicState startState, int startDepth, int maxDepth, CancellationToken cancellationToken) =>
            new IterativeSearchWrapper(this).IterativeSearch(startState, startDepth, maxDepth, cancellationToken);

    }
}
