using System.Collections.Generic;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch.Cache;
using MinMaxSearch.Pruners;
using MinMaxSearch.UnitTests.SampleTrees;

namespace MinMaxSearch.UnitTests.CacheTests
{
    /// <summary>
    /// This class tests that the right values are cached
    /// </summary>
    [TestClass]
    public class CachedValueTests
    {
        private const double MIN_EVALUATION = -10;
        private const double MAX_EVALUATION = 10;
        
        [TestMethod]
        [DataRow(true, true)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(false, false)]
        public void AlphaBetaPrunnedTree_CachRemebersRightValues(bool dieEarly, bool favorShortPaths)
        {
            var searchTree = new Tree1();

            searchTree.EndState1.SetEvaluationTo(5);
            searchTree.EndState2.SetEvaluationTo(2); // This should cuase a pruning

            var engine = GetReuseCacheEngine(dieEarly, favorShortPaths);
            engine.Search(searchTree.ManyChildrenState, 10);

            Assert.AreEqual(new EvaluationRange(int.MinValue, 2), GetEvaluation(engine, searchTree.ChildState2));
            Assert.AreEqual(new EvaluationRange(5, 5), GetEvaluation(engine, searchTree.ChildState1));
            Assert.AreEqual(new EvaluationRange(5, int.MaxValue), GetEvaluation(engine, searchTree.ManyChildrenState));
        }

        [TestMethod]
        [DataRow(true, true)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(false, false)]
        public void Max_PrunTree_CachRemebersRightValues(bool dieEarly, bool favorShortPaths)
        {
            var searchTree = new Tree1();

            searchTree.EndState1.SetEvaluationTo(5);
            searchTree.EndState2.SetEvaluationTo(6);
            var myPrunner = A.Fake<IPruner>();
            A.CallTo(() => myPrunner.ShouldPrune(A<IState>._, A<int>._, A<List<IState>>._))
                .ReturnsLazily((IState s, int d, List<IState> l) => s == searchTree.ChildState2);

            var engine = GetReuseCacheEngine(dieEarly, favorShortPaths).AddPruner(myPrunner);
            engine.Search(searchTree.ManyChildrenState, 10);

            Assert.AreEqual(new EvaluationRange(5, 5), GetEvaluation(engine, searchTree.ChildState1));
            Assert.AreEqual(new EvaluationRange(5, int.MaxValue), GetEvaluation(engine, searchTree.ManyChildrenState));
        }

        [TestMethod]
        [DataRow(true, true)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(false, false)]
        public void PrunTreeContainingMaxWin_CachRemebersRightValues(bool dieEarly, bool favorShortPaths)
        {
            var searchTree = new Tree1();
            searchTree.EndState1.SetEvaluationTo(MAX_EVALUATION);

            var myPrunner = A.Fake<IPruner>();
            A.CallTo(() => myPrunner.ShouldPrune(A<IState>._, A<int>._, A<List<IState>>._))
                .ReturnsLazily((IState s, int d, List<IState> l) => s == searchTree.ChildState2);

            var engine = GetReuseCacheEngine(dieEarly, favorShortPaths).AddPruner(myPrunner);
            engine.Search(searchTree.ManyChildrenState, 10);

            Assert.AreEqual(new EvaluationRange(MAX_EVALUATION, MAX_EVALUATION), GetEvaluation(engine, searchTree.ChildState1));
            Assert.AreEqual(new EvaluationRange(MAX_EVALUATION, MAX_EVALUATION), GetEvaluation(engine, searchTree.ManyChildrenState));
        }

        [TestMethod]
        [DataRow(true, true)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(false, false)]
        public void Min_PrunTree_CachRemebersRightValues(bool dieEarly, bool favorShortPaths)
        {
            var searchTree = new Tree1();

            searchTree.EndState2.SetEvaluationTo(5);
            searchTree.EndState3.SetEvaluationTo(6);
            var myPrunner = A.Fake<IPruner>();
            A.CallTo(() => myPrunner.ShouldPrune(A<IState>._, A<int>._, A<List<IState>>._))
                .ReturnsLazily((IState s, int d, List<IState> l) => s == searchTree.EndState3);

            var engine = GetReuseCacheEngine(dieEarly, favorShortPaths).AddPruner(myPrunner);
            engine.Search(searchTree.ChildState2, 10);

            Assert.AreEqual(new EvaluationRange(5, 5), GetEvaluation(engine, searchTree.EndState2));
            Assert.AreEqual(new EvaluationRange(int.MinValue, 5), GetEvaluation(engine, searchTree.ChildState2));
        }

