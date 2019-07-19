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
    public class SearchEngine
    {

        public SearchEngine(SearchEngine engine)
        {
            AlternateEvaluation = engine.AlternateEvaluation;
            DieEarly = engine.DieEarly;
            FavorShortPaths = engine.FavorShortPaths;
            IsUnstableState = engine.IsUnstableState;
            MaxDegreeOfParallelism = engine.MaxDegreeOfParallelism;
            MaxScore = engine.MaxScore;
            MinScore = engine.MinScore;
            PreventLoops = engine.PreventLoops;
            pruners = new List<IPruner>(engine.pruners);
        }

        public SearchEngine()
        {
        }

        private readonly List<IPruner> pruners = new List<IPruner>();
        
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

        public ParallelismMode ParallelismMode { get; set; } = ParallelismMode.FirstLevelOnly;

        public Func<IState, int, List<IState>, double> AlternateEvaluation { get; set; }

        public SearchResult Search(IDeterministicState startState, int maxDepth) =>
            Search(startState, maxDepth, CancellationToken.None);

        public Task<SearchResult> SearchAsync(IDeterministicState startState, int maxDepth, CancellationToken cancellationToken) => 
            Task.Run(() => Search(startState, maxDepth, cancellationToken));

        public SearchResult Search(IDeterministicState startState, int maxDepth, CancellationToken cancellationToken)
        {
            if (!startState.GetNeighbors().Any())
                throw new NoNeighborsException("start state has no neighbors " + startState);
            
            var searchWorker = new SearchWorker(CreateSearchOptions(), GetThreadManager());
            var searchContext = new SearchContext(maxDepth, 0, cancellationToken);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var evaluation = searchWorker.Evaluate(startState, searchContext);
            stopwatch.Stop();
            evaluation.StateSequence.Reverse();
            evaluation.StateSequence.RemoveAt(0); // Removing the top node will make the result "nicer"
            return new SearchResult(evaluation, stopwatch.Elapsed);
        }
        
        private SearchOptions CreateSearchOptions() => new SearchOptions(pruners, IsUnstableState, PreventLoops,
            FavorShortPaths, DieEarly, MaxScore, MinScore, AlternateEvaluation);

        private IThreadManager GetThreadManager()
        {
            if (ParallelismMode == ParallelismMode.NonParallelism)
                return new ThreadManager(1);

            return new ThreadManager(maxDegreeOfParallelism);
        }

        public SearchResult IterativeSearch(IDeterministicState startState, int startDepth, int maxDepth, TimeSpan timeout) =>
            IterativeSearch(startState, startDepth, maxDepth, new CancellationTokenSource(timeout).Token);

        public SearchResult IterativeSearch(IDeterministicState startState, int startDepth, int maxDepth, CancellationToken cancellationToken)
        {
            if (startDepth >= maxDepth)
                throw new Exception($"{nameof(startDepth)} (== {startDepth}) must be bigger than {nameof(maxDepth)} ( == {maxDepth})");

            var stopewatch = new Stopwatch();
            stopewatch.Start();

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
            stopewatch.Stop();

            return new SearchResult(bestResultSoFar, stopewatch.Elapsed);
        }
    }
}
