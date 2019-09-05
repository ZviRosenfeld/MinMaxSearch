using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using MinMaxSearch.Range;

namespace MinMaxSearch
{
    public class IterativeSearchWrapper
    {
        private readonly ISearchEngine searchEngine;

        public IterativeSearchWrapper(ISearchEngine searchEngine)
        {
            this.searchEngine = searchEngine;
        }

        /// <summary>
        /// Runs an iterative deepening search.
        /// In Iterative search, a depth-limited version of depth-first search is run repeatedly with increasing depth limits till some condition is met.
        /// </summary>
        /// <param name="startState"> The state that the search will start from</param>
        /// <param name="startDepth"> The first search's depth</param>
        /// <param name="maxDepth"> The max search depth</param>
        /// <param name="timeout"> After timeout time the search will be terminated. We will return the best solution found in the deepest completed search (or, if the first search wasn't completed, the results found so far in the first search).</param>
        public SearchResult IterativeSearch(IDeterministicState startState, int startDepth, int maxDepth, TimeSpan timeout) =>
            IterativeSearch(startState, startDepth, maxDepth, new CancellationTokenSource(timeout).Token);

        /// <summary>
        /// Runs an iterative deepening search.
        /// In Iterative search, a depth-limited version of depth-first search is run repeatedly with increasing depth limits till some condition is met.
        /// </summary>
        /// <param name="startState"> The state that the search will start from</param>
        /// <param name="startDepth"> The first search's depth</param>
        /// <param name="maxDepth"> The max search depth</param>
        /// <param name="cancellationToken"> Used to terminate the search. We will return the best solution found in the deepest completed search (or, if the first search wasn't completed, the results found so far in the first search).</param>
        public SearchResult IterativeSearch(IDeterministicState startState, int startDepth, int maxDepth,
            CancellationToken cancellationToken)
        {
            if (startDepth >= maxDepth)
                throw new Exception($"{nameof(startDepth)} (== {startDepth}) must be bigger than {nameof(maxDepth)} ( == {maxDepth})");

            return IterativeSearch(startState, new RangeEnumerable(startDepth, maxDepth), cancellationToken);
        }

        /// <summary>
        /// Runs an iterative deepening search.
        /// In Iterative search, a depth-limited version of depth-first search is run repeatedly with increasing depth limits till some condition is met.
        /// </summary>
        /// <param name="startState"> The state that the search will start from</param>
        /// <param name="depths"> A lists of the depths to check</param>
        /// <param name="cancellationToken"> Used to terminate the search. We will return the best solution found in the deepest completed search (or, if the first search wasn't completed, the results found so far in the first search).</param>
        public SearchResult IterativeSearch(IDeterministicState startState, IEnumerable<int> depths, CancellationToken cancellationToken)
        {
            var stopewatch = new Stopwatch();
            stopewatch.Start();

            SearchResult bestResultSoFar = null;
            int maxDepth = -1;
            foreach (var depth in depths)
            {
                if (depth < maxDepth) continue;

                var result = searchEngine.Search(startState, depth, cancellationToken);
                if (!cancellationToken.IsCancellationRequested || bestResultSoFar == null)
                {
                    bestResultSoFar = result;
                    maxDepth = depth;
                }
                else break;

                if (result.AllChildrenAreDeadEnds)
                    break; // No point searching any deeper
            }
            stopewatch.Stop();

            return new SearchResult(bestResultSoFar, stopewatch.Elapsed, maxDepth, bestResultSoFar.IsSearchCompleted);
        }
    }
}
