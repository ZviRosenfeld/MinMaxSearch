using System;
using System.Collections.Generic;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch.Cache;
using MinMaxSearch.Exceptions;
using MinMaxSearch.UnitTests.TestStates;

namespace MinMaxSearch.UnitTests
{
    [TestClass]
    public class SearchExceptionTests
    {
        [TestMethod]
        [ExpectedException(typeof(NoNeighborsException))]
        public void Search_StartStateHasNoNeighbors_ExceptionThrown()
        {
            var state = A.Fake<IDeterministicState>();
            A.CallTo(() => state.GetNeighbors()).Returns(new List<IDeterministicState>());
            A.CallTo(() => state.Turn).Returns(Player.Empty);

            var searchEngine = TestUtils.GetBasicSearchEngine();
            searchEngine.Search(state, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(EmptyPlayerException))]
        public void Search_StartEmptyPlayer_ExceptionThrown()
        {
            var state = A.Fake<IDeterministicState>();
            A.CallTo(() => state.GetNeighbors()).Returns(new List<IDeterministicState> { state });
            A.CallTo(() => state.Turn).Returns(Player.Empty);

            var searchEngine = TestUtils.GetBasicSearchEngine();
            searchEngine.Search(state, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(EmptyPlayerException))]
        public void Search_EmptyPlayerStateInSearch_ExceptionThrown()
        {
            var state = A.Fake<IDeterministicState>();
            A.CallTo(() => state.GetNeighbors()).Returns(new List<IDeterministicState> { state });
            A.CallTo(() => state.Turn).Returns(Player.Empty);

            var startState = A.Fake<IDeterministicState>();
            A.CallTo(() => startState.GetNeighbors()).Returns(new List<IDeterministicState> { state });
            A.CallTo(() => startState.Turn).Returns(Player.Min);

            var searchEngine = TestUtils.GetBasicSearchEngine(ParallelismMode.NonParallelism);
            searchEngine.Search(startState, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(BadStateTypeException))]
        public void Search_ContainsAnIState_ExceptionThrown()
        {
            var istate = A.Fake<IState>();
            var state = A.Fake<IDeterministicState>();
            A.CallTo(() => state.GetNeighbors()).Returns(new List<IState> { istate });
            A.CallTo(() => state.Turn).Returns(Player.Max);
            A.CallTo(() => istate.Turn).Returns(Player.Max);

            var searchEngine = TestUtils.GetBasicSearchEngine(ParallelismMode.NonParallelism);
            searchEngine.Search(state, 5);
        }

        [TestMethod]
        [DataRow(-2)]
        [DataRow(0)]
        [ExpectedException(typeof(ArgumentException))]
        public void Search_NegativeDepth_ThrowException(int depth)
        {
            var engine = TestUtils.GetBasicSearchEngine();
            engine.Search(new IncreasingNumberState(1, Player.Max), depth);
        }

        [TestMethod]
        [DataRow(10, 5)]
        [ExpectedException(typeof(ArgumentException))]
        public void Search_MinScoreGreaterThanMaxScore(int minScore, int maxScore)
        {
            var engine = TestUtils.GetBasicSearchEngine();
            engine.MinScore = minScore;
            engine.MaxScore = maxScore;
            engine.Search(new IncreasingNumberState(1, Player.Max), 1);
        }

        [TestMethod]
        [DataRow(CacheMode.ReuseCache, CacheKeyType.StateAndDepth)]
        [DataRow(CacheMode.ReuseCache, CacheKeyType.StateAndPassedThroughStates)]
        [DataRow(CacheMode.NewCache, CacheKeyType.StateAndDepth)]
        [ExpectedException(typeof(MinMaxSearchException))]
        public void Search_StateDefinesDepthAndCacheKeyTypeIsStateOnly_ThrowException(CacheMode cacheMode, CacheKeyType cacheKeyType)
        {
            var engine = new SearchEngine(cacheMode, cacheKeyType) { StateDefinesDepth = true };
            engine.Search(new IncreasingNumberState(1, Player.Max), 1);
        }

        [TestMethod]
        [ExpectedException(typeof(MinMaxSearchException))]
        public void Constructor_UseCache_CacheKeyTypeUnknown_ThrowException()
        {
            new SearchEngine(CacheMode.NewCache, CacheKeyType.Unknown);
        }

        [TestMethod]
        public void Constructor_NoCache_CacheKeyTypeUnknown_NoException()
        {
            new SearchEngine(CacheMode.NoCache, CacheKeyType.Unknown);
        }
    }
}
