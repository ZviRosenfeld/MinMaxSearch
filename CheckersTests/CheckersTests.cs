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
            Assert.AreEqual(CheckerPiece.MaxChecker, ((CheckersState) searchResult.NextMove).Board[4, 4], "Max should have moved to the double-jump");
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
            Assert.AreEqual(CheckerPiece.MaxKing, ((CheckersState)searchResult.NextMove).Board[7, 6], "Max should have kinged a king " + Environment.NewLine + searchResult.NextMove);
        }
    }
}
