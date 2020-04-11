using System.Collections.Generic;
using System.Threading;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch.Cache;
using MinMaxSearch.Exceptions;
using MinMaxSearch.UnitTests.SampleTrees;

namespace MinMaxSearch.UnitTests.CacheTests
{
    [TestClass]
    public class BasicCacheTests
    {
        [TestMethod]
        public void Search_EngineRemebersCachedStates()
        {
            var tree = new UnaryDeterministicTree();
            var engine = new SearchEngine(CacheMode.ReuseCache, CacheKeyType.StateOnly)
            {
                SkipEvaluationForFirstNodeSingleNeighbor = false
            };
            engine.Search(tree.RootState, 10); // This should put all the states in the cache
            var result = engine.Search(tree.RootState, 10);
            Assert.AreEqual(result.StateSequence.Count, 1);
            Assert.IsTrue(result.FullTreeSearchedOrPruned);
        }

        [TestMethod]
        public void FillCache_EngineRemembersCachedStates()
        {
            var tree = new UnaryDeterministicTree();
            var engine = new SearchEngine(CacheMode.ReuseCache, CacheKeyType.StateOnly)
            {
                SkipEvaluationForFirstNodeSingleNeighbor = false
            };
            engine.FillCache(tree.RootState, CancellationToken.None);
            var result = engine.Search(tree.RootState, 10);
            Assert.AreEqual(1, result.StateSequence.Count);
            Assert.IsTrue(result.FullTreeSearchedOrPruned);
        }

        [TestMethod]
        public void SetCustomCache_UseCustomCache()
        {
            var tree = new UnaryDeterministicTree();
            var customCache = A.Fake<ICacheManager>();
            A.CallTo(() => customCache.GetStateEvaluation(A<IState>._, A<int>._, A<IList<IState>>._)).Returns(new EvaluationRange(1));

            var engine = new SearchEngine(CacheMode.ReuseCache, () => customCache)
            {
                SkipEvaluationForFirstNodeSingleNeighbor = false
            };
            engine.Search(tree.RootState, 10);
            A.CallTo(() => tree.EndState.GetNeighbors()).MustNotHaveHappened();
        }

        [TestMethod]
        public void ClearCacheWithCondition()
        {
            var tree = new UnaryDeterministicTree();
            var engine = new SearchEngine(CacheMode.ReuseCache, CacheKeyType.StateOnly)
            {
                SkipEvaluationForFirstNodeSingleNeighbor = false
            };
            engine.Search(tree.RootState, 10); // This should put all the states in the cache
            engine.CacheManager.Clear((s, _, l) => s != tree.State2);
            var result = engine.Search(tree.RootState, 10);
            Assert.AreNotEqual(result.StateSequence.Count, 2);
        }

        [TestMethod]
        public void ClearCache_CacheCleared()
        {
            var tree = new UnaryDeterministicTree();
            var engine = new SearchEngine(CacheMode.ReuseCache, CacheKeyType.StateOnly)
            {
                SkipEvaluationForFirstNodeSingleNeighbor = false
            };
            engine.Search(tree.RootState, 10); // This should put all the states in the cache
            engine.CacheManager.Clear();
            var result = engine.Search(tree.RootState, 10);
            Assert.AreNotEqual(result.StateSequence.Count, 1);
        }

        [TestMethod]
        [DataRow(CacheMode.NewCache)]
        [DataRow(CacheMode.NoCache)]
        [ExpectedException(typeof(MinMaxSearchException))]
        public void FillCache_CacheModeNotSetToResueCache_ThrowsException(CacheMode cacheMode)
        {
            var tree = new UnaryDeterministicTree();
            var engine = new SearchEngine(cacheMode, CacheKeyType.StateOnly)
            {
                SkipEvaluationForFirstNodeSingleNeighbor = false
            };
            engine.FillCache(tree.RootState, CancellationToken.None);
        }
    }
}
