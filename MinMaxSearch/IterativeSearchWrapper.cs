using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using MinMaxSearch.Exceptions;
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
        /// <param name="cancellationToken"> Terminates the search immediately. We will return the best solution found in the deepest completed search (or, if the first search wasn't completed, the results found so far in the first search).</param>
        public SearchResult IterativeSearch(IDeterministicState startState, int startDepth, int maxDepth,
            CancellationToken cancellationToken) =>
            IterativeSearch(startState, startDepth, maxDepth, cancellationToken, CancellationToken.None);

        /// <summary>
        /// Runs an iterative deepening search.
        /// In Iterative search, a depth-limited version of depth-first search is run repeatedly with increasing depth limits till some condition is met.
        /// </summary>
        /// <param name="startState"> The state that the search will start from</param>
        /// <param name="startDepth"> The first search's depth</param>
        /// <param name="maxDepth"> The max search depth</param>
        /// <param name="cancellationToken"> Terminates the search immediately. We will return the best solution found in the deepest completed search (or, if the first search wasn't completed, the results found so far in the first search).</param>
        /// <param name="cancellationTokenAfterFirstSearch">This token will terminate the search once the first search was complete (or immediately if the first search has already been completed).</param>
        public SearchResult IterativeSearch(IDeterministicState startState, int startDepth, int maxDepth,
            CancellationToken cancellationToken, CancellationToken cancellationTokenAfterFirstSearch)
        {
            if (startDepth >= maxDepth)
                throw new Exception($"{nameof(startDepth)} (== {startDepth}) must be bigger than {nameof(maxDepth)} ( == {maxDepth})");

            return IterativeSearch(startState, new RangeEnumerable(startDepth, maxDepth), cancellationToken, cancellationTokenAfterFirstSearch);
        }

        /// <summary>
        /// Runs an iterative deepening search.
        /// In Iterative search, a depth-limited version of depth-first search is run repeatedly with increasing depth limits till some condition is met.
        /// </summary>
        /// <param name="startState"> The state that the search will start from</param>
        /// <param name="depths"> A lists of the depths to check</param>
        /// <param name="cancellationToken"> Terminates the search immediately. We will return the best solution found in the deepest completed search (or, if the first search wasn't completed, the results found so far in the first search).</param>
        public SearchResult IterativeSearch(IDeterministicState startState, IEnumerable<int> depths, CancellationToken cancellationToken) =>
            IterativeSearch(startState, depths, cancellationToken, CancellationToken.None);

        /// <summary>
        /// Runs an iterative deepening search.
        /// In Iterative search, a depth-limited version of depth-first search is run repeatedly with increasing depth limits till some condition is met.
        /// </summary>
        /// <param name="startState"> The state that the search will start from</param>
        /// <param name="depths"> A lists of the depths to check</param>
        /// <param name="cancellationToken"> Terminates the search immediately. We will return the best solution found in the deepest completed search (or, if the first search wasn't completed, the results found so far in the first search).</param>
        /// <param name="cancellationTokenAfterFirstSearch">This token will terminate the search once the first search was complete (or immediately if the first search has already been completed).</param>
        public SearchResult IterativeSearch(IDeterministicState startState, IEnumerable<int> depths, CancellationToken cancellationToken, CancellationToken cancellationTokenAfterFirstSearch)
        {
            var stopewatch = new Stopwatch();
            stopewatch.Start();
            var linkedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cancellationTokenAfterFirstSearch).Token;

            SearchResult bestResultSoFar = null;
            int maxDepth = -1;
            foreach (var depth in depths)
            {
                if (depth < maxDepth) continue;
                else maxDepth = depth;

                var result = searchEngine.Search(startState, depth, bestResultSoFar == null ? cancellationToken : linkedCancellationToken);
                if (linkedCancellationToken.IsCancellationRequested)
                {
                    if (bestResultSoFar == null)
                        bestResultSoFar = result;

                    break;
                }

                bestResultSoFar = result;

                if (result.FullTreeSearchedOrPrunned)
                    break; // No point searching any deeper
            }
            stopewatch.Stop();

            if (bestResultSoFar == null)
                throw new InternalException($"Code 1000 ({nameof(bestResultSoFar)} is null)");

            return new SearchResult(bestResultSoFar, stopewatch.Elapsed, maxDepth, bestResultSoFar.IsSearchCompleted);
        }
    }
}
