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
            board[4, 0] = CheckerPiece.MaxChecker;
            board[5, 1] = CheckerPiece.MinChecker;
            board[5, 3] = CheckerPiece.MinChecker;
            var startState = board.ToState(Player.Max);

            var engine = Utils.GetSearchEngine();

            var searchResult = engine.Search(startState, 10);
            Assert.AreEqual(CheckerPiece.MaxChecker, ((CheckersState) searchResult.NextMove).Board[4, 4], "Max should have moved to the double-jump");
        }
    }
}