        [TestMethod]
        [DataRow(true, true)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(false, false)]
        public void PrunTreeContainingMinWin_CachRemebersRightValues(bool dieEarly, bool favorShortPaths)
        {
            var searchTree = new Tree1();

            searchTree.EndState2.SetEvaluationTo(MIN_EVALUATION);
            searchTree.EndState3.SetEvaluationTo(6);
            var myPrunner = A.Fake<IPruner>();
            A.CallTo(() => myPrunner.ShouldPrune(A<IState>._, A<int>._, A<List<IState>>._))
                .ReturnsLazily((IState s, int d, List<IState> l) => s == searchTree.EndState3);

            var engine = GetReuseCacheEngine(dieEarly, favorShortPaths).AddPruner(myPrunner);
            engine.Search(searchTree.ChildState2, 10);

            Assert.AreEqual(new EvaluationRange(MIN_EVALUATION, MIN_EVALUATION), GetEvaluation(engine, searchTree.EndState2));
            Assert.AreEqual(new EvaluationRange(MIN_EVALUATION, MIN_EVALUATION), GetEvaluation(engine, searchTree.ChildState2));
        }

        [TestMethod]
        [DataRow(true, true)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(false, false)]
        public void PrunTreeContainingWrongWin_CachRemebersRightValues(bool dieEarly, bool favorShortPaths)
        {
            var searchTree = new Tree1();

            searchTree.EndState2.SetEvaluationTo(MAX_EVALUATION);
            searchTree.EndState3.SetEvaluationTo(6);
            var myPrunner = A.Fake<IPruner>();
            A.CallTo(() => myPrunner.ShouldPrune(A<IState>._, A<int>._, A<List<IState>>._))
                .ReturnsLazily((IState s, int d, List<IState> l) => s == searchTree.EndState3);

            var engine = GetReuseCacheEngine(dieEarly, favorShortPaths).AddPruner(myPrunner);
            engine.Search(searchTree.ChildState2, 10);

            Assert.AreEqual(new EvaluationRange(MAX_EVALUATION, MAX_EVALUATION), GetEvaluation(engine, searchTree.EndState2));
            Assert.AreEqual(new EvaluationRange(int.MinValue, 6), GetEvaluation(engine, searchTree.ChildState2));
        }

        [TestMethod]
        [DataRow(true, true)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(false, false)]
        public void ChildrenContainWinForMax_CachRemebersRightValues(bool dieEarly, bool favorShortPaths)
        {
            var searchTree = new Tree1();
            searchTree.EndState1.SetEvaluationTo(MAX_EVALUATION);

            var engine = GetReuseCacheEngine(dieEarly, favorShortPaths);
            engine.Search(searchTree.ManyChildrenState, 10);

            Assert.AreEqual(new EvaluationRange(MAX_EVALUATION, MAX_EVALUATION), GetEvaluation(engine, searchTree.ChildState1));
            Assert.AreEqual(new EvaluationRange(MAX_EVALUATION, MAX_EVALUATION), GetEvaluation(engine, searchTree.ManyChildrenState));
        }

        [TestMethod]
        [DataRow(true, true)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(false, false)]
        public void ChildrenContainWinForMin_CachRemebersRightValues(bool dieEarly, bool favorShortPaths)
        {
            var searchTree = new Tree1(Player.Min);
            searchTree.EndState1.SetEvaluationTo(MIN_EVALUATION);

            var engine = GetReuseCacheEngine(dieEarly, favorShortPaths);
            engine.Search(searchTree.ManyChildrenState, 10);

            Assert.AreEqual(new EvaluationRange(MIN_EVALUATION, MIN_EVALUATION), GetEvaluation(engine, searchTree.ChildState1));
            Assert.AreEqual(new EvaluationRange(MIN_EVALUATION, MIN_EVALUATION), GetEvaluation(engine, searchTree.ManyChildrenState));
        }

        [TestMethod]
        [DataRow(true, true)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(false, false)]
        public void RepeatedStateInStateDefinesDepthGame_CacheRepeatedValue(bool dieEarly, bool favorShortPaths)
        {
            var searchTree = new RepeatStateTree();
            searchTree.ChildState2.SetEvaluationTo(3, 4);

            var engine = new SearchEngine()
            {
                CacheMode = CacheMode.NewCache,
                FavorShortPaths = favorShortPaths,
                DieEarly = dieEarly,
                StateDefinesDepth = true,
                ParallelismMode = ParallelismMode.NonParallelism
            };
            var result = engine.Search(searchTree.StartState, 2);

            Assert.AreEqual(3, result.Evaluation); // Check that we read evaluation for Child1 from the cache the second time
        }
        
