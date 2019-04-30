using System;
using Connect4Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch;
using TicTacToeTests;

namespace Benchmarking
{
    [TestClass]
    public class Benchmarking
    {
        [TestMethod]
        [TestCategory("Benchmarking")]
        public void BenchmarkTicTacToe()
        {
            var engine = TicTacToeBassicTests.GetSearchEngine();
            var startState = new TicTacToeState(new[,]
            {
                { Player.Empty, Player.Empty, Player.Empty},
                { Player.Empty, Player.Empty, Player.Empty},
                { Player.Empty, Player.Empty, Player.Empty},
            }, Player.Max);

            PrintBenchmarkData(engine, startState, 10);
        }

        [TestMethod]
        [TestCategory("Benchmarking")]
        public void BenchmarkConnect4()
        {
            var engine = Connect4TestUtils.GetSearchEngine();
            var startState = new Connect4State(Connect4TestUtils.GetEmptyBoard(), Player.Max);

            PrintBenchmarkData(engine, startState, 10);
        }

        private void PrintBenchmarkData(SearchEngine searchEngine, IState startState, int searchDepth)
        {
            var startTime = DateTime.Now;
            var result = searchEngine.Evaluate(startState, Player.Max, searchDepth);
            var endTime = DateTime.Now;

            Console.WriteLine("Time: " + (endTime - startTime));
            Console.WriteLine("Leaves: " + result.Leaves);
            Console.WriteLine("IntarnalNodes: " + result.IntarnalNodes);
        }
    }
}
