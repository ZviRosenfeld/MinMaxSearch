using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch;

namespace TicTacToeTests
{
    [TestClass]
    [TestCategory("Benchmarking")]
    public class TicTacToeBenchmarking
    {
        [TestMethod]
        public void BenchmarkTicTacToe()
        {
            BenchmarkWithDegreeOfParallelism(1, ParallelismMode.NonParallelism);
            BenchmarkWithDegreeOfParallelism(1, ParallelismMode.FirstLevelOnly);
            BenchmarkWithDegreeOfParallelism(4, ParallelismMode.TotalParallelism);
        }

        private void BenchmarkWithDegreeOfParallelism(int degreeOfParallelism, ParallelismMode parallelismMode)
        {
            Console.WriteLine("Running with degreeOfParallelism: " + degreeOfParallelism + ", Mode: " + parallelismMode);
            var engine = TicTacToeBassicTests.GetSearchEngine(degreeOfParallelism, parallelismMode);
            var startState = new TicTacToeState(new[,]
            {
                { Player.Empty, Player.Empty, Player.Empty},
                { Player.Empty, Player.Empty, Player.Empty},
                { Player.Empty, Player.Empty, Player.Empty},
            }, Player.Max);

            var results = engine.Search(startState, 10);
            Console.WriteLine("Time: " + results.SearchTime);
            Console.WriteLine("Leaves: " + results.Leaves);
            Console.WriteLine("InternalNodes: " + results.InternalNodes);
        }
    }
}
