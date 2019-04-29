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
            var engine = TicTacToeBassicTests.GetSearchEngine(10);
            var startState = new TicTacToeState(new[,]
            {
                { Player.Empty, Player.Empty, Player.Empty},
                { Player.Empty, Player.Empty, Player.Empty},
                { Player.Empty, Player.Empty, Player.Empty},
            }, Player.Max);

            PrintBenchmarkData(engine, startState);
        }

        [TestMethod]
        [TestCategory("Benchmarking")]
        public void BenchmarkConnect4()
        {
            var engine = Connect4Tests.Connect4Tests.GetSearchEngine(10);
            var startState = new Connect4State(new[,]
            {
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
            }, Player.Max);

            PrintBenchmarkData(engine, startState);
        }

        private void PrintBenchmarkData(SearchEngine searchEngine, IState startState)
        {
            var startTime = DateTime.Now;
            var result = searchEngine.Evaluate(startState, Player.Max);
            var endTime = DateTime.Now;

            Console.WriteLine("Time: " + (endTime - startTime));
            Console.WriteLine("Leaves: " + result.Leaves);
            Console.WriteLine("IntarnalNodes: " + result.IntarnalNodes);
        }
    }
}
