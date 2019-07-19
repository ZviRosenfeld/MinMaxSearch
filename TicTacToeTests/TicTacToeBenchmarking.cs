using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch;
using MinMaxSearch.Benckmarking;

namespace TicTacToeTests
{
    [TestClass]
    [TestCategory("Benchmarking")]
    public class TicTacToeBenchmarking
    {
        [TestMethod]
        public void BenchmarkTicTacToe()
        {
            BenchmarkWithDegreeOfParallelism(1);
            BenchmarkWithDegreeOfParallelism(2);
            BenchmarkWithDegreeOfParallelism(8);
        }

        private void BenchmarkWithDegreeOfParallelism(int degreeOfParallelism)
        {
            Console.WriteLine("Running with degreeOfParallelism: " + degreeOfParallelism);
            var engine = TicTacToeBassicTests.GetSearchEngine(degreeOfParallelism, ParallelismMode.FirstLevelOnly);
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
