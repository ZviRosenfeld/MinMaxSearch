using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch.UnitTests.TestStates;

namespace MinMaxSearch.UnitTests
{
    [TestClass]
    public class IterativeSearchEngineTests
    {
        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void IterativeSearch_StartDepthEqualThenMaxDepth_ThrowException()
        {
            var searchEngine = new SearchEngineBuilder().Build();
            searchEngine.IterativeSearch(new IncreasingNumberState(1, Player.Max), 2, 2, CancellationToken.None);
        }

        [DataRow(1, ParallelismMode.TotalParallelism)]
        [DataRow(2, ParallelismMode.TotalParallelism)]
        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(1, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void IterativeSearch_SearchCanceldBeforeFirstSearchFinished_DontReturnNullResult(int degreeOfParallelism, ParallelismMode parallelismMode)
        {
            var cancellationSource = new CancellationTokenSource();
            cancellationSource.Cancel();
            var searchEngine = new SearchEngineBuilder { MaxDegreeOfParallelism = degreeOfParallelism, ParallelismMode = parallelismMode}.Build();
            var result = searchEngine.IterativeSearch(new IncreasingNumberState(1, Player.Max), 1, 2, cancellationSource.Token);
            Assert.IsNotNull(result, "We shouldn't have return a null result");
            Assert.IsFalse(result.IsSearchCompleted, "The search shouldn't have been completed");
        }

        [DataRow(1, ParallelismMode.TotalParallelism)]
        [DataRow(2, ParallelismMode.TotalParallelism)]
        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(1, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void IterativeSearch_SearchCancled_WeDontContinueLookingAfterSearchCancled(int degreeOfParallelism, ParallelismMode parallelismMode)
        {
            var cancellationSource = new CancellationTokenSource(100);
            var searchEngine = new SearchEngineBuilder() {MaxDegreeOfParallelism = degreeOfParallelism}.Build();
            var result = Task.Run(() => searchEngine.IterativeSearch(new CancelAtValue4State(1, cancellationSource, Player.Max), 1, int.MaxValue, cancellationSource.Token));
            result.Wait(200);

            Assert.IsTrue(result.IsCompleted, "Search should have completed by now");
            Assert.AreEqual(4, result.Result.Evaluation, "Evaluation should have been 3; found " + result.Result.Evaluation);
            Assert.IsTrue(result.Result.IsSearchCompleted, "The search should have been completed");
        }

        [DataRow(1, ParallelismMode.TotalParallelism)]
        [DataRow(2, ParallelismMode.TotalParallelism)]
        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(1, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void IterativeSearch_TimeoutSet_WeDontContinueLookingAfterTimeout(int degreeOfParallelism, ParallelismMode parallelismMode)
        {
            var searchEngine = new SearchEngineBuilder() { MaxDegreeOfParallelism = degreeOfParallelism, ParallelismMode = parallelismMode}.Build();
            var result = Task.Run(() => searchEngine.IterativeSearch(new SlowState(0), 1, int.MaxValue, TimeSpan.FromMilliseconds(200)));
            
            Assert.IsFalse(result.IsCompleted, "We shouldn't have stopped running yet");

            result.Wait(400);

            Assert.IsTrue(result.IsCompleted, "Search should have completed by now");
            Assert.IsTrue(result.Result.IsSearchCompleted, "The search should have been completed");
        }

        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(2, ParallelismMode.FirstLevelOnly)]
        [DataRow(8, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void Search_SearchDepthIsRight(int depth, ParallelismMode parallelismMode)
        {
            var engine = new SearchEngineBuilder() {MaxDegreeOfParallelism = 8}.Build();
            var result = engine.IterativeSearch(new IncreasingNumberState(8, Player.Max), 1, depth, CancellationToken.None);
            Assert.AreEqual(depth, result.SearchDepth, "Got wring depth");
        }

        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(2, ParallelismMode.FirstLevelOnly)]
        [DataRow(8, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void Search_IsSearchCompletedIsRight(int depth, ParallelismMode parallelismMode)
        {
            var engine = new SearchEngineBuilder() { MaxDegreeOfParallelism = 8 }.Build();
            var result = engine.IterativeSearch(new IncreasingNumberState(8, Player.Max), 1, depth, CancellationToken.None);
            Assert.IsTrue(result.IsSearchCompleted, "The search should have been completed");
        }

        [TestMethod]
        public void IterativeSearch_ResultsContainsSearchTime()
        {
            var searchEngine = new SearchEngineBuilder().Build();
            var result1 = searchEngine.Search(new IncreasingNumberState(1, Player.Max), 20);
            var result2 = searchEngine.IterativeSearch(new IncreasingNumberState(1, Player.Max), 1, 20, CancellationToken.None);

            Assert.AreNotEqual(TimeSpan.Zero, result1.SearchTime, $"{nameof(result1)}.{nameof(result1.SearchTime)} shouldn't be zero");
            Assert.AreNotEqual(TimeSpan.Zero, result2.SearchTime, $"{nameof(result2)}.{nameof(result2.SearchTime)} shouldn't be zero");
            Assert.IsTrue(result1.SearchTime < result2.SearchTime, $"{nameof(result1)}.{nameof(result1.SearchTime)} = {result1.SearchTime}; {nameof(result2)}.{nameof(result2.SearchTime)} = {result2.SearchTime}");
        }

        [TestMethod]
        public void IterativeSearch_MinAndMaxSearch_GetFullRangeOfDepth()
        {
            var range = new [] {4, 5, 6, 7};
            var engine = new SearchEngine(new TestSearchWorkerCheckMaxDepth(range));
            engine.IterativeSearch(new IncreasingNumberState(2, Player.Max), 4, 7, CancellationToken.None);
        }

        [TestMethod]
        public void IterativeSearch_ArrayRange_GetFullRangeOfDepth()
        {
            var range = new[] { 1, 3, 5 };
            var engine = new SearchEngine(new TestSearchWorkerCheckMaxDepth(range));
            engine.IterativeSearch(new IncreasingNumberState(2, Player.Max), range, CancellationToken.None);
        }

        [TestMethod]
        public void IterativeSearch_IgnoreDepthsAlreadyChecked()
        {
            var actualRange = new[] { 1, 3, 2, 4, 4 };
            var expactedRange = new [] { 1, 3, 4};
            var engine = new SearchEngine(new TestSearchWorkerCheckMaxDepth(expactedRange));
            engine.IterativeSearch(new IncreasingNumberState(2, Player.Max), actualRange, CancellationToken.None);
        }
    }
}
