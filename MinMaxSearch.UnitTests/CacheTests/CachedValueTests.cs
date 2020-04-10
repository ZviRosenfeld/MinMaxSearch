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
        public void AlphaBetaPrunedTree_CacheRemembersRightValues(bool dieEarly, bool favorShortPaths)
        {
            var searchTree = new DeterministicTree();

            searchTree.EndState1.SetEvaluationTo(5);
            searchTree.EndState2.SetEvaluationTo(2); // This should cause a pruning

            var engine = GetReuseCacheEngine(dieEarly, favorShortPaths);
            engine.Search(searchTree.RootState, 10);

            Assert.AreEqual(new EvaluationRange(int.MinValue, 2), GetEvaluation(engine, searchTree.ChildState2));
            Assert.AreEqual(new EvaluationRange(5, 5), GetEvaluation(engine, searchTree.ChildState1));
            Assert.AreEqual(new EvaluationRange(5, int.MaxValue), GetEvaluation(engine, searchTree.RootState));
        }

        [TestMethod]
        [DataRow(true, true)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(false, false)]
        public void Max_PrunTree_CachRemebersRightValues(bool dieEarly, bool favorShortPaths)
        {
            var searchTree = new DeterministicTree();

            searchTree.EndState1.SetEvaluationTo(5);
            searchTree.EndState2.SetEvaluationTo(6);
            var myPrunner = A.Fake<IPruner>();
            A.CallTo(() => myPrunner.ShouldPrune(A<IState>._, A<int>._, A<List<IState>>._))
                .ReturnsLazily((IState s, int d, List<IState> l) => s == searchTree.ChildState2);

            var engine = GetReuseCacheEngine(dieEarly, favorShortPaths).AddPruner(myPrunner);
            engine.Search(searchTree.RootState, 10);

            Assert.AreEqual(new EvaluationRange(5, 5), GetEvaluation(engine, searchTree.ChildState1));
            Assert.AreEqual(new EvaluationRange(5, int.MaxValue), GetEvaluation(engine, searchTree.RootState));
        }

        [TestMethod]
        [DataRow(true, true)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(false, false)]
        public void PrunTreeContainingMaxWin_CachRemebersRightValues(bool dieEarly, bool favorShortPaths)
        {
            var searchTree = new DeterministicTree();
            searchTree.EndState1.SetEvaluationTo(MAX_EVALUATION);

            var myPrunner = A.Fake<IPruner>();
            A.CallTo(() => myPrunner.ShouldPrune(A<IState>._, A<int>._, A<List<IState>>._))
                .ReturnsLazily((IState s, int d, List<IState> l) => s == searchTree.ChildState2);

            var engine = GetReuseCacheEngine(dieEarly, favorShortPaths).AddPruner(myPrunner);
            engine.Search(searchTree.RootState, 10);

            Assert.AreEqual(new EvaluationRange(MAX_EVALUATION, MAX_EVALUATION), GetEvaluation(engine, searchTree.ChildState1));
            Assert.AreEqual(new EvaluationRange(MAX_EVALUATION, MAX_EVALUATION), GetEvaluation(engine, searchTree.RootState));
        }

        [TestMethod]
        [DataRow(true, true)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(false, false)]
        public void Min_PrunTree_CachRemebersRightValues(bool dieEarly, bool favorShortPaths)
        {
            var searchTree = new DeterministicTree();

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
            var searchTree = new DeterministicTree();

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
            var searchTree = new DeterministicTree();

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
            var searchTree = new DeterministicTree();
            searchTree.EndState1.SetEvaluationTo(MAX_EVALUATION);

            var engine = GetReuseCacheEngine(dieEarly, favorShortPaths);
            engine.Search(searchTree.RootState, 10);

            Assert.AreEqual(new EvaluationRange(MAX_EVALUATION, MAX_EVALUATION), GetEvaluation(engine, searchTree.ChildState1));
            Assert.AreEqual(new EvaluationRange(MAX_EVALUATION, MAX_EVALUATION), GetEvaluation(engine, searchTree.RootState));
        }

        [TestMethod]
        [DataRow(true, true)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(false, false)]
        public void ChildrenContainWinForMin_CachRemebersRightValues(bool dieEarly, bool favorShortPaths)
        {
            var searchTree = new DeterministicTree(Player.Min);
            searchTree.EndState1.SetEvaluationTo(MIN_EVALUATION);

            var engine = GetReuseCacheEngine(dieEarly, favorShortPaths);
            engine.Search(searchTree.RootState, 10);

            Assert.AreEqual(new EvaluationRange(MIN_EVALUATION, MIN_EVALUATION), GetEvaluation(engine, searchTree.ChildState1));
            Assert.AreEqual(new EvaluationRange(MIN_EVALUATION, MIN_EVALUATION), GetEvaluation(engine, searchTree.RootState));
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

            var engine = new SearchEngine(CacheMode.NewCache)
            {
                FavorShortPaths = favorShortPaths,
                DieEarly = dieEarly,
                StateDefinesDepth = true,
                ParallelismMode = ParallelismMode.NonParallelism
            };
            var result = engine.Search(searchTree.RootState, 2);

            Assert.AreEqual(3, result.Evaluation); // Check that we read the evaluation for Child1 from the cache the second time
        }
        
        [TestMethod]
        [DataRow(true, false, true)]
        [DataRow(false, true, true)]
        [DataRow(false, false, false)]
        public void RepeatedStateInStateDefinesDepthGame_DontCacheRepeatedValue(bool useUnstableStateMethod, bool reuseCache, bool stateDefinesDepth)
        {
            var searchTree = new RepeatStateTree();
            searchTree.ChildState2.SetEvaluationTo(3, 4);

            var engine = new SearchEngine(reuseCache ? CacheMode.ReuseCache : CacheMode.NewCache)
            {
                StateDefinesDepth = stateDefinesDepth,
                ParallelismMode = ParallelismMode.NonParallelism
            };
            if (useUnstableStateMethod)
                engine.IsUnstableState = (s, d, l) => false;
            
            var result = engine.Search(searchTree.RootState, 2);

            Assert.AreEqual(4, result.Evaluation); // Check that we didn't read the evaluation for Child1 from the cache the second time
        }

        [TestMethod]
        [DataRow(true, true)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(false, false)]
        public void RepeatedStateInStateDefinesDepthGame_StatesPrunedByAlphaBeta_CacheDoesntRepeatedValue(bool dieEarly, bool favorShortPaths)
        {
            var searchTree = new RepeatStateTree();
            searchTree.EndState1.SetEvaluationTo(5);
            searchTree.EndState2.SetEvaluationTo(4, 7);
            searchTree.EndState3.SetEvaluationTo(6);

            var engine = new SearchEngine(CacheMode.NewCache)
            {
                FavorShortPaths = favorShortPaths,
                DieEarly = dieEarly,
                StateDefinesDepth = true,
                ParallelismMode = ParallelismMode.NonParallelism
            };

            var result = engine.Search(searchTree.RootState, 4);

            Assert.AreEqual(6, result.Evaluation); // Check that we didn't read the evaluation for endState2 from the cache the second time
        }

        [TestMethod]
        [DataRow(true, true)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(false, false)]
        public void RepeatedStateInStateDefinesDepthGame_StatesPrunedByPruner_CacheDoesntRepeatedValue(bool dieEarly, bool favorShortPaths)
        {
            var searchTree = new RepeatStateTree();
            searchTree.ChildState4.SetEvaluationTo(3, 4);

            var myPruner = A.Fake<IPruner>();
            A.CallTo(() => myPruner.ShouldPrune(A<IState>._, A<int>._, A<List<IState>>._))
                .ReturnsLazily((IState s, int d, List<IState> l) => s == searchTree.ChildState5);
            var engine = new SearchEngine(CacheMode.NewCache)
            {
                FavorShortPaths = favorShortPaths,
                DieEarly = dieEarly,
                StateDefinesDepth = true,
                ParallelismMode = ParallelismMode.NonParallelism
            }.AddPruner(myPruner);

            var result = engine.Search(searchTree.RootState, 3);

            Assert.AreEqual(4, result.Evaluation); // Check that we didn't read the evaluation for childState5 from the cache the second time
        }

        [TestMethod]
        public void DontCachePartualValues()
        {
            var searchTree = new DeterministicTree();
            var extendedState = A.Fake<IDeterministicState>();
            extendedState.SetEvaluationTo(MAX_EVALUATION);
            searchTree.EndState1.SetNeigbor(extendedState);
            extendedState.SetAsEndState();
            A.CallTo(() => extendedState.Turn).Returns(Player.Min);

            var engine = GetReuseCacheEngine(true, true);
            var result = engine.Search(searchTree.RootState, 10);

            A.CallTo(() => searchTree.EndState2.Evaluate(A<int>._, A<List<IState>>._)).MustHaveHappened();
            Assert.AreEqual(null, GetEvaluation(engine, searchTree.ChildState2));
            Assert.IsTrue(result.IsSearchCompleted);
        }

        private static SearchEngine GetReuseCacheEngine(bool dieEarly, bool favorShortPaths) =>
            new SearchEngine(CacheMode.ReuseCache)
            {
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
