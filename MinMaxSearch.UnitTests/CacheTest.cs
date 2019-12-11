using System.Threading;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch.Cache;
using MinMaxSearch.Exceptions;

namespace MinMaxSearch.UnitTests
{
    [TestClass]
    public class CacheTest
    {
        private readonly IDeterministicState state1 = A.Fake<IDeterministicState>();
        private readonly IDeterministicState state2 = A.Fake<IDeterministicState>();
        private readonly IDeterministicState state3 = A.Fake<IDeterministicState>();
        private readonly IDeterministicState endState = A.Fake<IDeterministicState>();
        
        [TestInitialize]
        public void TestInitialize()
        {
            A.CallTo(() => state1.ToString()).Returns("State1");
            A.CallTo(() => state2.ToString()).Returns("State2");
            A.CallTo(() => state3.ToString()).Returns("State3");
            A.CallTo(() => endState.ToString()).Returns("EndState");
            endState.SetAsEndState();
            A.CallTo(() => state1.Turn).Returns(Player.Max);
            A.CallTo(() => state2.Turn).Returns(Player.Max);
            A.CallTo(() => state3.Turn).Returns(Player.Max);
            A.CallTo(() => endState.Turn).Returns(Player.Max);
            state1.SetNeigbor(state2);
            state2.SetNeigbor(state3);
            state3.SetNeigbor(endState);
        }

        [TestMethod]
        public void Search_EngineRemebersCachedStates()
        {
            var engine = new SearchEngine()
            {
                CacheMode = CacheMode.ReuseCache
            };
            engine.Search(state1, 10); // This should put all the states in the cache
            var result = engine.Search(state1, 10);
            Assert.AreEqual(result.StateSequence.Count, 1);
            Assert.IsTrue(result.AllChildrenAreDeadEnds);
        }

        [TestMethod]
        public void FillCache_EngineRemebersCachedStates()
        {
            var engine = new SearchEngine()
            {
                CacheMode = CacheMode.ReuseCache
            };
            engine.FillCache(state1, CancellationToken.None);
            var result = engine.Search(state1, 10);
            Assert.AreEqual(1, result.StateSequence.Count);
            Assert.IsTrue(result.AllChildrenAreDeadEnds);
        }

        [TestMethod]
        public void SetCustomCache_UseCustomCache()
        {
            var customCache = A.Fake<ICacheManager>();
            A.CallTo(() => customCache.ContainsState(A<IState>.That.IsSameAs(state2))).Returns(true);
            A.CallTo(() => customCache.GetStateEvaluation(A<IState>._)).Returns(1);

            var engine = new SearchEngine()
            {
                CacheMode = CacheMode.ReuseCache
            }.SetCustomCache(customCache);
            engine.Search(state1, 10);
            A.CallTo(() => endState.GetNeighbors()).MustNotHaveHappened();
        }

        [TestMethod]
        public void ClearCacheWithCondition()
        {
            var engine = new SearchEngine()
            {
                CacheMode = CacheMode.ReuseCache
            };
            engine.Search(state1, 10); // This should put all the states in the cache
            engine.GetCacheManager().Clear(s => s != state2);
            var result = engine.Search(state1, 10);
            Assert.AreNotEqual(result.StateSequence.Count, 2);
        }

        [TestMethod]
        public void ClearCache_CacheCleared()
        {
            var engine = new SearchEngine()
            {
                CacheMode = CacheMode.ReuseCache
            };
            engine.Search(state1, 10); // This should put all the states in the cache
            engine.GetCacheManager().Clear();
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
                CacheMode = cacheMode
            };
            engine.FillCache(state1, CancellationToken.None);
        }
    }
}
