using System;

namespace MinMaxSearch.Benckmarking
{
    public static class BenchmarkManager
    {
        public static BenckmarkResult Benchmark(this SearchEngine searchEngine, IDeterministicState startState, int searchDepth)
        {
            var startTime = DateTime.Now;
            var result = searchEngine.Search(startState, searchDepth);
            var endTime = DateTime.Now;
            return new BenckmarkResult(endTime - startTime, result.Leaves, result.InternalNodes);
        }
    }
}
