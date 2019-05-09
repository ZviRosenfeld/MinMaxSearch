using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch;

namespace TicTacToeTests
{
    [TestClass]
    public class TicTacToeBassicTests
    {
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(8)]
        [TestMethod]
        public void OneStepAwayFromMaxWinning_MaxTurn_MaxWin(int degreeOfParallelism)
        {
            TicTacToeState startState = new TicTacToeState(new[,]
            {
                { Player.Max, Player.Empty, Player.Max},
                { Player.Empty, Player.Empty, Player.Empty},
                { Player.Empty, Player.Empty, Player.Empty},
            }, Player.Max);

            var engine = GetSearchEngine(degreeOfParallelism);
            var evaluation = engine.Search(startState, 2);

            Assert.AreEqual(TicTacToeState.MaxValue, evaluation.NextMove.Evaluate(0, new List<IState>()), "Should have found a wining state");
            Assert.AreEqual(1, evaluation.StateSequence.Count, "StateSequence should only have one state in it");
        }

        [DataRow(1)]
        [DataRow(2)]
        [DataRow(8)]
        [TestMethod]
        public void TwoStepsAwayFromMaxWinning__MinsTurn_DontLetMinMax(int degreeOfParallelism)
        {
            TicTacToeState startState = new TicTacToeState(new[,]
            {
                { Player.Empty, Player.Empty, Player.Empty},
                { Player.Empty, Player.Empty, Player.Empty},
                { Player.Max, Player.Empty, Player.Max},
            }, Player.Min);

            var engine = GetSearchEngine(degreeOfParallelism);
            var newState = (TicTacToeState) engine.Search(startState, 2).NextMove;

            Assert.AreEqual(Player.Min, newState.Board[2,1], "Min didn't block Max's win. Board = " + newState);
        }

        [DataRow(1)]
        [DataRow(2)]
        [DataRow(8)]
        [TestMethod]
        public void ThreeStepsAwayFromMaxWinning_MaxTurn_MaxWin(int degreeOfParallelism)
        {
            TicTacToeState startState = new TicTacToeState(new[,]
            {
                { Player.Max, Player.Min, Player.Max},
                { Player.Empty, Player.Empty, Player.Empty},
                { Player.Min, Player.Empty, Player.Empty},
            }, Player.Max);

            var engine = GetSearchEngine(degreeOfParallelism);
            var evaluation = engine.Search(startState, 3);

            Assert.AreEqual(Player.Max, ((TicTacToeState)evaluation.NextMove).Board[2, 2]);
            var lastMove = (IDeterministicState)evaluation.StateSequence.Last();
            Assert.AreEqual(TicTacToeState.MaxValue, lastMove.Evaluate(0, new List<IState>()), "Should have found a wining state");
        }

        [DataRow(1)]
        [DataRow(2)]
        [DataRow(8)]
        [TestMethod]
        public void FiveStepsAwayFromMaxWinning_MaxTurn_MaxWin(int degreeOfParallelism)
        {
            TicTacToeState startState = new TicTacToeState(new[,]
            {
                { Player.Max, Player.Empty, Player.Empty},
                { Player.Min, Player.Empty, Player.Empty},
                { Player.Empty, Player.Empty, Player.Empty},
            }, Player.Max);

            var engine = GetSearchEngine(degreeOfParallelism);
            var evaluation = engine.Search(startState, 5);

            Assert.AreEqual(TicTacToeState.MaxValue, evaluation.Evaluation);
            var lastMove = (IDeterministicState) evaluation.StateSequence.Last();
            Assert.AreEqual(TicTacToeState.MaxValue, lastMove.Evaluate(0, new List<IState>()), "Should have found a wining state");
        }

        [DataRow(1)]
        [DataRow(2)]
        [DataRow(8)]
        [TestMethod]
        public void FiveStepsAwayFromMinWinning_MinTurn_MinWin(int degreeOfParallelism)
        {
            TicTacToeState startState = new TicTacToeState(new[,]
            {
                { Player.Empty, Player.Empty, Player.Empty},
                { Player.Empty, Player.Min, Player.Max},
                { Player.Empty, Player.Empty, Player.Empty},
            }, Player.Min);

            var engine = GetSearchEngine(degreeOfParallelism);
            var evaluation = engine.Search(startState, 5);

            Assert.AreEqual(TicTacToeState.MinValue, evaluation.Evaluation);
            var lastMove = (IDeterministicState)evaluation.StateSequence.Last();
            Assert.AreEqual(TicTacToeState.MinValue, lastMove.Evaluate(0, new List<IState>()), "Should have found a wining state");
        }

        [DataRow(1)]
        [DataRow(2)]
        [DataRow(8)]
        [TestMethod]
        public void NewGame_NoOneCanWin(int degreeOfParallelism)
        {
            TicTacToeState startState = new TicTacToeState(new[,]
            {
                { Player.Empty, Player.Empty, Player.Empty},
                { Player.Empty, Player.Empty, Player.Empty},
                { Player.Empty, Player.Empty, Player.Empty},
            }, Player.Max);

            var engine = GetSearchEngine(degreeOfParallelism);
            var evaluation = engine.Search(startState, 10);

            Assert.AreEqual(0, evaluation.Evaluation);
            var lastMove = (IDeterministicState)evaluation.StateSequence.Last();
            Assert.AreEqual(0, lastMove.Evaluate(0, new List<IState>()), "Should have found a wining state");
            Assert.IsTrue(evaluation.AllChildrenAreDeadEnds);

            if (degreeOfParallelism == 1)
            {
                //Check that the our optimizations are working
                Assert.IsTrue(evaluation.Leaves < 63000, "Too many leaves in search.");
                Assert.IsTrue(evaluation.InternalNodes < 84000, "Too many intarnal nodes in search.");
            }

            // Too few leaves or internal nodes means that something went wrong
            Assert.IsTrue(evaluation.Leaves > 500, "Too few leaves in search.");
            Assert.IsTrue(evaluation.InternalNodes > 500, "Too few intarnal nodes in search.");
        }

        public static SearchEngine GetSearchEngine(int degreeOfParallelism) => 
            new SearchEngine()
            {
                MaxDegreeOfParallelism = degreeOfParallelism,
                DieEarly = true,
                MinScore = -0.5,
                MaxScore = 0.5
            };
    }
}
