using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch;

namespace CheckersTests
{
    /// <summary>
    /// Since NextMovesGenerator is a rather complex class, I'm writing test to check that it works alright.
    /// </summary>
    [TestClass]
    public class NextMovesGeneratorTests
    {
        [TestMethod]
        public void MinMoveTest()
        {
            var board = TestUtils.GetEmptyBoard(3);
            board[2, 2] = CheckerPiece.MinChecker;
            var nextMove = new NextMovesGenerator(board, Player.Min).GenerateNextMoves();

            var expectedBoard = TestUtils.GetEmptyBoard(3);
            expectedBoard[1, 1] = CheckerPiece.MinChecker;
            Assert.IsTrue(TestUtils.AreEquale(expectedBoard, nextMove[0].GetBoard()), "Got board" + Environment.NewLine + nextMove[0]);
        }
        
        [TestMethod]
        public void MaxMoveTest()
        {
            var board = TestUtils.GetEmptyBoard(3);
            board[0, 0] = CheckerPiece.MaxChecker;
            var nextMove = new NextMovesGenerator(board, Player.Max).GenerateNextMoves();

            var expectedBoard = TestUtils.GetEmptyBoard(3);
            expectedBoard[1, 1] = CheckerPiece.MaxChecker;
            Assert.IsTrue(TestUtils.AreEquale(expectedBoard, nextMove[0].GetBoard()), "Got board" + Environment.NewLine + nextMove[0]);
        }

        [TestMethod]
        public void MinCheckerDontMoveBack()
        {
            var board = TestUtils.GetEmptyBoard(2);
            board[0, 0] = CheckerPiece.MinChecker;
            var nextMove = new NextMovesGenerator(board, Player.Min).GenerateNextMoves();

            Assert.AreEqual(0, nextMove.Count);
        }
        [TestMethod]
        public void MaxCheckerDontMoveBack()
        {
            var board = TestUtils.GetEmptyBoard(2);
            board[1, 1] = CheckerPiece.MaxChecker;
            var nextMove = new NextMovesGenerator(board, Player.Max).GenerateNextMoves();

            Assert.AreEqual(0, nextMove.Count);
        }


        [TestMethod]
        public void MinMoveGetsKingTest()
        {
            var board = TestUtils.GetEmptyBoard(2);
            board[1, 1] = CheckerPiece.MinChecker;
            var nextMove = new NextMovesGenerator(board, Player.Min).GenerateNextMoves();

            var expectedBoard = TestUtils.GetEmptyBoard(2);
            expectedBoard[0, 0] = CheckerPiece.MinKing;
            Assert.IsTrue(TestUtils.AreEquale(expectedBoard, nextMove[0].GetBoard()), "Got board" + Environment.NewLine + nextMove[0]);
        }

        [TestMethod]
        public void MaxMoveGetsKingTest()
        {
            var board = TestUtils.GetEmptyBoard(2);
            board[0, 0] = CheckerPiece.MaxChecker;
            var nextMove = new NextMovesGenerator(board, Player.Max).GenerateNextMoves();

            var expectedBoard = TestUtils.GetEmptyBoard(2);
            expectedBoard[1, 1] = CheckerPiece.MaxKing;
            Assert.IsTrue(TestUtils.AreEquale(expectedBoard, nextMove[0].GetBoard()), "Got board" + Environment.NewLine + nextMove[0]);
        }

        [TestMethod]
        public void MinJumpTest()
        {
            var board = TestUtils.GetEmptyBoard(4);
            board[3, 3] = CheckerPiece.MinChecker;
            board[2, 2] = CheckerPiece.MaxKing;
            var nextMove = new NextMovesGenerator(board, Player.Min).GenerateNextMoves();

            var expectedBoard = TestUtils.GetEmptyBoard(4);
            expectedBoard[1, 1] = CheckerPiece.MinChecker;
            Assert.IsTrue(TestUtils.AreEquale(expectedBoard, nextMove[0].GetBoard()), "Got board" + Environment.NewLine + nextMove[0]);
        }

