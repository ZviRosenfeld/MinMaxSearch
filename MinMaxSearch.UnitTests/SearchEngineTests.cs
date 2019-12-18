using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch.Cache;
using MinMaxSearch.UnitTests.TestStates;

namespace MinMaxSearch.UnitTests
{
    [TestClass]
    public class SearchEngineTests
    {
        private readonly IDeterministicState state1 = A.Fake<IDeterministicState>();
        private readonly IDeterministicState state2 = A.Fake<IDeterministicState>();
        private readonly IDeterministicState state3 = A.Fake<IDeterministicState>();
        private readonly IDeterministicState endState1 = A.Fake<IDeterministicState>();
        private readonly IDeterministicState endState2 = A.Fake<IDeterministicState>();
        private readonly IDeterministicState endState3 = A.Fake<IDeterministicState>();

        [TestInitialize]
        public void TestInitialize()
        {
            A.CallTo(() => state1.ToString()).Returns("State1");
            A.CallTo(() => state2.ToString()).Returns("State2");
            A.CallTo(() => state3.ToString()).Returns("State3");
            A.CallTo(() => endState1.ToString()).Returns("EndState1");
            A.CallTo(() => endState2.ToString()).Returns("EndState2");
            A.CallTo(() => endState3.ToString()).Returns("EndState3");
            endState1.SetAsEndState();
            endState2.SetAsEndState();
            endState3.SetAsEndState();
            A.CallTo(() => state1.Turn).Returns(Player.Max);
            A.CallTo(() => state2.Turn).Returns(Player.Max);
            A.CallTo(() => state3.Turn).Returns(Player.Max);
            A.CallTo(() => endState1.Turn).Returns(Player.Max);
            A.CallTo(() => endState2.Turn).Returns(Player.Max);
            A.CallTo(() => endState3.Turn).Returns(Player.Max);
        }

