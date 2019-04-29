using System.Linq;
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

            var engine = GetSearchEngine(2);
            var evaluation = engine.Evaluate(startState, Player.Max);

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

            var engine = GetSearchEngine(2);
            var newState = (Connect4State)engine.Evaluate(startState, Player.Min).NextMove;

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

            var engine = GetSearchEngine(3);
            var evaluation = engine.Evaluate(startState, Player.Max);
            
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

            var engine = GetSearchEngine(5);
            var evaluation = engine.Evaluate(startState, Player.Max);

            Assert.AreEqual(BoardEvaluator.MaxEvaluation, evaluation.Evaluation);
            Assert.IsTrue(BoardEvaluator.IsWin(((Connect4State)evaluation.StateSequence.Last()).Board, Player.Max), "Should have found a wining state");
        }
        
        [TestMethod]
        public void NewGame_NoOneCanWin()
        {
            var startState = new Connect4State(new[,]
            {
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
                {Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty, Player.Empty},
            }, Player.Max);

            var engine = GetSearchEngine(7);
            var evaluation = engine.Evaluate(startState, Player.Max);

            Assert.IsFalse(BoardEvaluator.IsWin(((Connect4State)evaluation.StateSequence.Last()).Board, Player.Max));
            
            //Check that the our optimizations are working
            Assert.IsTrue(evaluation.Leaves < 19000, "Too many leaves in search; Pruning dosn't seem to be working");
            Assert.IsTrue(evaluation.IntarnalNodes < 7000, "Too many intarnal nodes in search; Pruning dosn't seem to be working");
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

            var engine = new SearchEngine(5) { FavorShortPaths = true};
            var evaluation = engine.Evaluate(startState, Player.Max);

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

            var engine = new SearchEngine(5) { FavorShortPaths = true};
            var evaluation = engine.Evaluate(startState, Player.Min);

            Assert.IsTrue(evaluation.StateSequence.Count > 2, "Min should have blocked the near win");
            Assert.AreEqual(Player.Min, ((Connect4State)evaluation.NextMove).Board[3, 2], "Min didn't block Max's win");
        }

        public static SearchEngine GetSearchEngine(int depth) =>
            new SearchEngine(depth)
            {
                RememberDeadEndStates = true,
                DieEarly = true,
                MinScore = BoardEvaluator.MinEvaluation + 1,
                MaxScore = BoardEvaluator.MaxEvaluation -1
            };
    }
}
