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
        public SearchEngine()
        {
            cacheManagerFactory = () => new NullCacheManager();
        }

        /// <summary>
        /// In some search domains, remembering states that lead to wins, losses or draw can improve performance.
        /// 
        /// Use this constructor to create an engine with a cache.
        /// 
        /// Note that caching will only work if you implement Equals and GetHashValue in a meaningful way for your states.
        /// </summary>
        public SearchEngine(CacheMode cacheMode, CacheKeyType cacheKeyType)
        {
            CacheMode = cacheMode;

            if (cacheMode == CacheMode.NoCache)
                throw new MinMaxSearchException($"Can't set {nameof(cacheMode)} to {CacheMode.NoCache} when using a cache. If you don't want to use a cache, please use the empty constructor.");
            else if (cacheMode == CacheMode.ReuseCache)
            {
                CacheManager = GetCacheManager(cacheKeyType);
                cacheManagerFactory = () => CacheManager;
            }
            else
                cacheManagerFactory = () => GetCacheManager(cacheKeyType);
        }

        private ICacheManager GetCacheManager(CacheKeyType cacheKeyType)
        {
            switch (cacheKeyType)
            {
                case CacheKeyType.StateOnly: return new StateCacheManager();
                case CacheKeyType.StateAndDepth: return new StateAndDepthCacheManager();
                case CacheKeyType.StateAndPassedThroughStates: return new StateAndPassedThroughCacheManager();
                default: throw new InternalException("Code 1005 (not supported cache type)");
            }
        }

        /// <summary>
        /// In some search domains, remembering states that lead to wins, losses or draw can improve performance.
        /// 
        /// Use this constructor to create an engine with a custom cache.
        /// 
        /// Note that caching will only work if you implement Equals and GetHashValue in a meaningful way for your states.
        /// </summary>
        public SearchEngine(CacheMode cacheMode, Func<ICacheManager> cacheManagerFactory)
        {
            CacheMode = cacheMode;
            if (cacheMode == CacheMode.NoCache)
                throw new MinMaxSearchException($"Can't set {nameof(cacheMode)} to {CacheMode.NoCache} when using a cache. If you don't want to use a cache, please use the empty constructor.");
            else if (cacheMode == CacheMode.ReuseCache)
            {
                CacheManager = cacheManagerFactory();
                this.cacheManagerFactory = () => CacheManager;
            }
            else
                this.cacheManagerFactory = cacheManagerFactory;
        }

        private readonly List<IPruner> pruners = new List<IPruner>();

        public SearchEngine AddPruner(IPruner pruner)
        {
            pruners.Add(pruner);
            return this;
        }

        /// <summary>
        /// At unstable states, we'll continue searching even after we've hit the maxDepth limit
        /// </summary>
        public Func<IState, int, List<IState>, bool> IsUnstableState { get; set; } = null;

        /// <summary>
        /// Note that this will only work if you implement Equals and GetHashValue in a meaningful way for your states. 
        /// </summary>
        public bool PreventLoops { get; set; }

        /// <summary>
        /// If two path give the same score, but one is shorter then the other - we'll take the shorter one
        /// </summary>
        public bool FavorShortPaths { get; set; } = true;

        /// <summary>
        /// The search will end once we find a score that is greater than or equal to SearchEngine.MaxScore for Max or smaller or equal to SearchEngine.MinScore for Min.
        /// </summary>
        public bool DieEarly { get; set; }

        /// <summary>
        /// Any score equal to or greater than MaxScore is considered a win for Max
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
            FavorShortPaths, DieEarly, MaxScore, MinScore, AlternateEvaluation, StateDefinesDepth, CacheMode);

        public CacheMode CacheMode { get; } = CacheMode.NoCache;

        public CacheKeyType CacheKeyType { get; } = CacheKeyType.StateOnly;

        /// <summary>
        /// Note that this CacheManager will only be used if CacheMode is set to ReuseCache.
        /// Otherwise, the engine will initialize a new cache for each search.
        /// </summary>
        public ICacheManager CacheManager { get; }

        private Func<ICacheManager> cacheManagerFactory;

        /// <summary>
        /// If this is set to true, in the case that the first node has a single neighbor, the engine will return that neighbor rather than evaluation the search tree.
        /// Note that this only applies to the first node.
        /// </summary>
        public bool SkipEvaluationForFirstNodeSingleNeighbor { get; set; } = true;

        /// <summary>
        /// Set this to true it is possible to infer a state's depth from the state alone.
        /// This is trues for games like tic-tac-toe and connect4, where the depth of a state is the number of tokens on the board.
        /// The engine will use this knowledge to optimize the search
        /// </summary>
        public bool StateDefinesDepth { get; set; } = false;

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

        public SearchEngine CloneWithCacheManager(ICacheManager cacheManager)
        {
            var newEngine = new SearchEngine(CacheMode, () => cacheManager);
            CopySearchOptions(newEngine);
            return newEngine;
        }

        public SearchEngine Clone()
        {
            var newEngine = new SearchEngine(CacheMode, CacheKeyType);
            CopySearchOptions(newEngine);
            return newEngine;
        }

        private void CopySearchOptions(SearchEngine newEngine)
        {
            newEngine.AlternateEvaluation = AlternateEvaluation;
            newEngine.DieEarly = DieEarly;
            newEngine.FavorShortPaths = FavorShortPaths;
            newEngine.IsUnstableState = IsUnstableState;
            newEngine.MaxDegreeOfParallelism = MaxDegreeOfParallelism;
            newEngine.MaxLevelOfParallelism = MaxLevelOfParallelism;
            newEngine.MaxScore = MaxScore;
            newEngine.MinScore = MinScore;
            newEngine.PreventLoops = PreventLoops;
            newEngine.ParallelismMode = ParallelismMode;
            newEngine.SkipEvaluationForFirstNodeSingleNeighbor = SkipEvaluationForFirstNodeSingleNeighbor;
            foreach (var pruner in pruners)
                newEngine.AddPruner(pruner);
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
            
            if (SkipEvaluationForFirstNodeSingleNeighbor && startState.GetNeighbors().Count() == 1)
            {
                var singleNeighbor = startState.GetNeighbors().First();
                return new SearchResult(singleNeighbor.Evaluate(0, new List<IState>(), CreateSearchOptions()), startState, true, true, false);
            }

            var searchContext = new SearchContext(maxDepth, 0, cancellationToken);
            var searchWorker = new SearchWorker(CreateSearchOptions(), GetThreadManager(maxDepth), cacheManagerFactory());
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
