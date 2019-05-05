using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MinMaxSearch.UnitTests
{
    [TestClass]
    public class IterativeSearchEngineTests
    {
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void IterativeSearch_StartDepthEqualThenMaxDepth_ThrowException()
        {
            var searchEngine = new SearchEngine();
            searchEngine.IterativeSearch(new IncreasingNumberState(1, Player.Max), 2, 2, CancellationToken.None);
        }

        [DataRow(1)]
        [DataRow(2)]
        [DataRow(8)]
        [TestMethod]
        public void IterativeSearch_SearchCanceldBeforeFirstSearchFinished_DontReturnNullResult(int degreeOfParallelism)
        {
            var cancellationSource = new CancellationTokenSource();
            cancellationSource.Cancel();
            var searchEngine = new SearchEngine {MaxDegreeOfParallelism = degreeOfParallelism};
            var result = searchEngine.IterativeSearch(new IncreasingNumberState(1, Player.Max), 1, 2, cancellationSource.Token);
            Assert.IsNotNull(result, "We should never return a null result");
        }

        [DataRow(1)]
        [DataRow(2)]
        [DataRow(8)]
        [TestMethod]
        public void IterativeSearch_SearchCancled_WeDontContinueLookingAfterSearchCancled(int degreeOfParallelism)
        {
            var cancellationSource = new CancellationTokenSource(100);
            var searchEngine = new SearchEngine() {MaxDegreeOfParallelism = degreeOfParallelism};
            var result = Task.Run(() => searchEngine.IterativeSearch(new CancelAtValue4State(1, cancellationSource, Player.Max), 1, int.MaxValue, cancellationSource.Token));
            Thread.Sleep(100);

            Assert.IsTrue(result.IsCompleted, "Search should have complated by now");
            Assert.AreEqual(4, result.Result.Evaluation, "Evaluation should have been 3; found " + result.Result.Evaluation);
        }
    }
}
