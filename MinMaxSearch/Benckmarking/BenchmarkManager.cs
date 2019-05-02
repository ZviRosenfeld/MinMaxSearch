using System;
using System.Threading;
using System.Threading.Tasks;

namespace MinMaxSearch.Benckmarking
{
    public static class BenchmarkManager
    {
        public static BenckmarkResult Benchmark(this SearchEngine searchEngine, IState startState, int searchDepth)
        {
            var startTime = DateTime.Now;
            var result = searchEngine.Search(startState, Player.Max, searchDepth);
            var endTime = DateTime.Now;
            return new BenckmarkResult(endTime - startTime, result.Leaves, result.InternalNodes);

        }
    }
}
