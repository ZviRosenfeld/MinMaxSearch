using System.Collections.Generic;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch.Cache;
using MinMaxSearch.Pruners;

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
            var searchTree = new SampleTree();

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
            var searchTree = new SampleTree();

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
            var searchTree = new SampleTree();
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
            var searchTree = new SampleTree();

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
            var searchTree = new SampleTree();

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
            var searchTree = new SampleTree();

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
            var searchTree = new SampleTree();
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
            var searchTree = new SampleTree(Player.Min);
            searchTree.EndState1.SetEvaluationTo(MIN_EVALUATION);

            var engine = GetReuseCacheEngine(dieEarly, favorShortPaths);
            engine.Search(searchTree.ManyChildrenState, 10);

            Assert.AreEqual(new EvaluationRange(MIN_EVALUATION, MIN_EVALUATION), GetEvaluation(engine, searchTree.ChildState1));
            Assert.AreEqual(new EvaluationRange(MIN_EVALUATION, MIN_EVALUATION), GetEvaluation(engine, searchTree.ManyChildrenState));
        }

        private static SearchEngine GetReuseCacheEngine(bool dieEarly, bool favorShortPaths) =>
            new SearchEngine()
            {
                CacheMode = CacheMode.ReuseCache,
                ParallelismMode = ParallelismMode.NonParallelism,
                MaxScore = MAX_EVALUATION,
                MinScore = MIN_EVALUATION,
                DieEarly = dieEarly,
                FavorShortPaths = favorShortPaths
            };

        private EvaluationRange GetEvaluation(SearchEngine engine, IState state) =>
            ((CacheManager)engine.CacheManager)[state];
    }
}
