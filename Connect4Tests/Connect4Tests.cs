﻿using System;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch;

namespace Connect4Tests
{
    [TestClass]
    public class Connect4Tests
    {
        [DataRow(1, ParallelismMode.TotalParallelism)]
        [DataRow(2, ParallelismMode.TotalParallelism)]
        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(1, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void OneStepAwayFromMaxWinning_MaxTurn_MaxWin(int degreeOfParallelism, ParallelismMode parallelismMode)
        {
            var startState = new Connect4State(new[,]
            {
                {Player.Empty, Player.Max, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Max, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Max, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
            }, Player.Max);

            var engine = Connect4TestUtils.GetSearchEngine(degreeOfParallelism, parallelismMode);
            var evaluation = engine.Search(startState, 2);

            Assert.IsTrue(BoardEvaluator.IsWin(((Connect4State) evaluation.NextMove).Board, Player.Max),
                "Should have found a wining state");
            Assert.AreEqual(1, evaluation.StateSequence.Count, "StateSequence should only have one state in it");
        }

        [DataRow(1, ParallelismMode.TotalParallelism)]
        [DataRow(2, ParallelismMode.TotalParallelism)]
        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(1, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void TwoStepsAwayFromMaxWinning__MinsTurn_DontLetMinMax(int degreeOfParallelism, ParallelismMode parallelismMode)
        {
            var startState = new Connect4State(new[,]
            {
                {Player.Empty, Player.Max, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Max, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Max, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
            }, Player.Min);

            var engine = Connect4TestUtils.GetSearchEngine(degreeOfParallelism, parallelismMode);
            var newState = (Connect4State)engine.Search(startState, 2).NextMove;

            Assert.AreEqual(Player.Min, newState.Board[3, 1], "Min didn't block Max's win");
        }

        [DataRow(1, ParallelismMode.TotalParallelism)]
        [DataRow(2, ParallelismMode.TotalParallelism)]
        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(1, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void ThreeStepsAwayFromMaxWinning_MaxTurn_MaxWin(int degreeOfParallelism, ParallelismMode parallelismMode)
        {
            var startState = new Connect4State(new[,]
            {
                {Player.Empty, Player.Empty, Player.Empty, Player.Max, Player.Max, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
            }, Player.Max);

            var engine = Connect4TestUtils.GetSearchEngine(degreeOfParallelism, parallelismMode);
            var evaluation = engine.Search(startState, 3);
            
            Assert.IsTrue(BoardEvaluator.IsWin(((Connect4State) evaluation.StateSequence.Last()).Board, Player.Max), "Should have found a wining state");
        }
        
        [DataRow(1, ParallelismMode.TotalParallelism)]
        [DataRow(2, ParallelismMode.TotalParallelism)]
        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(1, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void FiveStepsAwayFromMaxWinning_MaxTurn_MaxWin(int degreeOfParallelism, ParallelismMode parallelismMode)
        {
            var startState = new Connect4State(new[,]
            {
                {Player.Empty, Player.Min, Player.Max, Player.Empty, Player.Max, Player.Empty},
                {Player.Empty, Player.Max, Player.Min, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Max, Player.Max, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
            }, Player.Max);

            var engine = Connect4TestUtils.GetSearchEngine(degreeOfParallelism, parallelismMode);
            var evaluation = engine.Search(startState, 5);

            Assert.AreEqual(BoardEvaluator.MaxEvaluation, evaluation.Evaluation);
            Assert.IsTrue(BoardEvaluator.IsWin(((Connect4State)evaluation.StateSequence.Last()).Board, Player.Max), "Should have found a wining state");
        }

        [DataRow(1, ParallelismMode.TotalParallelism)]
        [DataRow(2, ParallelismMode.TotalParallelism)]
        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(2, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void NewGame_NoOneCanWin(int degreeOfParallelism, ParallelismMode parallelismMode)
        {
            var startState = new Connect4State(Connect4TestUtils.GetEmptyBoard(), Player.Max);

            var engine = Connect4TestUtils.GetSearchEngine(degreeOfParallelism, parallelismMode);
            var evaluation = engine.Search(startState, 7);

            Assert.IsFalse(BoardEvaluator.IsWin(((Connect4State)evaluation.StateSequence.Last()).Board, Player.Max));
            Assert.IsFalse(evaluation.AllChildrenAreDeadEnds);

            if (degreeOfParallelism == 1)
            {
                //Check that the our optimizations are working
                Assert.IsTrue(evaluation.Leaves < 26000, "Too many leaves in search. Leaves = " + evaluation.Leaves);
                Assert.IsTrue(evaluation.InternalNodes < 10000,
                    "Too many intarnal nodes in search. Nodes = " + evaluation.InternalNodes);
            }
            // Too few leaves or internal nodes means that something went wrong
            Assert.IsTrue(evaluation.Leaves > 1000, "Too few leaves in search. Leaves = " + evaluation.Leaves);
            Assert.IsTrue(evaluation.InternalNodes > 1000, "Too few intarnal nodes in search. Nodes = " + evaluation.InternalNodes);
        }

        [DataRow(1, ParallelismMode.TotalParallelism)]
        [DataRow(2, ParallelismMode.TotalParallelism)]
        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(1, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void MaxCanWinNextMoveOrInThree_MaxWinsNextMove(int degreeOfParallelism, ParallelismMode parallelismMode)
        {
            var startState = new Connect4State(new[,]
            {
                {Player.Empty, Player.Empty, Player.Max, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Max, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Max, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
            }, Player.Max);

            var engine = new SearchEngine() { FavorShortPaths = true, MaxDegreeOfParallelism = degreeOfParallelism, ParallelismMode = parallelismMode};
            var evaluation = engine.Search(startState, 5);

            Assert.IsTrue(evaluation.StateSequence.Count == 1, "Max should have won in one move");
            Assert.AreEqual(Player.Max, ((Connect4State)evaluation.NextMove).Board[3, 2], "Max didn't win");

        }

        [DataRow(1, ParallelismMode.TotalParallelism)]
        [DataRow(2, ParallelismMode.TotalParallelism)]
        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(1, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void MaxCanWinInFourMovesOrTwo_MinBlocksNearWin(int degreeOfParallelism, ParallelismMode parallelismMode)
        {
            var startState = new Connect4State(new[,]
            {
                {Player.Empty, Player.Empty, Player.Max, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Max, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Max, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
            }, Player.Min);

            var engine = new SearchEngine() { FavorShortPaths = true, MaxDegreeOfParallelism = degreeOfParallelism, ParallelismMode = parallelismMode};
            var evaluation = engine.Search(startState, 5);

            Assert.IsTrue(evaluation.StateSequence.Count > 2, "Min should have blocked the near win");
            Assert.AreEqual(Player.Min, ((Connect4State)evaluation.NextMove).Board[3, 2], "Min didn't block Max's win");
        }

        [DataRow(1, ParallelismMode.TotalParallelism)]
        [DataRow(2, ParallelismMode.TotalParallelism)]
        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(1, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void NewGame_CheckCancellationToken(int degreeOfParallelism, ParallelismMode parallelismMode)
        {
            var startState = new Connect4State(Connect4TestUtils.GetEmptyBoard(), Player.Max);

            var engine = Connect4TestUtils.GetSearchEngine(degreeOfParallelism, parallelismMode);
            var cancellationSource = new CancellationTokenSource();
            var searchTask = engine.SearchAsync(startState, 20, cancellationSource.Token);
            Thread.Sleep(500);
            cancellationSource.Cancel();
            Thread.Sleep(500);

            Assert.IsTrue(searchTask.IsCompleted, "Search should have complated by now");
            var t = searchTask.Result; // Check that we can get a result even if the search was terminated
        }
    }
}
