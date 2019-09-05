using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
        /// The search will end once we find a score better then MaxScore for Max or worse then MinScore for Min
        /// </summary>
        public bool DieEarly { get; set; }

        public double MaxScore { get; set; } = double.MaxValue;

        public double MinScore { get; set; } = double.MinValue;

        private int maxDegreeOfParallelism = 1;

        /// <summary>
        /// Note that this will only have an effect if ParallelismMode is set to TotalParallelism
        /// </summary>
        public int MaxDegreeOfParallelism
        {
            get => maxDegreeOfParallelism;
            set => maxDegreeOfParallelism = value > 0
                ? value
                : throw new BadDegreeOfParallelismException("DegreeOfParallelism must be at least one. Tried to set it to " + maxDegreeOfParallelism);
        }

        public ParallelismMode ParallelismMode { get; set; } = ParallelismMode.FirstLevelOnly;

        public Func<IState, int, List<IState>, double> AlternateEvaluation { get; set; }

        private SearchOptions CreateSearchOptions() => new SearchOptions(pruners, IsUnstableState, PreventLoops,
            FavorShortPaths, DieEarly, MaxScore, MinScore, AlternateEvaluation);

        private IThreadManager GetThreadManager()
        {
            if (ParallelismMode == ParallelismMode.FirstLevelOnly)
                return new FirstLevelOnlyThreadManager();

            if (ParallelismMode == ParallelismMode.NonParallelism || maxDegreeOfParallelism == 1)
                return new SequencelThreadManager();

            return new ThreadManager(maxDegreeOfParallelism);
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
            var searchWorker = new SearchWorker(CreateSearchOptions(), GetThreadManager());
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = searchWorker.Evaluate(startState, searchContext);
            stopwatch.Stop();
            result.StateSequence.Reverse();
            result.StateSequence.RemoveAt(0); // Removing the top node will make the result "nicer"
            return new SearchResult(result, stopwatch.Elapsed, maxDepth, !cancellationToken.IsCancellationRequested);
        }
    }
}
