using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch;

namespace CheckersTests
{
    [TestClass]
    public class CheckersTests
    {
        [TestMethod]
        public void MaxCanJumpOneOrTwoPiecese_JumpOverTwoPieces()
        {
            var board = Utils.GetEmptyBoard();
            board[0, 0] = CheckerPiece.MaxChecker;
            board[1, 1] = CheckerPiece.MinChecker;
            board[0, 4] = CheckerPiece.MaxChecker;
            board[1, 5] = CheckerPiece.MinChecker;
            board[3, 5] = CheckerPiece.MinChecker;
            var startState = board.ToState(Player.Max);

            var engine = Utils.GetCheckersSearchEngine();

            var searchResult = engine.Search(startState, 10);
            Assert.AreEqual(CheckerPiece.MaxChecker, searchResult.NextMove.GetBoard()[4, 4], "Max should have moved to the double-jump" + Environment.NewLine + searchResult.NextMove);
        }

        [TestMethod]
        public void MinCanJumpOneOrTwoPiecese_JumpOverTwoPieces()
        {
            var board = Utils.GetEmptyBoard();
            board[7, 7] = CheckerPiece.MinChecker;
            board[6, 6] = CheckerPiece.MaxChecker;
            board[7, 4] = CheckerPiece.MinChecker;
            board[6, 5] = CheckerPiece.MaxChecker;
            board[4, 5] = CheckerPiece.MaxChecker;
            var startState = board.ToState(Player.Min);

            var engine = Utils.GetCheckersSearchEngine();

            var searchResult = engine.Search(startState, 10);
            Assert.AreEqual(CheckerPiece.MinChecker, searchResult.NextMove.GetBoard()[3, 4], "Min should have moved to the double-jump" + Environment.NewLine + searchResult.NextMove);
        }

        [TestMethod]
        public void MaxCanKindPiece_KingPiece()
        {
            var board = Utils.GetEmptyBoard();
            board[0, 0] = CheckerPiece.MaxChecker;
            board[6, 7] = CheckerPiece.MaxChecker;
            board[4, 4] = CheckerPiece.MinChecker;
            var startState = board.ToState(Player.Max);

            var engine = Utils.GetCheckersSearchEngine();

            var searchResult = engine.Search(startState, 2);
            Assert.AreEqual(CheckerPiece.MaxKing, searchResult.NextMove.GetBoard()[7, 6], "Max should have kinged a king " + Environment.NewLine + searchResult.NextMove);
        }

        [TestMethod]
        [DataRow(ParallelismMode.TotalParallelism, true)]
        [DataRow(ParallelismMode.FirstLevelOnly, false)]
        [DataRow(ParallelismMode.NonParallelism, true)]
        [DataRow(ParallelismMode.NonParallelism, false)]
        public void MaxCanWinInThreeTurns_MaxWins(ParallelismMode parallelismMode, bool dieEarly)
        {
            var board = Utils.GetEmptyBoard(5);
            board[0, 0] = CheckerPiece.MaxChecker;
            board[1, 3] = CheckerPiece.MaxChecker;
            board[3, 3] = CheckerPiece.MinChecker;
            var startState = board.ToState(Player.Max);

            var engine = Utils.GetCheckersSearchEngine(4, parallelismMode, 1, dieEarly);

            var searchResult = engine.Search(startState, 3);
            Assert.AreEqual(CheckerPiece.MaxChecker, searchResult.NextMove.GetBoard()[2, 2], "Max should have set a trap by moving to [2, 2] " + Environment.NewLine + searchResult.NextMove);
            Assert.AreEqual(CheckersState.MAX_WIN, searchResult.Evaluation, "Max should have won!");
        }

        [TestMethod]
        [DataRow(ParallelismMode.TotalParallelism, true)]
        [DataRow(ParallelismMode.FirstLevelOnly, false)]
        [DataRow(ParallelismMode.NonParallelism, true)]
        [DataRow(ParallelismMode.NonParallelism, false)]
        public void MinCanWinInFiveTurns_MinWins(ParallelismMode parallelismMode, bool dieEarly)
        {
            var board = Utils.GetEmptyBoard(7);
            board[2, 4] = CheckerPiece.MinKing;
            board[5, 3] = CheckerPiece.MaxKing;
            var startState = board.ToState(Player.Min);

            var engine = Utils.GetCheckersSearchEngine(4, parallelismMode, 1, dieEarly);

            var searchResult = engine.Search(startState, 5);
            Assert.AreEqual(CheckerPiece.MinKing, searchResult.NextMove.GetBoard()[3, 3], "Min should have set a trap by moving to [3, 3] " + Environment.NewLine + searchResult.NextMove);
            Assert.AreEqual(CheckersState.MIN_WIN, searchResult.Evaluation, "Min should have won!");
        }
    }
}
