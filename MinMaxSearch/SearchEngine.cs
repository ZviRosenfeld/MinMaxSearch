using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MinMaxSearch.Range;

namespace MinMaxSearch
{
    public class SearchEngine
    {
        private ISearchWorker searchWorker;
        
        [Obsolete("Please use " + nameof(SearchEngineBuilder))]
        public SearchEngine(ISearchWorker searchWorker)
        {
            this.searchWorker = searchWorker;
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

            //var searchWorker = new SearchWorker(CreateSearchOptions(), GetThreadManager());
            var searchContext = new SearchContext(maxDepth, 0, cancellationToken);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = searchWorker.Evaluate(startState, searchContext);
            stopwatch.Stop();
            result.StateSequence.Reverse();
            result.StateSequence.RemoveAt(0); // Removing the top node will make the result "nicer"
            return new SearchResult(result, stopwatch.Elapsed, maxDepth);
        }
        
        /// <summary>
        /// Runs an Iterative deepening search.
        /// In Iterative search, a depth-limited version of depth-first search is run repeatedly with increasing depth limits till some condition is met.
        /// </summary>
        /// <param name="startState"> The state that the search will start from</param>
        /// <param name="startDepth"> The first search's depth</param>
        /// <param name="maxDepth"> The max search depth</param>
        /// <param name="timeout"> After timeout time the search will be terminated. We will return the best solution found in the deepest completed search(or null if no search was completed).</param>
        public SearchResult IterativeSearch(IDeterministicState startState, int startDepth, int maxDepth, TimeSpan timeout) =>
            IterativeSearch(startState, startDepth, maxDepth, new CancellationTokenSource(timeout).Token);

        /// <summary>
        /// Runs an Iterative deepening search.
        /// In Iterative search, a depth-limited version of depth-first search is run repeatedly with increasing depth limits till some condition is met.
        /// </summary>
        /// <param name="startState"> The state that the search will start from</param>
        /// <param name="depths"> A lists of the depths to check</param>
        /// <param name="cancellationToken"> Used to terminate the search. We will return the best solution found in the deepest completed search (or null if no search was completed).</param>
        public SearchResult IterativeSearch(IDeterministicState startState, IEnumerable<int> depths, CancellationToken cancellationToken)
        {
            var stopewatch = new Stopwatch();
            stopewatch.Start();

            SearchResult bestResultSoFar = null;
            int maxDepth = -1;
            foreach (var depth in depths)
            {
                if (depth < maxDepth) continue;

                var result = Search(startState, depth, cancellationToken);
                if (!cancellationToken.IsCancellationRequested)
                {
                    bestResultSoFar = result;
                    maxDepth = depth;
                }
                else break;

                if (result.AllChildrenAreDeadEnds)
                    break; // No point searching any deeper
            }
            stopewatch.Stop();

            return bestResultSoFar == null
                ? null
                : new SearchResult(bestResultSoFar, stopewatch.Elapsed, maxDepth);
        }

        /// <summary>
        /// Runs an Iterative deepening search.
        /// In Iterative search, a depth-limited version of depth-first search is run repeatedly with increasing depth limits till some condition is met.
        /// </summary>
        /// <param name="startState"> The state that the search will start from</param>
        /// <param name="startDepth"> The first search's depth</param>
        /// <param name="maxDepth"> The max search depth</param>
        /// <param name="cancellationToken"> Used to terminate the search. We will return the best solution found in the deepest completed search (or null if no search was completed).</param>
        public SearchResult IterativeSearch(IDeterministicState startState, int startDepth, int maxDepth,
            CancellationToken cancellationToken)
        {
            if (startDepth >= maxDepth)
                throw new Exception($"{nameof(startDepth)} (== {startDepth}) must be bigger than {nameof(maxDepth)} ( == {maxDepth})");

            return IterativeSearch(startState, new RangeEnumerable(startDepth, maxDepth), cancellationToken);
        }
    }
}
