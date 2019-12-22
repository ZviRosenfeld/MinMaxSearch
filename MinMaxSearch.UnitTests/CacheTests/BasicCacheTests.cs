using System.Threading;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch.Cache;
using MinMaxSearch.Exceptions;

namespace MinMaxSearch.UnitTests.CacheTests
{
    [TestClass]
    public class BasicCacheTests
    {
        // This class uses the folowing tree:
        // stat1 -> sate2 -> state3 -> EndState1

        private readonly IDeterministicState state1 = A.Fake<IDeterministicState>();
        private readonly IDeterministicState state2 = A.Fake<IDeterministicState>();
        private readonly IDeterministicState state3 = A.Fake<IDeterministicState>();
        private readonly IDeterministicState endState1 = A.Fake<IDeterministicState>();

        [TestInitialize]
        public void TestInitialize()
        {
            A.CallTo(() => state1.ToString()).Returns("State1");
            A.CallTo(() => state2.ToString()).Returns("State2");
            A.CallTo(() => state3.ToString()).Returns("State3");
            A.CallTo(() => endState1.ToString()).Returns("EndState1");
           
            A.CallTo(() => state1.Turn).Returns(Player.Max);
            A.CallTo(() => state2.Turn).Returns(Player.Max);
            A.CallTo(() => state3.Turn).Returns(Player.Max);
            A.CallTo(() => endState1.Turn).Returns(Player.Max);
            
            state1.SetNeigbor(state2);
            state2.SetNeigbor(state3);
            state3.SetNeigbor(endState1);
            endState1.SetAsEndState();
        }

        [TestMethod]
        public void Search_EngineRemebersCachedStates()
        {
            var engine = new SearchEngine()
            {
                CacheMode = CacheMode.ReuseCache,
                SkipEvaluationForFirstNodeSingleNeighbor = false
            };
            engine.Search(state1, 10); // This should put all the states in the cache
            var result = engine.Search(state1, 10);
            Assert.AreEqual(result.StateSequence.Count, 1);
            Assert.IsTrue(result.FullTreeSearchedOrPruned);
        }

        [TestMethod]
        public void FillCache_EngineRemebersCachedStates()
        {
            var engine = new SearchEngine()
            {
                CacheMode = CacheMode.ReuseCache,
                SkipEvaluationForFirstNodeSingleNeighbor = false
            };
            engine.FillCache(state1, CancellationToken.None);
            var result = engine.Search(state1, 10);
            Assert.AreEqual(1, result.StateSequence.Count);
            Assert.IsTrue(result.FullTreeSearchedOrPruned);
        }

        [TestMethod]
        public void SetCustomCache_UseCustomCache()
        {
            var customCache = A.Fake<ICacheManager>();
            A.CallTo(() => customCache.GetStateEvaluation(A<IState>._)).Returns(new EvaluationRange(1));

            var engine = new SearchEngine()
            {
                CacheMode = CacheMode.ReuseCache,
                CacheManager = customCache,
                SkipEvaluationForFirstNodeSingleNeighbor = false
            };
            engine.Search(state1, 10);
            A.CallTo(() => endState1.GetNeighbors()).MustNotHaveHappened();
        }

        [TestMethod]
        public void ClearCacheWithCondition()
        {
            var engine = new SearchEngine()
            {
                CacheMode = CacheMode.ReuseCache,
                SkipEvaluationForFirstNodeSingleNeighbor = false
            };
            engine.Search(state1, 10); // This should put all the states in the cache
            engine.CacheManager.Clear(s => s != state2);
            var result = engine.Search(state1, 10);
            Assert.AreNotEqual(result.StateSequence.Count, 2);
        }

        [TestMethod]
        public void ClearCache_CacheCleared()
        {
            var engine = new SearchEngine()
            {
                CacheMode = CacheMode.ReuseCache,
                SkipEvaluationForFirstNodeSingleNeighbor = false
            };
            engine.Search(state1, 10); // This should put all the states in the cache
            engine.CacheManager.Clear();
            var result = engine.Search(state1, 10);
            Assert.AreNotEqual(result.StateSequence.Count, 1);
        }

        [TestMethod]
        [DataRow(CacheMode.NewCache)]
        [DataRow(CacheMode.NoCache)]
        [ExpectedException(typeof(MinMaxSearchException))]
        public void FillCache_CacheModeNotSetToResueCache_ThrowsException(CacheMode cacheMode)
        {
            var engine = new SearchEngine()
            {
                CacheMode = cacheMode,
                SkipEvaluationForFirstNodeSingleNeighbor = false
            };
            engine.FillCache(state1, CancellationToken.None);
        }
    }
}
