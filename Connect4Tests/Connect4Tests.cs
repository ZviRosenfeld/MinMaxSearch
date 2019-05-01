using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch;

namespace Connect4Tests
{
    [TestClass]
    public class Connect4Tests
    {
        [TestMethod]
        public void OneStepAwayFromMaxWinning_MaxTurn_MaxWin()
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

            var engine = Connect4TestUtils.GetSearchEngine();
            var evaluation = engine.Search(startState, Player.Max, 2);

            Assert.IsTrue(BoardEvaluator.IsWin(((Connect4State) evaluation.NextMove).Board, Player.Max),
                "Should have found a wining state");
            Assert.AreEqual(1, evaluation.StateSequence.Count, "StateSequence should only have one state in it");
        }
        
        [TestMethod]
        public void TwoStepsAwayFromMaxWinning__MinsTurn_DontLetMinMax()
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

            var engine = Connect4TestUtils.GetSearchEngine();
            var newState = (Connect4State)engine.Search(startState, Player.Min, 2).NextMove;

            Assert.AreEqual(Player.Min, newState.Board[3, 1], "Min didn't block Max's win");
        }
        
        [TestMethod]
        public void ThreeStepsAwayFromMaxWinning_MaxTurn_MaxWin()
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

            var engine = Connect4TestUtils.GetSearchEngine();
            var evaluation = engine.Search(startState, Player.Max, 3);
            
            Assert.IsTrue(BoardEvaluator.IsWin(((Connect4State) evaluation.StateSequence.Last()).Board, Player.Max), "Should have found a wining state");
        }
        
        [TestMethod]
        public void FiveStepsAwayFromMaxWinning_MaxTurn_MaxWin()
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

            var engine = Connect4TestUtils.GetSearchEngine();
            var evaluation = engine.Search(startState, Player.Max, 5);

            Assert.AreEqual(BoardEvaluator.MaxEvaluation, evaluation.Evaluation);
            Assert.IsTrue(BoardEvaluator.IsWin(((Connect4State)evaluation.StateSequence.Last()).Board, Player.Max), "Should have found a wining state");
        }
        
        [TestMethod]
        public void NewGame_NoOneCanWin()
        {
            var startState = new Connect4State(Connect4TestUtils.GetEmptyBoard(), Player.Max);

            var engine = Connect4TestUtils.GetSearchEngine();
            var evaluation = engine.Search(startState, Player.Max, 7);

            Assert.IsFalse(BoardEvaluator.IsWin(((Connect4State)evaluation.StateSequence.Last()).Board, Player.Max));
            
            //Check that the our optimizations are working
            Assert.IsTrue(evaluation.Leaves < 19000, "Too many leaves in search.");
            Assert.IsTrue(evaluation.InternalNodes < 7000, "Too many intarnal nodes in search.");

            // Too few leaves or internal nodes means that something went wrong
            Assert.IsTrue(evaluation.Leaves > 1000, "Too few leaves in search.");
            Assert.IsTrue(evaluation.InternalNodes > 1000, "Too few intarnal nodes in search.");
        }

        [TestMethod]
        public void MaxCanWinNextMoveOrInThree_MaxWinsNextMove()
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

            var engine = new SearchEngine() { FavorShortPaths = true};
            var evaluation = engine.Search(startState, Player.Max, 5);

            Assert.IsTrue(evaluation.StateSequence.Count == 1, "Max should have won in one move");
            Assert.AreEqual(Player.Max, ((Connect4State)evaluation.NextMove).Board[3, 2], "Max didn't win");

        }
        
        [TestMethod]
        public void MaxCanWinInFourMovesOrTwo_MinBlocksNearWin()
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

            var engine = new SearchEngine() { FavorShortPaths = true};
            var evaluation = engine.Search(startState, Player.Min, 5);

            Assert.IsTrue(evaluation.StateSequence.Count > 2, "Min should have blocked the near win");
            Assert.AreEqual(Player.Min, ((Connect4State)evaluation.NextMove).Board[3, 2], "Min didn't block Max's win");
        }

        [TestMethod]
        public void NewGame_CheckCancellationToken()
        {
            var startState = new Connect4State(Connect4TestUtils.GetEmptyBoard(), Player.Max);

            var engine = Connect4TestUtils.GetSearchEngine();
            var cancellationSource = new CancellationTokenSource(1000);
            var searchTask = engine.EvaluateAsync(startState, Player.Max, 20, cancellationSource.Token);    
            Thread.Sleep(1500);

            Assert.IsTrue(searchTask.IsCompleted, "Search should have complated by now");
        }     
    }
}
