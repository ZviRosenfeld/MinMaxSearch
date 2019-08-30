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
        [DataRow(1, ParallelismMode.TotalParallelism)]
        [DataRow(2, ParallelismMode.TotalParallelism)]
        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(1, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void MaxHasTwoWinsNextTurn_MinBlocksTheMoreLikelyOne(int degreeOfParallelism, ParallelismMode parallelismMode)
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

            var engine = Connect4TestUtils.GetSearchEngine(degreeOfParallelism, parallelismMode);
            var newState = (ProbabilisticConnect4State) engine.Search(startState, 2).NextMove;
            Assert.AreEqual(Player.Min, newState.Board[3, 4], "Min didn't block the more likely win");
        }

        [DataRow(1, ParallelismMode.TotalParallelism)]
        [DataRow(2, ParallelismMode.TotalParallelism)]
        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(1, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void MaxTwoTurnsAwayFromTwoWins_MaxGoesForTheMoreLikilyWin(int degreeOfParallelism, ParallelismMode parallelismMode)
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

            var engine = Connect4TestUtils.GetSearchEngine(degreeOfParallelism, parallelismMode);
            var newState = (ProbabilisticConnect4State)engine.Search(startState, 3).NextMove;
            Assert.AreEqual(Player.Max, newState.Board[0, 4], "Min didn't block the more likely win");
        }

        // Since this runs a probabilistic game, there is a certain chance that the test will fail even if everything is working fine
        [TestMethod]
        public void TestCompeteWorksWithProbabilisticStates()
        {
            var engine = Connect4TestUtils.GetSearchEngineBuilder(1, ParallelismMode.FirstLevelOnly);
            var startState = new StartState(new Connect4State(Connect4TestUtils.GetEmptyBoard(), Player.Max));

            var results = engine.Compete(startState, 3, (s, d, l) => 0);

            var finalState = (ProbabilisticConnect4State) results.FinalState;
            Assert.IsTrue(BoardEvaluator.IsWin(finalState.Board, Player.Min), "Min should have won; Final state is " + finalState);
        }

        [TestMethod]
        [TestCategory("Benchmarking")]
        public void BenchmarkProbabilisticConnect4()
        {
            BenchmarkWithDegreeOfParallelism(1, ParallelismMode.NonParallelism);
            BenchmarkWithDegreeOfParallelism(1, ParallelismMode.FirstLevelOnly);
            BenchmarkWithDegreeOfParallelism(4, ParallelismMode.TotalParallelism);
        }

        private void BenchmarkWithDegreeOfParallelism(int degreeOfParallelism, ParallelismMode parallelismMode)
        {
            Console.WriteLine("Running with degreeOfParallelism: " + degreeOfParallelism + ", Mode: " + parallelismMode);
            var engine = Connect4TestUtils.GetSearchEngine(degreeOfParallelism, parallelismMode);
            var startState = new StartState(new Connect4State(Connect4TestUtils.GetEmptyBoard(), Player.Max));

            var results = engine.Search(startState, 7);
            Console.WriteLine("Time: " + results.SearchTime);
            Console.WriteLine("Leaves: " + results.Leaves);
            Console.WriteLine("InternalNodes: " + results.InternalNodes);
        }
    }
}
