using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch;
using MinMaxSearch.Banckmarking;

namespace Connect4Tests
{
    [TestClass]
    public class Connect4Benchmarking
    {
        [TestMethod]
        [TestCategory("Benchmarking")]
        public void BenchmarkConnect4()
        {
            var engine = Connect4TestUtils.GetSearchEngine();
            var startState = new Connect4State(Connect4TestUtils.GetEmptyBoard(), Player.Max);

            var results = engine.Benchmark(startState, 11, 2);
            results.Print();
        }
    }
}