        [TestMethod]
        [DataRow(true, false, true)]
        [DataRow(false, true, true)]
        [DataRow(false, false, false)]
        public void RepeatedStateInStateDefinesDepthGame_DontCacheRepeatedValue(bool useUnstalbeStateMethod, bool reuseCache, bool stateDefinesDepth)
        {
            var searchTree = new RepeatStateTree();
            searchTree.ChildState2.SetEvaluationTo(3, 4);

            var engine = new SearchEngine()
            {
                CacheMode = reuseCache ? CacheMode.ReuseCache : CacheMode.NewCache,
                StateDefinesDepth = stateDefinesDepth,
                ParallelismMode = ParallelismMode.NonParallelism
            };
            if (useUnstalbeStateMethod)
                engine.IsUnstableState = (s, d, l) => false;
            
            var result = engine.Search(searchTree.StartState, 2);

            Assert.AreEqual(4, result.Evaluation); // Check that we didn't read evaluation for Child1 from the cache the second time
        }

        [TestMethod]
        [DataRow(true, true)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(false, false)]
        public void RepeatedStateInStateDefinesDepthGame_StatesPrunnedByAlphaBeta_CacheDonstRepeatedValue(bool dieEarly, bool favorShortPaths)
        {
            var searchTree = new RepeatStateTree();
            searchTree.EndState1.SetEvaluationTo(5);
            searchTree.EndState2.SetEvaluationTo(4, 7);
            searchTree.EndState3.SetEvaluationTo(6);

            var engine = new SearchEngine()
            {
                CacheMode = CacheMode.NewCache,
                FavorShortPaths = favorShortPaths,
                DieEarly = dieEarly,
                StateDefinesDepth = true,
                ParallelismMode = ParallelismMode.NonParallelism
            };

            var result = engine.Search(searchTree.StartState, 4);

            Assert.AreEqual(6, result.Evaluation); // Check that we didn't read evaluation for endState2 from the cache the second time
        }

        [TestMethod]
        [DataRow(true, true)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(false, false)]
        public void RepeatedStateInStateDefinesDepthGame_StatesPrunnedByPrunner_CacheDonstRepeatedValue(bool dieEarly, bool favorShortPaths)
        {
            var searchTree = new RepeatStateTree();
            searchTree.ChildState4.SetEvaluationTo(3, 4);

            var myPrunner = A.Fake<IPruner>();
            A.CallTo(() => myPrunner.ShouldPrune(A<IState>._, A<int>._, A<List<IState>>._))
                .ReturnsLazily((IState s, int d, List<IState> l) => s == searchTree.ChildState5);
            var engine = new SearchEngine()
            {
                CacheMode = CacheMode.NewCache,
                FavorShortPaths = favorShortPaths,
                DieEarly = dieEarly,
                StateDefinesDepth = true,
                ParallelismMode = ParallelismMode.NonParallelism
            }.AddPruner(myPrunner);

            var result = engine.Search(searchTree.StartState, 3);

            Assert.AreEqual(4, result.Evaluation); // Check that we didn't read evaluation for childState5 from the cache the second time
        }

        [TestMethod]
        public void DontCachePartualValues()
        {
            var searchTree = new Tree1();
            var extendedState = A.Fake<IDeterministicState>();
            extendedState.SetEvaluationTo(MAX_EVALUATION);
            searchTree.EndState1.SetNeigbor(extendedState);
            extendedState.SetAsEndState();
            A.CallTo(() => extendedState.Turn).Returns(Player.Min);

            var engine = GetReuseCacheEngine(true, true);
            var result = engine.Search(searchTree.ManyChildrenState, 10);

            A.CallTo(() => searchTree.EndState2.Evaluate(A<int>._, A<List<IState>>._)).MustHaveHappened();
            Assert.AreEqual(null, GetEvaluation(engine, searchTree.ChildState2));
            Assert.IsTrue(result.IsSearchCompleted);
        }

        private static SearchEngine GetReuseCacheEngine(bool dieEarly, bool favorShortPaths) =>
            new SearchEngine()
            {
                CacheMode = CacheMode.ReuseCache,
                ParallelismMode = ParallelismMode.NonParallelism,
                MaxScore = MAX_EVALUATION,
                MinScore = MIN_EVALUATION,
                DieEarly = dieEarly,
                FavorShortPaths = favorShortPaths,
                SkipEvaluationForFirstNodeSingleNeighbor = false
            };

        private EvaluationRange GetEvaluation(SearchEngine engine, IState state) =>
            ((CacheManager)engine.CacheManager)[state];
    }
}
