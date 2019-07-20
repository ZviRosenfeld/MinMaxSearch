using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch;
using System;

namespace Connect4Tests
{
    [TestClass]
    [TestCategory("Benchmarking")]
    public class Connect4Benchmarking
    {
        private const int SearchDepth = 11;

        [TestMethod]
        public void BenchmarkConnect4()
        {
            BenchmarkWithDegreeOfParallelism(1, ParallelismMode.NonParallelism);
            BenchmarkWithDegreeOfParallelism(1, ParallelismMode.FirstLevelOnly);
            BenchmarkWithDegreeOfParallelism(4, ParallelismMode.TotalParallelism);
        }

        private void BenchmarkWithDegreeOfParallelism(int degreeOfParallelism, ParallelismMode parallelismMode)
        {
            Console.WriteLine("Running with degreeOfParallelism: " + degreeOfParallelism + ", Mode: " + parallelismMode);
            var engine = Connect4TestUtils.GetSearchEngine(degreeOfParallelism, parallelismMode);
            var startState = new Connect4State(Connect4TestUtils.GetEmptyBoard(), Player.Max);

            var results = engine.Search(startState, SearchDepth);
            Console.WriteLine("Time: " + results.SearchTime);
            Console.WriteLine("Leaves: " + results.Leaves);
            Console.WriteLine("InternalNodes: " + results.InternalNodes);
        }
    }
}
