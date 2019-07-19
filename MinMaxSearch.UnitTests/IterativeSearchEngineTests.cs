﻿using System;
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
            var searchEngine = new SearchEngine();
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
            var searchEngine = new SearchEngine {MaxDegreeOfParallelism = degreeOfParallelism, ParallelismMode = parallelismMode};
            var result = searchEngine.IterativeSearch(new IncreasingNumberState(1, Player.Max), 1, 2, cancellationSource.Token);
            Assert.IsNotNull(result, "We should never return a null result");
        }

        [DataRow(1, ParallelismMode.TotalParallelism)]
        [DataRow(2, ParallelismMode.TotalParallelism)]
        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(1, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void IterativeSearch_SearchCancled_WeDontContinueLookingAfterSearchCancled(int degreeOfParallelism, ParallelismMode parallelismMode)
        {
            var cancellationSource = new CancellationTokenSource(100);
            var searchEngine = new SearchEngine() {MaxDegreeOfParallelism = degreeOfParallelism};
            var result = Task.Run(() => searchEngine.IterativeSearch(new CancelAtValue4State(1, cancellationSource, Player.Max), 1, int.MaxValue, cancellationSource.Token));
            result.Wait(200);

            Assert.IsTrue(result.IsCompleted, "Search should have complated by now");
            Assert.AreEqual(4, result.Result.Evaluation, "Evaluation should have been 3; found " + result.Result.Evaluation);
        }

        [DataRow(1, ParallelismMode.TotalParallelism)]
        [DataRow(2, ParallelismMode.TotalParallelism)]
        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(1, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void IterativeSearch_TimeoutSet_WeDontContinueLookingAfterTimeout(int degreeOfParallelism, ParallelismMode parallelismMode)
        {
            var searchEngine = new SearchEngine() { MaxDegreeOfParallelism = degreeOfParallelism, ParallelismMode = parallelismMode};
            var result = Task.Run(() => searchEngine.IterativeSearch(new SlowState(0), 1, int.MaxValue, TimeSpan.FromMilliseconds(100)));
            
            Assert.IsFalse(result.IsCompleted, "We shouldn't have stopped running yet");

            result.Wait(400);

            Assert.IsTrue(result.IsCompleted, "Search should have complated by now");
        }
    }
}
