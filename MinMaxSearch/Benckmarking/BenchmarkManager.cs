using System;

namespace MinMaxSearch.Benckmarking
{
    public static class BenchmarkManager
    {
        /// <summary>
        /// Performs a search and return statistics about it (mainly how long it took)
        /// </summary>
        public static BenckmarkResult Benchmark(this SearchEngine searchEngine, IDeterministicState startState, int searchDepth)
        {
            var startTime = DateTime.Now;
            var result = searchEngine.Search(startState, searchDepth);
            var endTime = DateTime.Now;
            return new BenckmarkResult(endTime - startTime, result.Leaves, result.InternalNodes);
        }
    }
}
