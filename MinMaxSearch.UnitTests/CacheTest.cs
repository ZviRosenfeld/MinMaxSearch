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
        // This class contains 2 search trees:
        // stat1 -> sate2 -> state3 -> endState1
        //
        // And:
        //                    manyChildrenState
        // childState1                    childState2          
        //  endState1            endState2            endState3

        private readonly IDeterministicState state1 = A.Fake<IDeterministicState>();
        private readonly IDeterministicState state2 = A.Fake<IDeterministicState>();
        private readonly IDeterministicState state3 = A.Fake<IDeterministicState>();
        private readonly IDeterministicState endState1 = A.Fake<IDeterministicState>();
        private readonly IDeterministicState endState2 = A.Fake<IDeterministicState>();
        private readonly IDeterministicState endState3 = A.Fake<IDeterministicState>();
        private readonly IDeterministicState manyChildrenState = A.Fake<IDeterministicState>();
        private readonly IDeterministicState childState1 = A.Fake<IDeterministicState>();
        private readonly IDeterministicState childState2 = A.Fake<IDeterministicState>();
        
        private const double MIN_EVALUATION = -10;
        private const double MAX_EVALUATION = 10;

        [TestInitialize]
        public void TestInitialize()
        {
            A.CallTo(() => state1.ToString()).Returns("State1");
            A.CallTo(() => state2.ToString()).Returns("State2");
            A.CallTo(() => state3.ToString()).Returns("State3");
            A.CallTo(() => endState1.ToString()).Returns("EndState1");
            A.CallTo(() => endState2.ToString()).Returns("EndState2");
            A.CallTo(() => endState3.ToString()).Returns("EndState3");
            A.CallTo(() => manyChildrenState.ToString()).Returns("ManyChildrenState");
            A.CallTo(() => childState1.ToString()).Returns("ChildState1");
            A.CallTo(() => childState2.ToString()).Returns("ChildState2");
            
            A.CallTo(() => state1.Turn).Returns(Player.Max);
            A.CallTo(() => state2.Turn).Returns(Player.Max);
            A.CallTo(() => state3.Turn).Returns(Player.Max);
            A.CallTo(() => endState1.Turn).Returns(Player.Max);
            A.CallTo(() => endState2.Turn).Returns(Player.Max);
            A.CallTo(() => endState3.Turn).Returns(Player.Max);
            A.CallTo(() => manyChildrenState.Turn).Returns(Player.Max);
            A.CallTo(() => childState1.Turn).Returns(Player.Min);
            A.CallTo(() => childState2.Turn).Returns(Player.Min);

            state1.SetNeigbor(state2);
            state2.SetNeigbor(state3);
            state3.SetNeigbor(endState1);
            manyChildrenState.SetNeigbors(childState1, childState2);
            childState1.SetNeigbor(endState1);
            childState2.SetNeigbors(endState2, endState3);
            endState1.SetAsEndState();
            endState2.SetAsEndState();
            endState3.SetAsEndState();
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
            Assert.IsTrue(result.FullTreeSearched);
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
            Assert.IsTrue(result.FullTreeSearched);
        }

        [TestMethod]
        public void SetCustomCache_UseCustomCache()
        {
            var customCache = A.Fake<ICacheManager>();
            A.CallTo(() => customCache.GetStateEvaluation(A<IState>._)).Returns(new EvaluationRange(1));

            var engine = new SearchEngine()
            {
                CacheMode = CacheMode.ReuseCache,
                CacheManager = customCache
            };
            engine.Search(state1, 10);
            A.CallTo(() => endState1.GetNeighbors()).MustNotHaveHappened();
        }

        [TestMethod]
        public void ClearCacheWithCondition()
        {
            var engine = new SearchEngine()
            {
                CacheMode = CacheMode.ReuseCache
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
                CacheMode = CacheMode.ReuseCache
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
                CacheMode = cacheMode
            };
            engine.FillCache(state1, CancellationToken.None);
        }

        [TestMethod]
        public void AlphaBetaPrunnedTree_CachRemebersRightValues()
        {
            endState1.SetEvaluationTo(5);
            endState2.SetEvaluationTo(2); // This should cuase a pruning

            var engine = new SearchEngine()
            {
                CacheMode = CacheMode.ReuseCache,
                ParallelismMode = ParallelismMode.NonParallelism
            };
            engine.Search(manyChildrenState, 10);
            
            Assert.AreEqual(new EvaluationRange(int.MinValue, 2), GetEvaluation(engine, childState2));
            Assert.AreEqual(new EvaluationRange(5, 5), GetEvaluation(engine, childState1));
            Assert.AreEqual(new EvaluationRange(5, int.MaxValue), GetEvaluation(engine, manyChildrenState));
        }

        [TestMethod]
        [DataRow(true, true)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(false, false)]
        public void ChildrenContainWinForMax_CachRemebersRightValues(bool dieEarly, bool favorShortPaths)
        {
            endState1.SetEvaluationTo(MAX_EVALUATION);

            var engine = new SearchEngine()
            {
                CacheMode = CacheMode.ReuseCache,
                ParallelismMode = ParallelismMode.NonParallelism,
                MaxScore = MAX_EVALUATION,
                MinScore = MIN_EVALUATION,
                DieEarly = dieEarly,
                FavorShortPaths = favorShortPaths
            };
            engine.Search(manyChildrenState, 10);

            Assert.AreEqual(new EvaluationRange(MAX_EVALUATION, MAX_EVALUATION), GetEvaluation(engine, childState1));
            Assert.AreEqual(new EvaluationRange(MAX_EVALUATION, MAX_EVALUATION), GetEvaluation(engine, manyChildrenState));
        }
        
        [TestMethod]
        [DataRow(true, true)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(false, false)]
        public void ChildrenContainWinForMin_CachRemebersRightValues(bool dieEarly, bool favorShortPaths)
        {
            endState1.SetEvaluationTo(MIN_EVALUATION);

            // Reverse the tree, so that we can check a win for min
            A.CallTo(() => endState1.Turn).Returns(Player.Min);
            A.CallTo(() => endState2.Turn).Returns(Player.Min);
            A.CallTo(() => endState3.Turn).Returns(Player.Min);
            A.CallTo(() => manyChildrenState.Turn).Returns(Player.Min);
            A.CallTo(() => childState1.Turn).Returns(Player.Max);
            A.CallTo(() => childState2.Turn).Returns(Player.Max);

            var engine = new SearchEngine()
            {
                CacheMode = CacheMode.ReuseCache,
                ParallelismMode = ParallelismMode.NonParallelism,
                MaxScore = MAX_EVALUATION,
                MinScore = MIN_EVALUATION,
                DieEarly = dieEarly,
                FavorShortPaths = favorShortPaths
            };
            engine.Search(manyChildrenState, 10);

            Assert.AreEqual(new EvaluationRange(MIN_EVALUATION, MIN_EVALUATION), GetEvaluation(engine, childState1));
            Assert.AreEqual(new EvaluationRange(MIN_EVALUATION, MIN_EVALUATION), GetEvaluation(engine, manyChildrenState));
        }

        private EvaluationRange GetEvaluation(SearchEngine engine, IState state) =>
            ((CacheManager) engine.CacheManager)[state];
    }
}
