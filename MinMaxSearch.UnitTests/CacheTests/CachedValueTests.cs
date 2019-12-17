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
        // This class uses search tree:
        //              manyChildrenState
        // childState1                    childState2          
        //  endState1            endState2            endState3

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
            A.CallTo(() => endState1.ToString()).Returns("EndState1");
            A.CallTo(() => endState2.ToString()).Returns("EndState2");
            A.CallTo(() => endState3.ToString()).Returns("EndState3");
            A.CallTo(() => manyChildrenState.ToString()).Returns("ManyChildrenState");
            A.CallTo(() => childState1.ToString()).Returns("ChildState1");
            A.CallTo(() => childState2.ToString()).Returns("ChildState2");

            A.CallTo(() => endState1.Turn).Returns(Player.Max);
            A.CallTo(() => endState2.Turn).Returns(Player.Max);
            A.CallTo(() => endState3.Turn).Returns(Player.Max);
            A.CallTo(() => manyChildrenState.Turn).Returns(Player.Max);
            A.CallTo(() => childState1.Turn).Returns(Player.Min);
            A.CallTo(() => childState2.Turn).Returns(Player.Min);

            manyChildrenState.SetNeigbors(childState1, childState2);
            childState1.SetNeigbor(endState1);
            childState2.SetNeigbors(endState2, endState3);
            endState1.SetAsEndState();
            endState2.SetAsEndState();
            endState3.SetAsEndState();
        }
        
        [TestMethod]
        [DataRow(true, true)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(false, false)]
        public void AlphaBetaPrunnedTree_CachRemebersRightValues(bool dieEarly, bool favorShortPaths)
        {
            endState1.SetEvaluationTo(5);
            endState2.SetEvaluationTo(2); // This should cuase a pruning

            var engine = GetReuseCacheEngine(dieEarly, favorShortPaths);
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
        public void Max_PrunTree_CachRemebersRightValues(bool dieEarly, bool favorShortPaths)
        {
            endState1.SetEvaluationTo(5);
            endState2.SetEvaluationTo(6);
            var myPrunner = A.Fake<IPruner>();
            A.CallTo(() => myPrunner.ShouldPrune(A<IState>._, A<int>._, A<List<IState>>._))
                .ReturnsLazily((IState s, int d, List<IState> l) => s == childState2);

            var engine = GetReuseCacheEngine(dieEarly, favorShortPaths).AddPruner(myPrunner);
            engine.Search(manyChildrenState, 10);

            Assert.AreEqual(new EvaluationRange(5, 5), GetEvaluation(engine, childState1));
            Assert.AreEqual(new EvaluationRange(5, int.MaxValue), GetEvaluation(engine, manyChildrenState));
        }

        [TestMethod]
        [DataRow(true, true)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(false, false)]
        public void PrunTreeContainingMaxWin_CachRemebersRightValues(bool dieEarly, bool favorShortPaths)
        {
            endState1.SetEvaluationTo(MAX_EVALUATION);

            var myPrunner = A.Fake<IPruner>();
            A.CallTo(() => myPrunner.ShouldPrune(A<IState>._, A<int>._, A<List<IState>>._))
                .ReturnsLazily((IState s, int d, List<IState> l) => s == childState2);

            var engine = GetReuseCacheEngine(dieEarly, favorShortPaths).AddPruner(myPrunner);
            engine.Search(manyChildrenState, 10);

            Assert.AreEqual(new EvaluationRange(MAX_EVALUATION, MAX_EVALUATION), GetEvaluation(engine, childState1));
            Assert.AreEqual(new EvaluationRange(MAX_EVALUATION, MAX_EVALUATION), GetEvaluation(engine, manyChildrenState));
        }

        [TestMethod]
        [DataRow(true, true)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(false, false)]
        public void Min_PrunTree_CachRemebersRightValues(bool dieEarly, bool favorShortPaths)
        {
            endState2.SetEvaluationTo(5);
            endState3.SetEvaluationTo(6);
            var myPrunner = A.Fake<IPruner>();
            A.CallTo(() => myPrunner.ShouldPrune(A<IState>._, A<int>._, A<List<IState>>._))
                .ReturnsLazily((IState s, int d, List<IState> l) => s == endState3);

            var engine = GetReuseCacheEngine(dieEarly, favorShortPaths).AddPruner(myPrunner);
            engine.Search(childState2, 10);

            Assert.AreEqual(new EvaluationRange(5, 5), GetEvaluation(engine, endState2));
            Assert.AreEqual(new EvaluationRange(int.MinValue, 5), GetEvaluation(engine, childState2));
        }

        [TestMethod]
        [DataRow(true, true)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(false, false)]
        public void PrunTreeContainingMinWin_CachRemebersRightValues(bool dieEarly, bool favorShortPaths)
        {
            endState2.SetEvaluationTo(MIN_EVALUATION);
            endState3.SetEvaluationTo(6);
            var myPrunner = A.Fake<IPruner>();
            A.CallTo(() => myPrunner.ShouldPrune(A<IState>._, A<int>._, A<List<IState>>._))
                .ReturnsLazily((IState s, int d, List<IState> l) => s == endState3);

            var engine = GetReuseCacheEngine(dieEarly, favorShortPaths).AddPruner(myPrunner);
            engine.Search(childState2, 10);

            Assert.AreEqual(new EvaluationRange(MIN_EVALUATION, MIN_EVALUATION), GetEvaluation(engine, endState2));
            Assert.AreEqual(new EvaluationRange(MIN_EVALUATION, MIN_EVALUATION), GetEvaluation(engine, childState2));
        }

        [TestMethod]
        [DataRow(true, true)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(false, false)]
        public void PrunTreeContainingWrongWin_CachRemebersRightValues(bool dieEarly, bool favorShortPaths)
        {
            endState2.SetEvaluationTo(MAX_EVALUATION);
            endState3.SetEvaluationTo(6);
            var myPrunner = A.Fake<IPruner>();
            A.CallTo(() => myPrunner.ShouldPrune(A<IState>._, A<int>._, A<List<IState>>._))
                .ReturnsLazily((IState s, int d, List<IState> l) => s == endState3);

            var engine = GetReuseCacheEngine(dieEarly, favorShortPaths).AddPruner(myPrunner);
            engine.Search(childState2, 10);

            Assert.AreEqual(new EvaluationRange(MAX_EVALUATION, MAX_EVALUATION), GetEvaluation(engine, endState2));
            Assert.AreEqual(new EvaluationRange(int.MinValue, 6), GetEvaluation(engine, childState2));
        }

        [TestMethod]
        [DataRow(true, true)]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(false, false)]
        public void ChildrenContainWinForMax_CachRemebersRightValues(bool dieEarly, bool favorShortPaths)
        {
            endState1.SetEvaluationTo(MAX_EVALUATION);

            var engine = GetReuseCacheEngine(dieEarly, favorShortPaths);
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

            var engine = GetReuseCacheEngine(dieEarly, favorShortPaths);
            engine.Search(manyChildrenState, 10);

            Assert.AreEqual(new EvaluationRange(MIN_EVALUATION, MIN_EVALUATION), GetEvaluation(engine, childState1));
            Assert.AreEqual(new EvaluationRange(MIN_EVALUATION, MIN_EVALUATION), GetEvaluation(engine, manyChildrenState));
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
