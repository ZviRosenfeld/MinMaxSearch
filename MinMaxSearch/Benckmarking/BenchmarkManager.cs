using System;

namespace MinMaxSearch.Benckmarking
{
    public static class BenchmarkManager
    {
        public static BenckmarkResult[] Benchmark(this SearchEngine searchEngine, IState startState, int searchDepth, int times)
        {
            var results = new BenckmarkResult[times];

            for (int i = 0; i < times; i++)
            {
                var startTime = DateTime.Now;
                var result = searchEngine.Search(startState, Player.Max, searchDepth);
                var endTime = DateTime.Now;
                results[i] = new BenckmarkResult(endTime - startTime, result.Leaves, result.InternalNodes);
            }

            return results;
        }

        public static void Print(this BenckmarkResult[] results)
        {
            for (int i = 0; i < results.Length; i++)
            {
                Console.WriteLine("Run " + i);
                Console.Write(results[i].ToString());
                Console.WriteLine("");
            }
        }
    }
}
