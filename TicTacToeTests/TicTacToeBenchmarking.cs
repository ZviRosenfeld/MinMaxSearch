using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch;
using MinMaxSearch.Banckmarking;

namespace TicTacToeTests
{
    [TestClass]
    public class TicTacToeBenchmarking
    {
        [TestMethod]
        [TestCategory("Benchmarking")]
        public void BenchmarkTicTacToe()
        {
            var engine = TicTacToeBassicTests.GetSearchEngine();
            var startState = new TicTacToeState(new[,]
            {
                { Player.Empty, Player.Empty, Player.Empty},
                { Player.Empty, Player.Empty, Player.Empty},
                { Player.Empty, Player.Empty, Player.Empty},
            }, Player.Max);

            var results = engine.Benchmark(startState, 10, 2);
            results.Print();
        }
    }
}