        [TestMethod]
        public void MaxJumpTest()
        {
            var board = TestUtils.GetEmptyBoard(4);
            board[0, 0] = CheckerPiece.MaxChecker;
            board[1, 1] = CheckerPiece.MinChecker;
            var nextMove = new NextMovesGenerator(board, Player.Max).GenerateNextMoves();

            var expectedBoard = TestUtils.GetEmptyBoard(4);
            expectedBoard[2, 2] = CheckerPiece.MaxChecker;
            Assert.IsTrue(TestUtils.AreEquale(expectedBoard, nextMove[0].GetBoard()), "Got board" + Environment.NewLine + nextMove[0]);
        }

        [TestMethod]
        public void MinCantJumpBackTest()
        {
            var board = TestUtils.GetEmptyBoard(3);
            board[0, 0] = CheckerPiece.MinChecker;
            board[1, 1] = CheckerPiece.MaxKing;
            var nextMove = new NextMovesGenerator(board, Player.Min).GenerateNextMoves();

            Assert.AreEqual(0, nextMove.Count);
        }

        [TestMethod]
        public void MaxCantJumpBackTest()
        {
            var board = TestUtils.GetEmptyBoard(3);
            board[2, 2] = CheckerPiece.MaxChecker;
            board[1, 1] = CheckerPiece.MinChecker;
            var nextMove = new NextMovesGenerator(board, Player.Max).GenerateNextMoves();

            Assert.AreEqual(0, nextMove.Count);
        }

        [TestMethod]
        public void MinJumpGetsKingTest()
        {
            var board = TestUtils.GetEmptyBoard(3);
            board[2, 2] = CheckerPiece.MinChecker;
            board[1, 1] = CheckerPiece.MaxChecker;
            var nextMove = new NextMovesGenerator(board, Player.Min).GenerateNextMoves();

            var expectedBoard = TestUtils.GetEmptyBoard(3);
            expectedBoard[0, 0] = CheckerPiece.MinKing;
            Assert.IsTrue(TestUtils.AreEquale(expectedBoard, nextMove[0].GetBoard()), "Got board" + Environment.NewLine + nextMove[0]);
        }

        [TestMethod]
        public void MaxJumpGetsKingTest()
        {
            var board = TestUtils.GetEmptyBoard(3);
            board[0, 0] = CheckerPiece.MaxChecker;
            board[1, 1] = CheckerPiece.MinKing;
            var nextMove = new NextMovesGenerator(board, Player.Max).GenerateNextMoves();

            var expectedBoard = TestUtils.GetEmptyBoard(3);
            expectedBoard[2, 2] = CheckerPiece.MaxKing;
            Assert.IsTrue(TestUtils.AreEquale(expectedBoard, nextMove[0].GetBoard()), "Got board" + Environment.NewLine + nextMove[0]);
        }

        [TestMethod]
        public void MinDoubleJumpTest()
        {
            var board = TestUtils.GetEmptyBoard(5);
            board[4, 4] = CheckerPiece.MinChecker;
            board[3, 3] = CheckerPiece.MaxChecker;
            board[1, 3] = CheckerPiece.MaxKing;
            var nextMove = new NextMovesGenerator(board, Player.Min).GenerateNextMoves();

            var expectedBoard1 = TestUtils.GetEmptyBoard(5);
            expectedBoard1[2, 2] = CheckerPiece.MinChecker;
            expectedBoard1[1, 3] = CheckerPiece.MaxKing;
            Assert.IsTrue(TestUtils.AreEquale(expectedBoard1, nextMove[0].GetBoard()), "Got board" + Environment.NewLine + nextMove[0]);

            var expectedBoard2 = TestUtils.GetEmptyBoard(5);
            expectedBoard2[0, 4] = CheckerPiece.MinKing;
            Assert.IsTrue(TestUtils.AreEquale(expectedBoard2, nextMove[1].GetBoard()), "Got board" + Environment.NewLine + nextMove[1]);
        }

        [TestMethod]
        public void MaxDoubleJumpTest()
        {
            var board = TestUtils.GetEmptyBoard(5);
            board[0, 0] = CheckerPiece.MaxChecker;
            board[1, 1] = CheckerPiece.MinChecker;
            board[3, 1] = CheckerPiece.MinKing;
            var nextMove = new NextMovesGenerator(board, Player.Max).GenerateNextMoves();

            var expectedBoard1 = TestUtils.GetEmptyBoard(5);
            expectedBoard1[2, 2] = CheckerPiece.MaxChecker;
            expectedBoard1[3, 1] = CheckerPiece.MinKing;
            Assert.IsTrue(TestUtils.AreEquale(expectedBoard1, nextMove[0].GetBoard()), "Got board" + Environment.NewLine + nextMove[0]);

            var expectedBoard2 = TestUtils.GetEmptyBoard(5);
            expectedBoard2[4, 0] = CheckerPiece.MaxKing;
            Assert.IsTrue(TestUtils.AreEquale(expectedBoard2, nextMove[1].GetBoard()), "Got board" + Environment.NewLine + nextMove[1]);
        }

