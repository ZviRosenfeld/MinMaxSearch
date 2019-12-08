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
            BenchmarkWithDegreeOfParallelism(ParallelismMode.NonParallelism);
            BenchmarkWithDegreeOfParallelism(ParallelismMode.FirstLevelOnly);
            BenchmarkWithDegreeOfParallelism(ParallelismMode.ParallelismByLevel, levelOfParallelism: 2);
            BenchmarkWithDegreeOfParallelism(ParallelismMode.TotalParallelism, 4);
        }

        private void BenchmarkWithDegreeOfParallelism(ParallelismMode parallelismMode, int degreeOfParallelism = 1, int levelOfParallelism = 1)
        {
            Console.WriteLine("Running with degreeOfParallelism: " + degreeOfParallelism + ", Mode: " + parallelismMode);
            var engine = Connect4TestUtils.GetSearchEngine(degreeOfParallelism, parallelismMode, levelOfParallelism);
            var startState = new Connect4State(Connect4TestUtils.GetEmptyBoard(), Player.Max);

            var results = engine.Search(startState, SearchDepth);
            Console.WriteLine("Time: " + results.SearchTime);
            Console.WriteLine("Leaves: " + results.Leaves);
            Console.WriteLine("InternalNodes: " + results.InternalNodes);
        }
    }
}
