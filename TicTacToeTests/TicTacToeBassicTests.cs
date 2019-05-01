using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch;

namespace TicTacToeTests
{
    [TestClass]
    public class TicTacToeBassicTests
    {
        [TestMethod]
        public void OneStepAwayFromMaxWinning_MaxTurn_MaxWin()
        {
            TicTacToeState startState = new TicTacToeState(new[,]
            {
                { Player.Max, Player.Empty, Player.Max},
                { Player.Empty, Player.Empty, Player.Empty},
                { Player.Empty, Player.Empty, Player.Empty},
            }, Player.Max);

            var engine = GetSearchEngine();
            var evaluation = engine.Search(startState, Player.Max, 2);

            Assert.AreEqual(TicTacToeState.MaxValue, evaluation.NextMove.Evaluate(0, new List<IState>()), "Should have found a wining state");
            Assert.AreEqual(1, evaluation.StateSequence.Count, "StateSequence should only have one state in it");
        }

        [TestMethod]
        public void TwoStepsAwayFromMaxWinning__MinsTurn_DontLetMinMax()
        {
            TicTacToeState startState = new TicTacToeState(new[,]
            {
                { Player.Empty, Player.Empty, Player.Empty},
                { Player.Empty, Player.Empty, Player.Empty},
                { Player.Max, Player.Empty, Player.Max},
            }, Player.Min);

            var engine = GetSearchEngine();
            var newState = (TicTacToeState) engine.Search(startState, Player.Min, 2).NextMove;

            Assert.AreEqual(Player.Min, newState.Board[2,1], "Min didn't block Max's win");
        }

        [TestMethod]
        public void ThreeStepsAwayFromMaxWinning_MaxTurn_MaxWin()
        {
            TicTacToeState startState = new TicTacToeState(new[,]
            {
                { Player.Max, Player.Min, Player.Max},
                { Player.Empty, Player.Empty, Player.Empty},
                { Player.Min, Player.Empty, Player.Empty},
            }, Player.Max);

            var engine = GetSearchEngine();
            var evaluation = engine.Search(startState, Player.Max, 3);

            Assert.AreEqual(Player.Max, ((TicTacToeState)evaluation.NextMove).Board[2, 2]);
            Assert.AreEqual(TicTacToeState.MaxValue, evaluation.StateSequence.Last().Evaluate(0, new List<IState>()), "Should have found a wining state");
        }

        [TestMethod]
        public void FiveStepsAwayFromMaxWinning_MaxTurn_MaxWin()
        {
            TicTacToeState startState = new TicTacToeState(new[,]
            {
                { Player.Max, Player.Empty, Player.Empty},
                { Player.Min, Player.Empty, Player.Empty},
                { Player.Empty, Player.Empty, Player.Empty},
            }, Player.Max);

            var engine = GetSearchEngine();
            var evaluation = engine.Search(startState, Player.Max, 5);

            Assert.AreEqual(TicTacToeState.MaxValue, evaluation.Evaluation);
            Assert.AreEqual(TicTacToeState.MaxValue, evaluation.StateSequence.Last().Evaluate(0, new List<IState>()), "Should have found a wining state");
        }

        [TestMethod]
        public void FiveStepsAwayFromMinWinning_MinTurn_MinWin()
        {
            TicTacToeState startState = new TicTacToeState(new[,]
            {
                { Player.Empty, Player.Empty, Player.Empty},
                { Player.Empty, Player.Min, Player.Max},
                { Player.Empty, Player.Empty, Player.Empty},
            }, Player.Min);

            var engine = GetSearchEngine();
            var evaluation = engine.Search(startState, Player.Min, 5);

            Assert.AreEqual(TicTacToeState.MinValue, evaluation.Evaluation);
            Assert.AreEqual(TicTacToeState.MinValue, evaluation.StateSequence.Last().Evaluate(0, new List<IState>()), "Should have found a wining state");
        }

        [TestMethod]
        public void NewGame_NoOneCanWin()
        {
            TicTacToeState startState = new TicTacToeState(new[,]
            {
                { Player.Empty, Player.Empty, Player.Empty},
                { Player.Empty, Player.Empty, Player.Empty},
                { Player.Empty, Player.Empty, Player.Empty},
            }, Player.Max);

            var engine = GetSearchEngine();
            var evaluation = engine.Search(startState, Player.Max, 10);

            Assert.AreEqual(0, evaluation.Evaluation);
            Assert.AreEqual(0, evaluation.StateSequence.Last().Evaluate(0, new List<IState>()), "Should have found a wining state");
            
            //Check that the our optimizations are working
            Assert.IsTrue(evaluation.Leaves < 7500, "Too many leaves in search.");
            Assert.IsTrue(evaluation.InternalNodes < 11000, "Too many intarnal nodes in search.");

            // Too few leaves or internal nodes means that something went wrong
            Assert.IsTrue(evaluation.Leaves > 500, "Too few leaves in search.");
            Assert.IsTrue(evaluation.InternalNodes > 500, "Too few intarnal nodes in search.");
        }

        public static SearchEngine GetSearchEngine() => 
            new SearchEngine()
            {
                DieEarly = true,
                MinScore = -0.5,
                MaxScore = 0.5
            };
    }
}