        [DataRow(1, ParallelismMode.TotalParallelism)]
        [DataRow(2, ParallelismMode.TotalParallelism)]
        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(1, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void Search_IsSearchCompletedTrue(int degreeOfParallelism, ParallelismMode parallelismMode)
        {
            state1.SetNeigbor(endState1);

            var engine = TestUtils.GetBasicSearchEngine(parallelismMode, degreeOfParallelism);
            var result = engine.Search(state1, 5);

            Assert.IsTrue(result.IsSearchCompleted, "Search should have been completed");
        }

        [DataRow(1, ParallelismMode.TotalParallelism)]
        [DataRow(2, ParallelismMode.TotalParallelism)]
        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(1, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void Search_WinMovesTwoAndThreeStepsAway_FindTheNearerOne(int degreeOfParallelism, ParallelismMode parallelismMode)
        {
            state1.SetNeigbors(new[] { state2, endState1, state3 });
            state2.SetNeigbor(endState2);
            state3.SetNeigbor(endState3);

            endState1.SetEvaluationTo(15);
            endState2.SetEvaluationTo(11);
            endState3.SetEvaluationTo(18);

            var engine = new SearchEngine()
            {
                MaxDegreeOfParallelism = degreeOfParallelism,
                FavorShortPaths = true,
                DieEarly = true,
                MaxScore = 10,
                ParallelismMode = parallelismMode,
                SkipEvaluationForFirstNodeSingleNeighbor = false,
                CacheMode = CacheMode.NewCache
            };
            var result = engine.Search(state1, 5);

            Assert.AreEqual(endState1, result.StateSequence.Last(), nameof(endState1) + " should have been good enough");
        }

        [DataRow(false)]
        [DataRow(true)]
        [TestMethod]
        public void Search_FindWinThreeStepsAway_DontCheckNeigborsFourStepsAway(bool unstableState)
        {
            state1.SetNeigbors(new [] {state2, state3});
            state2.SetNeigbor(endState1);
            state3.SetNeigbor(endState2);

            endState1.SetEvaluationTo(15);
            A.CallTo(() => endState2.Evaluate(A<int>._, A<List<IState>>._)).Invokes(() => throw new Exception("We shouldn't have needed to check " + nameof(endState2)));

            var engine = new SearchEngine()
            {
                FavorShortPaths = true,
                DieEarly = true,
                MaxScore = 10,
                IsUnstableState = (s, i, l) => unstableState,
                ParallelismMode = ParallelismMode.NonParallelism,
                SkipEvaluationForFirstNodeSingleNeighbor = false,
                CacheMode = CacheMode.NewCache
            };
            var result = engine.Search(state1, 6);

            Assert.AreEqual(endState1, result.StateSequence.Last(), nameof(endState1) + " should have been good enough");
        }

        [DataRow(1, ParallelismMode.TotalParallelism)]
        [DataRow(2, ParallelismMode.TotalParallelism)]
        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(1, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void Search_MaxHasTwoTurnsInARow_FindBestMove(int degreeOfParallelism, ParallelismMode parallelismMode)
        {
            state1.SetNeigbors(new List<IState> { endState1, endState2, endState3 });

            endState1.SetEvaluationTo(2);
            endState2.SetEvaluationTo(1);
            endState3.SetEvaluationTo(3);

            var engine = TestUtils.GetBasicSearchEngine(parallelismMode, degreeOfParallelism);
            var result = engine.Search(state1, 5);

            Assert.AreEqual(endState3, result.NextMove, "Actually found " + result.NextMove);
        }

        [DataRow(1, ParallelismMode.TotalParallelism)]
        [DataRow(2, ParallelismMode.TotalParallelism)]
        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(1, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void Search_RowOfMixedMove_FindBest(int degreeOfParallelism, ParallelismMode parallelismMode)
        {
            state1.SetNeigbors(new List<IState> { endState1, endState2, endState3 });

            A.CallTo(() => endState2.Turn).Returns(Player.Min);

            endState1.SetEvaluationTo(2);
            endState2.SetEvaluationTo(1);
            endState3.SetEvaluationTo(3);

            var engine = TestUtils.GetBasicSearchEngine(parallelismMode, degreeOfParallelism);
            var result = engine.Search(state1, 5);

            Assert.AreEqual(endState3, result.NextMove, "Actually found " + result.NextMove);
        }

        [DataRow(1, ParallelismMode.TotalParallelism)]
        [DataRow(2, ParallelismMode.TotalParallelism)]
        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(1, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void Search_DontStopWithUnstableState(int degreeOfParallelism, ParallelismMode parallelismMode)
        {
            var searchEngine = new SearchEngine()
            {
                IsUnstableState = (s, d, l) => s.Evaluate(d, l) < 10,
                MaxDegreeOfParallelism = degreeOfParallelism,
                ParallelismMode = parallelismMode,
                SkipEvaluationForFirstNodeSingleNeighbor = false,
                CacheMode = CacheMode.NewCache
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
            A.CallTo(() => endState1.Evaluate(A<int>._, A<List<IState>>.That.IsEmpty()))
                .Throws(new Exception("passedStats list should have been empty"));
            A.CallTo(() => endState1.Evaluate(A<int>._, A<List<IState>>._))
                .Invokes((int i, List<IState> l) =>
                {
                    Assert.AreEqual(1, l.Count, "passThroughStates should only have one node (state1)");
                    Assert.IsTrue(l.Contains(state1), "passThroughStates should contain state1");
                });

            state1.SetNeigbors(new List<IDeterministicState> { endState1 });

            A.CallTo(() => endState1.Turn).Returns(Player.Min);

            var searchEngine = TestUtils.GetBasicSearchEngine(parallelismMode, degreeOfParallelism);
            searchEngine.Search(state1, 5);
        }

        [DataRow(1, ParallelismMode.TotalParallelism)]
        [DataRow(2, ParallelismMode.TotalParallelism)]
        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(1, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void Search_DieEarllyOptionWorks(int degreeOfParallelism, ParallelismMode parallelismMode)
        {
            endState1.SetEvaluationTo(10);
            endState2.SetEvaluationTo(15);
            endState3.SetEvaluationTo(0);
            state1.SetNeigbors(new List<IState> { endState1, endState2, endState3 });
            A.CallTo(() => endState1.Turn).Returns(Player.Min);
            A.CallTo(() => endState2.Turn).Returns(Player.Min);
            A.CallTo(() => endState3.Turn).Returns(Player.Min);

            var searchEngine = new SearchEngine()
            {
                DieEarly = true,
                MaxScore = 5,
                MinScore = 5,
                MaxDegreeOfParallelism = degreeOfParallelism,
                ParallelismMode = parallelismMode,
                SkipEvaluationForFirstNodeSingleNeighbor = false,
                CacheMode = CacheMode.NewCache
            };
            var evaluation = searchEngine.Search(state1, 2);

            Assert.AreEqual(endState1, evaluation.StateSequence.Last(), "Should have ended with" + nameof(endState1));
        }

        [DataRow(1, ParallelismMode.TotalParallelism)]
        [DataRow(2, ParallelismMode.TotalParallelism)]
        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(1, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void Search_CheckPreventLoopPrunerWorks(int degreeOfParallelism, ParallelismMode parallelismMode)
        {
            var searchEngine = new SearchEngine()
            {
                PreventLoops = true,
                MaxDegreeOfParallelism = degreeOfParallelism,
                ParallelismMode = parallelismMode,
                SkipEvaluationForFirstNodeSingleNeighbor = false,
                CacheMode = CacheMode.NewCache
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
            var cancellationSource = new CancellationTokenSource();

            A.CallTo(() => state1.GetNeighbors()).ReturnsLazily(() =>
            {
                cancellationSource.Cancel();
                return new List<IDeterministicState> {endState1};
            });

            state1.SetEvaluationTo(1);
            endState1.SetEvaluationTo(2);
            
            A.CallTo(() => state1.Turn).Returns(Player.Min);

            var searchEngine = TestUtils.GetBasicSearchEngine(parallelismMode, degreeOfParallelism);
            var result = searchEngine.Search(state1, 5, cancellationSource.Token);

            Assert.AreEqual(1, result.Evaluation);
            Assert.AreEqual(0, result.StateSequence.Count, "We shouldn't have gotten to state3");
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
            var engine = TestUtils.GetBasicSearchEngine(maxDegreeOfParallelism: 8);
            var result = engine.Search(new IncreasingNumberState(8, Player.Max), depth);
            Assert.AreEqual(depth, result.SearchDepth, "Got wring depth");
        }

        [DataRow(1, ParallelismMode.NonParallelism)]
        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(1, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void CancelSearch_ReturnBestResultSoFar(int degreeOfParallelism, ParallelismMode parallelismMode)
        {
            state1.SetNeigbors(state2, state3);
            state2.SetNeigbor(state2);
            state3.SetNeigbor(state3);
            state2.SetEvaluationTo(1);
            state3.SetEvaluationTo(3);
            var cancellationSource = new CancellationTokenSource(20);
            var engine = TestUtils.GetBasicSearchEngine(parallelismMode, degreeOfParallelism);

            var result = engine.SearchAsync(state1, int.MaxValue, cancellationSource.Token).Result;

            Assert.AreEqual(3, result.Evaluation, "Didn't get a good enough state");
            if (parallelismMode == ParallelismMode.NonParallelism)
                A.CallTo(() => state3.GetNeighbors()).MustNotHaveHappened();
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void SkipEvaluationForFirstNodeSingleNeighborTest(bool skip)
        {
            var engine = new SearchEngine(){SkipEvaluationForFirstNodeSingleNeighbor = skip};

            state1.SetNeigbor(state2);
            state2.SetNeigbor(endState1);
            state2.SetEvaluationTo(2);
            endState1.SetEvaluationTo(3);

            var result = engine.Search(state1, 10);

            if (skip)
            {
                Assert.AreEqual(2, result.Evaluation);
                Assert.AreEqual(1, result.StateSequence.Count);
            }
            else
            {
                Assert.AreEqual(3, result.Evaluation);
                Assert.AreEqual(2, result.StateSequence.Count);
            }
        }
    }
}
