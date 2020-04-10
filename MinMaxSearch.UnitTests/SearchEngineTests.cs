using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch.Cache;
using MinMaxSearch.Pruners;
using MinMaxSearch.UnitTests.SampleTrees;
using MinMaxSearch.UnitTests.TestStates;

namespace MinMaxSearch.UnitTests
{
    [TestClass]
    public class SearchEngineTests
    {
        [DataRow(1, ParallelismMode.TotalParallelism)]
        [DataRow(2, ParallelismMode.TotalParallelism)]
        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(1, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void Search_IsSearchCompletedTrue(int degreeOfParallelism, ParallelismMode parallelismMode)
        {
            var tree = new UnaryDeterministicTree();

            var engine = TestUtils.GetBasicSearchEngine(parallelismMode, degreeOfParallelism);
            var result = engine.Search(tree.State3, 5);

            Assert.IsTrue(result.IsSearchCompleted, "Search should have been completed");
        }

        [DataRow(1, ParallelismMode.NonParallelism, true)]
        [DataRow(1, ParallelismMode.NonParallelism, false)]
        [DataRow(2, ParallelismMode.TotalParallelism, false)]
        [DataRow(8, ParallelismMode.TotalParallelism, true)]
        [DataRow(1, ParallelismMode.FirstLevelOnly, true)]
        [TestMethod]
        public void Search_WinMovesTwoAndThreeStepsAway_FindTheNearerOne(int degreeOfParallelism, ParallelismMode parallelismMode, bool dieEarly)
        {
            var tree = new UnevenDeterministicTree();

            tree.EndState1.SetEvaluationTo(15);
            tree.EndState2.SetEvaluationTo(11);
            tree.EndState3.SetEvaluationTo(18);

            var engine = new SearchEngine(CacheMode.NewCache)
            {
                MaxDegreeOfParallelism = degreeOfParallelism,
                FavorShortPaths = true,
                DieEarly = dieEarly,
                MaxScore = 10,
                ParallelismMode = parallelismMode,
                SkipEvaluationForFirstNodeSingleNeighbor = false,
            };
            var result = engine.Search(tree.RootState, 5);

            Assert.AreEqual(tree.EndState1, result.StateSequence.Last(), nameof(tree.EndState1) + " should have been good enough");
        }

        [DataRow(false, false)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        [TestMethod]
        public void Search_FindWinThreeStepsAway_DontCheckNeigborsFourStepsAway(bool unstableState, bool favorSortPaths)
        {
            var tree = new DeterministicTree();

            tree.EndState1.SetEvaluationTo(15);
            A.CallTo(() => tree.EndState2.Evaluate(A<int>._, A<List<IState>>._)).Invokes(() => throw new Exception("We shouldn't have needed to check " + nameof(tree.EndState2)));

            var engine = new SearchEngine(CacheMode.NewCache)
            {
                FavorShortPaths = favorSortPaths,
                DieEarly = true,
                MaxScore = 10,
                IsUnstableState = (s, i, l) => unstableState,
                ParallelismMode = ParallelismMode.NonParallelism,
                SkipEvaluationForFirstNodeSingleNeighbor = false,
            };
            var result = engine.Search(tree.RootState, 6);

            Assert.AreEqual(tree.EndState1, result.StateSequence.Last(), nameof(tree.EndState1) + " should have been good enough");
        }

        [DataRow(1, ParallelismMode.TotalParallelism)]
        [DataRow(2, ParallelismMode.TotalParallelism)]
        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(1, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void Search_MaxHasTwoTurnsInARow_FindBestMove(int degreeOfParallelism, ParallelismMode parallelismMode)
        {
            var tree = new DeterministicTree();
            A.CallTo(() => tree.ChildState2.Turn).Returns(Player.Max);
            tree.EndState2.SetEvaluationTo(2);
            tree.EndState3.SetEvaluationTo(3);

            var engine = TestUtils.GetBasicSearchEngine(parallelismMode, degreeOfParallelism);
            var result = engine.Search(tree.ChildState2, 5);

            Assert.AreEqual(tree.EndState3, result.NextMove, "Actually found " + result.NextMove);
        }

        [DataRow(1, ParallelismMode.TotalParallelism)]
        [DataRow(2, ParallelismMode.TotalParallelism)]
        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(1, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void Search_RowOfMixedTurns_FindBest(int degreeOfParallelism, ParallelismMode parallelismMode)
        {
            var tree = new DeterministicTree2();
            A.CallTo(() => tree.ChildState1.Turn).Returns(Player.Max);
            tree.EndState1.SetEvaluationTo(5);
            tree.EndState2.SetEvaluationTo(6);
            tree.EndState3.SetEvaluationTo(4);
            tree.EndState4.SetEvaluationTo(7);

            var engine = TestUtils.GetBasicSearchEngine(parallelismMode, degreeOfParallelism);
            var result = engine.Search(tree.RootState, 5);

            Assert.AreEqual(tree.EndState2, result.StateSequence.Last());
            Assert.AreEqual(6, result.Evaluation);
        }

        [DataRow(1, ParallelismMode.TotalParallelism)]
        [DataRow(2, ParallelismMode.TotalParallelism)]
        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(1, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void Search_DontStopWithUnstableState(int degreeOfParallelism, ParallelismMode parallelismMode)
        {
            var searchEngine = new SearchEngine(CacheMode.NewCache)
            {
                IsUnstableState = (s, d, l) => s.Evaluate(d, l) < 10,
                MaxDegreeOfParallelism = degreeOfParallelism,
                ParallelismMode = parallelismMode,
                SkipEvaluationForFirstNodeSingleNeighbor = false,
            };
            var result = searchEngine.Search(new IncreasingNumberState(0, Player.Max), 1);

            Assert.AreEqual(10, result.Evaluation, "Engine seems to have stopped before reaching a stable state");
        }

        [DataRow(1, ParallelismMode.TotalParallelism)]
        [DataRow(2, ParallelismMode.TotalParallelism)]
        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(1, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void Search_CheckThatRecordPassThroughStatesIsWorking(int degreeOfParallelism, ParallelismMode parallelismMode)
        {
            var tree = new DeterministicTree();
            A.CallTo(() => tree.EndState1.Evaluate(A<int>._, A<List<IState>>.That.IsEmpty()))
                .Throws(new Exception("passedStats list should have been empty"));
            A.CallTo(() => tree.EndState1.Evaluate(A<int>._, A<List<IState>>._))
                .Invokes((int i, List<IState> l) =>
                {
                    Assert.AreEqual(1, l.Count, "passThroughStates should only have one node (state1)");
                    Assert.IsTrue(l.Contains(tree.ChildState1), "passThroughStates should contain state1");
                });
            
            var searchEngine = TestUtils.GetBasicSearchEngine(parallelismMode, degreeOfParallelism);
            searchEngine.Search(tree.ChildState1, 5);
        }

        [DataRow(1, ParallelismMode.TotalParallelism)]
        [DataRow(2, ParallelismMode.TotalParallelism)]
        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(1, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void Search_DieEarllyOptionWorks(int degreeOfParallelism, ParallelismMode parallelismMode)
        {
            var tree = new DeterministicTree3();
            tree.EndState1.SetEvaluationTo(10);
            tree.EndState2.SetEvaluationTo(15);
            tree.EndState3.SetEvaluationTo(0);

            var searchEngine = new SearchEngine(CacheMode.NewCache)
            {
                DieEarly = true,
                MaxScore = 5,
                MinScore = 5,
                MaxDegreeOfParallelism = degreeOfParallelism,
                ParallelismMode = parallelismMode,
                SkipEvaluationForFirstNodeSingleNeighbor = false,
            };
            var evaluation = searchEngine.Search(tree.RootState, 2);

            Assert.AreEqual(tree.EndState1, evaluation.StateSequence.Last(), "Should have ended with" + nameof(tree.EndState1));
        }

        [DataRow(1, ParallelismMode.TotalParallelism)]
        [DataRow(2, ParallelismMode.TotalParallelism)]
        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(1, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void Search_CheckPreventLoopPrunerWorks(int degreeOfParallelism, ParallelismMode parallelismMode)
        {
            var searchEngine = new SearchEngine(CacheMode.NewCache)
            {
                PreventLoops = true,
                MaxDegreeOfParallelism = degreeOfParallelism,
                ParallelismMode = parallelismMode,
                SkipEvaluationForFirstNodeSingleNeighbor = false
            };
            searchEngine.Search(new ThrowExceptionAtDepthThreeState(0, Player.Max), 5);
        }

        [DataRow(1, ParallelismMode.TotalParallelism)]
        [DataRow(2, ParallelismMode.TotalParallelism)]
        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(1, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void Search_TaskCanceld_DontContinueSearching(int degreeOfParallelism, ParallelismMode parallelismMode)
        {
            var tree = new UnaryDeterministicTree();
            var cancellationSource = new CancellationTokenSource();

            A.CallTo(() => tree.RootState.GetNeighbors()).ReturnsLazily(() =>
            {
                cancellationSource.Cancel();
                return new List<IDeterministicState> {tree.State2};
            });

            tree.RootState.SetEvaluationTo(1);
            tree.State2.SetEvaluationTo(2);
            tree.EndState.SetEvaluationTo(2);
            
            var searchEngine = TestUtils.GetBasicSearchEngine(parallelismMode, degreeOfParallelism);
            var result = searchEngine.Search(tree.RootState, 5, cancellationSource.Token);

            Assert.AreEqual(1, result.Evaluation);
            Assert.AreEqual(0, result.StateSequence.Count, "We shouldn't have gotten to past " + nameof(tree.RootState));
            Assert.IsFalse(result.IsSearchCompleted, "The search shouldn't have been completed");
        }

        [TestMethod]
        public void Search_ResultsContainsSearchTime()
        {
            var searchEngine = TestUtils.GetBasicSearchEngine();
            var result1 = searchEngine.Search(new IncreasingNumberState(1, Player.Max), 5);
            var result2 = searchEngine.Search(new IncreasingNumberState(1, Player.Max), 20);

            Assert.AreNotEqual(TimeSpan.Zero, result1.SearchTime, $"{nameof(result1)}.{nameof(result1.SearchTime)} shouldn't be zero");
            Assert.AreNotEqual(TimeSpan.Zero, result2.SearchTime, $"{nameof(result2)}.{nameof(result2.SearchTime)} shouldn't be zero");
            Assert.IsTrue(result1.SearchTime < result2.SearchTime, $"{nameof(result1)}.{nameof(result1.SearchTime)} = {result1.SearchTime}; {nameof(result2)}.{nameof(result2.SearchTime)} = {result2.SearchTime}");
        }
        
        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(2, ParallelismMode.FirstLevelOnly)]
        [DataRow(8, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void Search_SearchDepthIsRight(int depth, ParallelismMode parallelismMode)
        {
            var engine = TestUtils.GetBasicSearchEngine(parallelismMode, 8);
            var result = engine.Search(new IncreasingNumberState(8, Player.Max), depth);
            Assert.AreEqual(depth, result.SearchDepth, "Got wring depth");
        }

        [DataRow(1, ParallelismMode.NonParallelism)]
        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(1, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void CancelSearch_ReturnBestResultSoFar(int degreeOfParallelism, ParallelismMode parallelismMode)
        {
            var tree = new EndlessTree();
            tree.ChildState1.SetEvaluationTo(1);
            tree.ChildState2.SetEvaluationTo(3);
            var cancellationSource = new CancellationTokenSource(20);
            var engine = TestUtils.GetBasicSearchEngine(parallelismMode, degreeOfParallelism);

            var result = engine.SearchAsync(tree.RootState, int.MaxValue, cancellationSource.Token).Result;

            Assert.AreEqual(3, result.Evaluation, "Didn't get a good enough state");
            if (parallelismMode == ParallelismMode.NonParallelism)
                A.CallTo(() => tree.ChildState2.GetNeighbors()).MustNotHaveHappened();
        }

        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(1, ParallelismMode.NonParallelism)]
        [DataRow(1, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void Search_PrunerTest(int maxDegreeOfParallelism, ParallelismMode parallelismMode)
        {
            var tree = new DeterministicTree();
            var pruner = A.Fake<IPruner>();
            A.CallTo(() => pruner.ShouldPrune(A<IState>._, A<int>._, A<List<IState>>._))
                .ReturnsLazily((IState s, int d, List<IState> l) => s.Equals(tree.ChildState2));
            var engine = TestUtils.GetBasicSearchEngine(parallelismMode, maxDegreeOfParallelism);
            engine.AddPruner(pruner);

            tree.EndState1.SetEvaluationTo(1);
            tree.EndState2.SetEvaluationTo(2);
            tree.EndState3.SetEvaluationTo(2);
            tree.ChildState2.SetEvaluationTo(-2);

            var result = engine.Search(tree.RootState, 4);
            Assert.AreEqual(1, result.Evaluation, $"We should have pruned away {nameof(tree.ChildState2)}");
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void SkipEvaluationForFirstNodeSingleNeighborTest(bool skip)
        {
            var engine = new SearchEngine(){SkipEvaluationForFirstNodeSingleNeighbor = skip};
            var tree = new UnaryDeterministicTree();
            tree.State2.SetEvaluationTo(2);
            tree.EndState.SetEvaluationTo(3);

            var result = engine.Search(tree.RootState, 10);

            if (skip)
            {
                Assert.AreEqual(2, result.Evaluation);
                Assert.AreEqual(1, result.StateSequence.Count);
                A.CallTo(() => tree.State2.GetNeighbors()).MustNotHaveHappened();
            }
            else
            {
                Assert.AreEqual(3, result.Evaluation);
                Assert.AreEqual(3, result.StateSequence.Count);
            }
        }
    }
}
