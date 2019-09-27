using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch;

namespace TicTacToeTests
{
    [TestClass]
    [TestCategory("ThreadSafety")]
    public class ThreadSafetyTests
    {
        private const int TEST_RUNS = 50;

        [DataRow(1, ParallelismMode.TotalParallelism)]
        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(1, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void TestSearchIsThreadSafe(int degreeOfParallelism, ParallelismMode parallelismMode) =>
            RunThreadSaftyTest(degreeOfParallelism, parallelismMode, RunMaxWinningSearch);

        [DataRow(1, ParallelismMode.TotalParallelism)]
        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(1, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void TestIterativeSearchIsThreadSafe(int degreeOfParallelism, ParallelismMode parallelismMode) =>
            RunThreadSaftyTest(degreeOfParallelism, parallelismMode, RunMaxWinningIterativeSearch);

        private void RunThreadSaftyTest(int degreeOfParallelism, ParallelismMode parallelismMode, Action<SearchEngine> searchMethod)
        {
            var engine = TicTacToeBassicTests.GetSearchEngine(degreeOfParallelism, parallelismMode);

            var tasks = new Task[TEST_RUNS];
            for (int i = 0; i < TEST_RUNS; i++)
                tasks[i] = Task.Run(() => searchMethod(engine));

            var allTaskFinished = true;
            foreach (var task in tasks)
                allTaskFinished = allTaskFinished && task.Wait(TimeSpan.FromSeconds(30));

            Assert.IsTrue(allTaskFinished, "Not all tasks finished");
        }

        private void RunMaxWinningSearch(SearchEngine engine)
        {
            var startState = new TicTacToeState(new[,]
            {
                { Player.Max, Player.Empty, Player.Empty},
                { Player.Min, Player.Empty, Player.Empty},
                { Player.Empty, Player.Empty, Player.Empty},
            }, Player.Max);

            var result = engine.Search(startState, 10);
            Assert.AreEqual(TicTacToeState.MaxValue, result.Evaluation);
        }

        private void RunMaxWinningIterativeSearch(SearchEngine engine)
        {
            var startState = new TicTacToeState(new[,]
            {
                { Player.Max, Player.Empty, Player.Empty},
                { Player.Min, Player.Empty, Player.Empty},
                { Player.Empty, Player.Empty, Player.Empty},
            }, Player.Max);

            var iterativeEngine = new IterativeSearchWrapper(engine);
            var result = iterativeEngine.IterativeSearch(startState, 1, 10, CancellationToken.None);
            Assert.AreEqual(TicTacToeState.MaxValue, result.Evaluation);
        }
    }
}
