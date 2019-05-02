using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch;

namespace Connect4Tests
{
    [TestClass]
    public class Connect4Tests
    {
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(8)]
        [TestMethod]
        public void OneStepAwayFromMaxWinning_MaxTurn_MaxWin(int degreeOfParallelism)
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

            var engine = Connect4TestUtils.GetSearchEngine(degreeOfParallelism);
            var evaluation = engine.Search(startState, Player.Max, 2);

            Assert.IsTrue(BoardEvaluator.IsWin(((Connect4State) evaluation.NextMove).Board, Player.Max),
                "Should have found a wining state");
            Assert.AreEqual(1, evaluation.StateSequence.Count, "StateSequence should only have one state in it");
        }

        [DataRow(1)]
        [DataRow(2)]
        [DataRow(8)]
        [TestMethod]
        public void TwoStepsAwayFromMaxWinning__MinsTurn_DontLetMinMax(int degreeOfParallelism)
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

            var engine = Connect4TestUtils.GetSearchEngine(degreeOfParallelism);
            var newState = (Connect4State)engine.Search(startState, Player.Min, 2).NextMove;

            Assert.AreEqual(Player.Min, newState.Board[3, 1], "Min didn't block Max's win");
        }

        [DataRow(1)]
        [DataRow(2)]
        [DataRow(8)]
        [TestMethod]
        public void ThreeStepsAwayFromMaxWinning_MaxTurn_MaxWin(int degreeOfParallelism)
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

            var engine = Connect4TestUtils.GetSearchEngine(degreeOfParallelism);
            var evaluation = engine.Search(startState, Player.Max, 3);
            
            Assert.IsTrue(BoardEvaluator.IsWin(((Connect4State) evaluation.StateSequence.Last()).Board, Player.Max), "Should have found a wining state");
        }

        [DataRow(1)]
        [DataRow(2)]
        [DataRow(8)]
        [TestMethod]
        public void FiveStepsAwayFromMaxWinning_MaxTurn_MaxWin(int degreeOfParallelism)
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

            var engine = Connect4TestUtils.GetSearchEngine(degreeOfParallelism);
            var evaluation = engine.Search(startState, Player.Max, 5);

            Assert.AreEqual(BoardEvaluator.MaxEvaluation, evaluation.Evaluation);
            Assert.IsTrue(BoardEvaluator.IsWin(((Connect4State)evaluation.StateSequence.Last()).Board, Player.Max), "Should have found a wining state");
        }

        [DataRow(1)]
        [DataRow(2)]
        [DataRow(8)]
        [TestMethod]
        public void NewGame_NoOneCanWin(int degreeOfParallelism)
        {
            var startState = new Connect4State(Connect4TestUtils.GetEmptyBoard(), Player.Max);

            var engine = Connect4TestUtils.GetSearchEngine(degreeOfParallelism);
            var evaluation = engine.Search(startState, Player.Max, 7);

            Assert.IsFalse(BoardEvaluator.IsWin(((Connect4State)evaluation.StateSequence.Last()).Board, Player.Max));

            if (degreeOfParallelism == 1)
            {
                //Check that the our optimizations are working
                Assert.IsTrue(evaluation.Leaves < 19000, "Too many leaves in search. Leaves = " + evaluation.Leaves);
                Assert.IsTrue(evaluation.InternalNodes < 7000,
                    "Too many intarnal nodes in search. Nodes = " + evaluation.InternalNodes);
            }
            // Too few leaves or internal nodes means that something went wrong
            Assert.IsTrue(evaluation.Leaves > 1000, "Too few leaves in search. Leaves = " + evaluation.Leaves);
            Assert.IsTrue(evaluation.InternalNodes > 1000, "Too few intarnal nodes in search. Nodes = " + evaluation.InternalNodes);
        }

        [DataRow(1)]
        [DataRow(2)]
        [DataRow(8)]
        [TestMethod]
        public void MaxCanWinNextMoveOrInThree_MaxWinsNextMove(int degreeOfParallelism)
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

            var engine = new SearchEngine() { FavorShortPaths = true, MaxDegreeOfParallelism = degreeOfParallelism};
            var evaluation = engine.Search(startState, Player.Max, 5);

            Assert.IsTrue(evaluation.StateSequence.Count == 1, "Max should have won in one move");
            Assert.AreEqual(Player.Max, ((Connect4State)evaluation.NextMove).Board[3, 2], "Max didn't win");

        }

        [DataRow(1)]
        [DataRow(2)]
        [DataRow(8)]
        [TestMethod]
        public void MaxCanWinInFourMovesOrTwo_MinBlocksNearWin(int degreeOfParallelism)
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

            var engine = new SearchEngine() { FavorShortPaths = true, MaxDegreeOfParallelism = degreeOfParallelism};
            var evaluation = engine.Search(startState, Player.Min, 5);

            Assert.IsTrue(evaluation.StateSequence.Count > 2, "Min should have blocked the near win");
            Assert.AreEqual(Player.Min, ((Connect4State)evaluation.NextMove).Board[3, 2], "Min didn't block Max's win");
        }

        [DataRow(1)]
        [DataRow(2)]
        [TestMethod]
        public void NewGame_CheckCancellationToken(int degreeOfParallelism)
        {
            var startState = new Connect4State(Connect4TestUtils.GetEmptyBoard(), Player.Max);

            var engine = Connect4TestUtils.GetSearchEngine(degreeOfParallelism);
            var cancellationSource = new CancellationTokenSource(1000);
            var searchTask = engine.SearchAsync(startState, Player.Max, 20, cancellationSource.Token);
            Thread.Sleep(2000);

            Assert.IsTrue(searchTask.IsCompleted, "Search should have complated by now");
            var t = searchTask.Result; // Check that we can get a result even if the search was terminated
        }     
    }
}
