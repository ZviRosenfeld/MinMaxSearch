using System.Collections.Generic;
using System.Linq;
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
        [DataRow(CacheKeyType.StateOnly)]
        [DataRow(CacheKeyType.StateAndDepth)]
        [DataRow(CacheKeyType.StateAndPassedThroughStates)]
        public void Search_EngineRemebersCachedStates(CacheKeyType cacheKeyType)
        {
            var tree = new UnaryDeterministicTree();
            var engine = new SearchEngine(CacheMode.ReuseCache, cacheKeyType)
            {
                SkipEvaluationForFirstNodeSingleNeighbor = false
            };
            engine.Search(tree.RootState, 10); // This should put all the states in the cache
            var result = engine.Search(tree.RootState, 10);
            Assert.AreEqual(1, result.StateSequence.Count);
            Assert.IsTrue(result.FullTreeSearchedOrPruned);
        }

        [TestMethod]
        [ExpectedException(typeof(MinMaxSearchException))]
        [DataRow(CacheKeyType.StateOnly)]
        [DataRow(CacheKeyType.StateAndDepth)]
        [DataRow(CacheKeyType.StateAndPassedThroughStates)]
        public void Constructor_NoCacheMode_ThrowException(CacheKeyType cacheKeyType)
        {
            new SearchEngine(CacheMode.NoCache, cacheKeyType);           
        }

        [TestMethod]
        public void Search_CacheKeyStateOnly_EngineRemebersCachedStates()
        {
            var tree = new UnaryDeterministicTree();
            var engine = new SearchEngine(CacheMode.ReuseCache, CacheKeyType.StateOnly)
            {
                SkipEvaluationForFirstNodeSingleNeighbor = false
            };
            engine.Search(tree.State2, 10); // This should put state's 2 and 3 in the cache
            var result = engine.Search(tree.RootState, 10);
            Assert.AreEqual(1, result.StateSequence.Count); // The tree should end at state2 - which is cached
            Assert.AreEqual(tree.State2, result.StateSequence.Last());
            Assert.IsTrue(result.FullTreeSearchedOrPruned);
        }

        [TestMethod]
        public void Search_CacheKeyStateAndDepth_EngineDoesntUseCachedStatesWithDiffrentDepth()
        {
            var tree = new UnaryDeterministicTree();
            var engine = new SearchEngine(CacheMode.ReuseCache, CacheKeyType.StateAndDepth)
            {
                SkipEvaluationForFirstNodeSingleNeighbor = false
            };
            engine.Search(tree.State2, 10); 
            var result = engine.Search(tree.RootState, 10);
            Assert.AreEqual(3, result.StateSequence.Count);  // The tree shouldn't end at state2 - since it's cached with depth 1, and in the second search it will have depth 2
            Assert.AreEqual(tree.EndState, result.StateSequence.Last());
        }

        [TestMethod]
        public void Search_CacheKeyStateAndDepth_EngineRemembersUsedCachedStatesWithSameDiffrentDepth()
        {
            var tree = new RepeatStateTree2();
            var engine = new SearchEngine(CacheMode.ReuseCache, CacheKeyType.StateAndDepth)
            {
                SkipEvaluationForFirstNodeSingleNeighbor = false
            };
            engine.Search(tree.State1, 10);
            var result = engine.Search(tree.State2, 10); // This should put state3 into the cache with depth 1
            Assert.AreEqual(1, result.StateSequence.Count);
            Assert.AreEqual(tree.State3, result.StateSequence.Last());
            Assert.IsTrue(result.FullTreeSearchedOrPruned);
        }

        [TestMethod]
        public void Search_CacheKeyStateAndPassedThroughStates_EngineDoesntUseCachedStatesWithDiffrentPassedThroughStates()
        {
            var tree = new RepeatStateTree2();
            var engine = new SearchEngine(CacheMode.ReuseCache, CacheKeyType.StateAndPassedThroughStates)
            {
                SkipEvaluationForFirstNodeSingleNeighbor = false
            };
            engine.Search(tree.State1, 10);
            var result = engine.Search(tree.State2, 10); // This should put state3 into the cache with depth 1
            Assert.AreEqual(2, result.StateSequence.Count);
            Assert.AreEqual(tree.EndState1, result.StateSequence.Last());
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
        [DataRow(CacheMode.NewCache)]
        [DataRow(CacheMode.ReuseCache)]
        public void SetCustomCache_UseCustomCache(CacheMode cacheMode)
        {
            var tree = new UnaryDeterministicTree();
            var customCache = A.Fake<ICacheManager>();
            A.CallTo(() => customCache.GetStateEvaluation(A<IState>._, A<int>._, A<IList<IState>>._)).Returns(new EvaluationRange(1));

            var engine = new SearchEngine(cacheMode, () => customCache)
            {
                SkipEvaluationForFirstNodeSingleNeighbor = false
            };
            engine.Search(tree.RootState, 10);
            A.CallTo(() => tree.EndState.GetNeighbors()).MustNotHaveHappened();
        }

        [TestMethod]
        [ExpectedException(typeof(MinMaxSearchException))]
        public void SetCustomCacheAndNoCacheMode_ThroWException()
        {
            new SearchEngine(CacheMode.NoCache, () => A.Fake<ICacheManager>());
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

        [TestMethod]
        [DataRow(CacheMode.NewCache)]
        [DataRow(CacheMode.ReuseCache)]
        public void Constructor_CacheModeSetToRightMode(CacheMode cacheMode)
        {
            var engine = new SearchEngine(cacheMode, CacheKeyType.StateOnly);
            Assert.AreEqual(cacheMode, engine.CacheMode);
        }

        [TestMethod]
        public void Constructor_NoCache_CacheModeSetToNoCache()
        {
            var engine = new SearchEngine();
            Assert.AreEqual(CacheMode.NoCache, engine.CacheMode);
        }

        [TestMethod]
        [DataRow(CacheMode.NewCache)]
        [DataRow(CacheMode.ReuseCache)]
        public void Constructor_CustomCache_CacheModeSetToRightMode(CacheMode cacheMode)
        {
            var engine = new SearchEngine(cacheMode, () => A.Fake<ICacheManager>());
            Assert.AreEqual(cacheMode, engine.CacheMode);
        }

        [TestMethod]
        public void Constructor_CustomCache_CacheKeyTypeSetToUnknown()
        {
            var engine = new SearchEngine(CacheMode.NewCache, () => A.Fake<ICacheManager>());
            Assert.AreEqual(CacheKeyType.Unknown, engine.CacheKeyType);
        }

        [TestMethod]
        [DataRow(CacheKeyType.StateOnly)]
        [DataRow(CacheKeyType.StateAndDepth)]
        [DataRow(CacheKeyType.StateAndPassedThroughStates)]
        public void Constructor_CacheKeyTypeSetToRightMode(CacheKeyType cacheKeyType)
        {
            var engine = new SearchEngine(CacheMode.NewCache, cacheKeyType);
            Assert.AreEqual(cacheKeyType, engine.CacheKeyType);
        }
    }
}
