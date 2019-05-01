using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch;
using MinMaxSearch.Banckmarking;

namespace TicTacToeTests
{
    [TestClass]
    public class TicTacToeBenchmarking
    {
        [TestMethod]
        [TestCategory("Benchmarking")]
        public void BenchmarkTicTacToe()
        {
            BenchmarkWithDegreeOfParallelism(1);
            BenchmarkWithDegreeOfParallelism(2);
            BenchmarkWithDegreeOfParallelism(8);
        }

        private void BenchmarkWithDegreeOfParallelism(int degreeOfParallelism)
        {
            Console.WriteLine("Running with degreeOfParallelism: " + degreeOfParallelism);
            var engine = TicTacToeBassicTests.GetSearchEngine(degreeOfParallelism);
            var startState = new TicTacToeState(new[,]
            {
                { Player.Empty, Player.Empty, Player.Empty},
                { Player.Empty, Player.Empty, Player.Empty},
                { Player.Empty, Player.Empty, Player.Empty},
            }, Player.Max);

            var results = engine.Benchmark(startState, 10, 1);
            results.Print();
        }
    }
}
