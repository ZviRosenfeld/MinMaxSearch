using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch.Cache;

namespace MinMaxSearch.UnitTests
{
    [TestClass]
    public class CacheTest
    {
        private readonly IDeterministicState state1 = A.Fake<IDeterministicState>();
        private readonly IDeterministicState state2 = A.Fake<IDeterministicState>();
        private readonly IDeterministicState endState = A.Fake<IDeterministicState>();
        
        [TestInitialize]
        public void TestInitialize()
        {
            A.CallTo(() => state1.ToString()).Returns("State1");
            A.CallTo(() => state2.ToString()).Returns("State2");
            A.CallTo(() => endState.ToString()).Returns("EndState");
            endState.SetAsEndState();
            A.CallTo(() => state1.Turn).Returns(Player.Max);
            A.CallTo(() => state2.Turn).Returns(Player.Max);
            A.CallTo(() => endState.Turn).Returns(Player.Max);
            state1.SetNeigbor(state2);
            state2.SetNeigbor(endState);
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
            Assert.AreEqual(result.StateSequence.Count, 0);
        }

        [TestMethod]
        public void ClearCache_CacheCleared()
        {
            var engine = new SearchEngine()
            {
                CacheMode = CacheMode.ReuseCache
            };
            engine.Search(state1, 10); // This should put all the states in the cache
            engine.ClearCache();
            var result = engine.Search(state1, 10);
            Assert.AreNotEqual(result.StateSequence.Count, 0);
        }
    }
}