        [TestMethod]
        [DataRow(Player.Max)]
        [DataRow(Player.Min)]
        public void KingMovesTest(Player player)
        {
            var board = TestUtils.GetEmptyBoard(3);
            board[1, 1] = player.ToKing();
            var nextMove = new NextMovesGenerator(board, player).GenerateNextMoves();

            Assert.AreEqual(4, nextMove.Count);

            var expectedBoard = TestUtils.GetEmptyBoard(3);
            expectedBoard[0, 0] = player.ToKing();
            Assert.IsTrue(nextMove.Contains(expectedBoard), "Didn't get board" + Environment.NewLine + expectedBoard);

            expectedBoard = TestUtils.GetEmptyBoard(3);
            expectedBoard[2, 2] = player.ToKing();
            Assert.IsTrue(nextMove.Contains(expectedBoard), "Didn't get board" + Environment.NewLine + expectedBoard);

            expectedBoard = TestUtils.GetEmptyBoard(3);
            expectedBoard[0, 2] = player.ToKing();
            Assert.IsTrue(nextMove.Contains(expectedBoard), "Didn't get board" + Environment.NewLine + expectedBoard);

            expectedBoard = TestUtils.GetEmptyBoard(3);
            expectedBoard[2, 0] = player.ToKing();
            Assert.IsTrue(nextMove.Contains(expectedBoard), "Didn't get board" + Environment.NewLine + expectedBoard);
        }

        [TestMethod]
        [DataRow(Player.Max)]
        [DataRow(Player.Min)]
        public void KingJumpsTest(Player player)
        {
            var board = TestUtils.GetEmptyBoard(5);
            board[2, 2] = player.ToKing();
            PlacePiecesToJump(player, board);
            var nextMove = new NextMovesGenerator(board, player).GenerateNextMoves();

            Assert.AreEqual(4, nextMove.Count);

            var expectedBoard = TestUtils.GetEmptyBoard(5);
            PlacePiecesToJump(player, expectedBoard);
            expectedBoard[0, 0] = player.ToKing();
            expectedBoard[1, 1] = CheckerPiece.Empty;
            Assert.IsTrue(nextMove.Contains(expectedBoard), "Didn't get board" + Environment.NewLine + expectedBoard.ToState(player));

            expectedBoard = TestUtils.GetEmptyBoard(5);
            PlacePiecesToJump(player, expectedBoard);
            expectedBoard[4, 4] = player.ToKing();
            expectedBoard[3, 3] = CheckerPiece.Empty;
            Assert.IsTrue(nextMove.Contains(expectedBoard), "Didn't get board" + Environment.NewLine + expectedBoard.ToState(player));

            expectedBoard = TestUtils.GetEmptyBoard(5);
            PlacePiecesToJump(player, expectedBoard);
            expectedBoard[0, 4] = player.ToKing();
            expectedBoard[1, 3] = CheckerPiece.Empty;
            Assert.IsTrue(nextMove.Contains(expectedBoard), "Didn't get board" + Environment.NewLine + expectedBoard.ToState(player));

            expectedBoard = TestUtils.GetEmptyBoard(5);
            PlacePiecesToJump(player, expectedBoard);
            expectedBoard[4, 0] = player.ToKing();
            expectedBoard[3, 1] = CheckerPiece.Empty;
            Assert.IsTrue(nextMove.Contains(expectedBoard), "Didn't get board" + Environment.NewLine + expectedBoard.ToState(player));
        }

        private static void PlacePiecesToJump(Player player, CheckerPiece[,] board)
        {
            board[1, 1] = player.GetReversePlayer().ToKing();
            board[1, 3] = player.GetReversePlayer().ToKing();
            board[3, 1] = player.GetReversePlayer().ToKing();
            board[3, 3] = player.GetReversePlayer().ToKing();
        }
    }
}
