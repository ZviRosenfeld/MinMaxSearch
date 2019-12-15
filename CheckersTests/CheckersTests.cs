using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch;

namespace CheckersTests
{
    [TestClass]
    public class CheckersTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var board = Utils.GetStartBoard();
            var state = new CheckersState(board, Player.Max);
            var next = state.GetNeighbors();
        }
    }
}
