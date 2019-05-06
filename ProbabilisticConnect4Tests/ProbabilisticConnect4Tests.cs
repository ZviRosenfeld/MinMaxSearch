using System;
using Connect4Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch;
using MinMaxSearch.Benckmarking;

namespace ProbabilisticConnect4Tests
{
    [TestClass]
    public class ProbabilisticConnect4Tests
    {
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(8)]
        [TestMethod]
        public void MaxHasTwoWinsNextTurn_MinBlocksTheMoreLikelyOne(int degreeOfParallelism)
        {
            var startState = new StartState(new Connect4State(new[,]
            {
                {Player.Empty, Player.Max, Player.Empty, Player.Empty, Player.Max, Player.Empty},
                {Player.Empty, Player.Max, Player.Empty, Player.Empty, Player.Max, Player.Empty},
                {Player.Empty, Player.Max, Player.Empty, Player.Empty, Player.Max, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
            }, Player.Min));

            var engine = Connect4TestUtils.GetSearchEngine(degreeOfParallelism);
            var newState = (ProbabilisticConnect4State) engine.Search(startState, 2).NextMove;
            Assert.AreEqual(Player.Min, newState.Board[3, 4], "Min didn't block the more likely win");
        }

        [DataRow(1)]
        [DataRow(2)]
        [DataRow(8)]
        [TestMethod]
        public void MaxTwoTurnsAwayFromTwoWins_MaxGoesForTheMoreLikilyWin(int degreeOfParallelism)
        {
            var startState = new StartState(new Connect4State(new[,]
            {
                {Player.Empty, Player.Empty, Player.Max, Player.Max, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
            }, Player.Max));

            var engine = Connect4TestUtils.GetSearchEngine(degreeOfParallelism);
            var newState = (ProbabilisticConnect4State)engine.Search(startState, 3).NextMove;
            Assert.AreEqual(Player.Max, newState.Board[0, 4], "Min didn't block the more likely win");
        }

        [TestMethod]
        [TestCategory("Benchmarking")]
        public void BenchmarkProbabilisticConnect4()
        {
            BenchmarkWithDegreeOfParallelism(1);
            BenchmarkWithDegreeOfParallelism(2);
            BenchmarkWithDegreeOfParallelism(8);
        }

        private void BenchmarkWithDegreeOfParallelism(int degreeOfParallelism)
        {
            Console.WriteLine("Running with degreeOfParallelism: " + degreeOfParallelism);
            var engine = Connect4TestUtils.GetSearchEngine(degreeOfParallelism);
            engine.RememberDeadEndStates = false;
            var startState = new StartState(new Connect4State(Connect4TestUtils.GetEmptyBoard(), Player.Max));

            var results = engine.Benchmark(startState, 7);
            Console.WriteLine(results.ToString());
        }
    }
}
