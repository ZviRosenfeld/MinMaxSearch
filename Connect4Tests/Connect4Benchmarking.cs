using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch;
using MinMaxSearch.Benckmarking;
using System;

namespace Connect4Tests
{
    [TestClass]
    [TestCategory("Benchmarking")]
    public class Connect4Benchmarking
    {
        [TestMethod]
        public void BenchmarkConnect4()
        {
            BenchmarkWithDegreeOfParallelism(1);
            BenchmarkWithDegreeOfParallelism(2);
            BenchmarkWithDegreeOfParallelism(4);
        }

        private void BenchmarkWithDegreeOfParallelism(int degreeOfParallelism)
        {
            Console.WriteLine("Running with degreeOfParallelism: " + degreeOfParallelism);
            var engine = Connect4TestUtils.GetSearchEngine(degreeOfParallelism);
            var startState = new Connect4State(Connect4TestUtils.GetEmptyBoard(), Player.Max);

            var results = engine.Benchmark(startState, 11, 1);
            results.Print();
        }
    }
}
