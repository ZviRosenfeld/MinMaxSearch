using System;
using System.Collections.Generic;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MinMaxSearch.UnitTests
{
    [TestClass]
    public class ProbabilisticSearchTests
    {
        private readonly IDeterministicState startState = A.Fake<IDeterministicState>();
        private readonly IProbabilisticState probabilisticState1 = A.Fake<IProbabilisticState>();
        private readonly IProbabilisticState probabilisticState2 = A.Fake<IProbabilisticState>();

        private readonly IProbabilisticState evaluation2State = A.Fake<IProbabilisticState>();
        private readonly IProbabilisticState evaluationNagitive2State = A.Fake<IProbabilisticState>();

        [TestInitialize]
        public void TestInitialize()
        {
            A.CallTo(() => startState.ToString()).Returns(nameof(startState));
            A.CallTo(() => probabilisticState1.ToString()).Returns(nameof(probabilisticState1));
            A.CallTo(() => probabilisticState2.ToString()).Returns(nameof(probabilisticState2));
            A.CallTo(() => evaluation2State.ToString()).Returns(nameof(evaluation2State));
            A.CallTo(() => evaluationNagitive2State.ToString()).Returns(nameof(evaluationNagitive2State));

            evaluation2State.SetEvaluationTo(2);
            evaluationNagitive2State.SetEvaluationTo(-2);
            
            A.CallTo(() => startState.Turn).Returns(Player.Max);
            A.CallTo(() => probabilisticState1.Turn).Returns(Player.Min);
            A.CallTo(() => probabilisticState2.Turn).Returns(Player.Min);
            A.CallTo(() => evaluation2State.Turn).Returns(Player.Max);
            A.CallTo(() => evaluationNagitive2State.Turn).Returns(Player.Max);

            evaluation2State.SetAsEndState();
            evaluationNagitive2State.SetAsEndState();
            startState.SetNeigbors(new List<IState> {probabilisticState1, probabilisticState2});  
        }

        [DataRow(1, ParallelismMode.TotalParallelism)]
        [DataRow(2, ParallelismMode.TotalParallelism)]
        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(1, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void Search_TowProbabilisticStates_ReturnBetterState(int degreeOfParallelism, ParallelismMode parallelismMode)
        {
            A.CallTo(() => probabilisticState1.GetNeighbors()).Returns(new List<Tuple<double, IEnumerable<IState>>>()
            {
                new Tuple<double, IEnumerable<IState>>(0.4, new List<IState> {evaluation2State}),
                new Tuple<double, IEnumerable<IState>>(0.6, new List<IState> {evaluationNagitive2State}),
            });
            A.CallTo(() => probabilisticState2.GetNeighbors()).Returns(new List<Tuple<double, IEnumerable<IState>>>()
            {
                new Tuple<double, IEnumerable<IState>>(0.1, new List<IState> {evaluation2State}),
                new Tuple<double, IEnumerable<IState>>(0.4, new List<IState> {evaluationNagitive2State}),
                new Tuple<double, IEnumerable<IState>>(0.5, new List<IState> {evaluation2State}),
            });

            var searchEngine = TestUtils.GetBasicSearchEngine(parallelismMode, degreeOfParallelism);
            var searchResult = searchEngine.Search(startState, 10);

            Assert.AreEqual(probabilisticState2, searchResult.NextMove, $"Should have found {nameof(probabilisticState2)} as the nextState");
            Assert.IsTrue(Math.Abs(searchResult.Evaluation - 0.4) < 0.01, "Evaluation should have been .4; actual result is " + searchResult.Evaluation);
        }

        [TestMethod]
        public void Search_StateWithNoRealNeighbors_EvaluatesState()
        {
            A.CallTo(() => startState.GetNeighbors()).Returns(new List<IState> {probabilisticState1});
            A.CallTo(() => probabilisticState1.GetNeighbors()).Returns(new List<Tuple<double, IEnumerable<IState>>>()
            {
                new Tuple<double, IEnumerable<IState>>(0.5, new List<IState>()),
                new Tuple<double, IEnumerable<IState>>(0.5, new List<IState>()),
            });
            probabilisticState1.SetEvaluationTo(2);

            var searchEngine = TestUtils.GetBasicSearchEngine();
            var searchResult = searchEngine.Search(startState, 10);

            Assert.AreEqual(2, searchResult.Evaluation);
        }

        [DataRow(1, ParallelismMode.TotalParallelism)]
        [DataRow(2, ParallelismMode.TotalParallelism)]
        [DataRow(8, ParallelismMode.TotalParallelism)]
        [DataRow(1, ParallelismMode.FirstLevelOnly)]
        [TestMethod]
        public void Search_CheckThatRecordPassThroughStatesIsWorking(int degreeOfParallelism, ParallelismMode parallelismMode)
        {
            A.CallTo(() => evaluation2State.Evaluate(A<int>._, A<List<IState>>.That.IsEmpty()))
                .Throws(new Exception("passedStats list should have been empty"));
            A.CallTo(() => evaluationNagitive2State.Evaluate(A<int>._, A<List<IState>>._))
                .Invokes((int i, List<IState> l) =>
                {
                    Assert.AreEqual(2, l.Count, "passThroughStates should have two states in it (startState and probabilisticState1)");
                    Assert.IsTrue(l.Contains(probabilisticState1), "passThroughStates should contain" + nameof(probabilisticState1));
                });

            startState.SetNeigbor(probabilisticState1);
            A.CallTo(() => probabilisticState1.GetNeighbors()).Returns(new List<Tuple<double, IEnumerable<IState>>>()
            {
                new Tuple<double, IEnumerable<IState>>(1, new List<IState> {evaluation2State, evaluationNagitive2State})
            });

            var searchEngine = TestUtils.GetBasicSearchEngine(parallelismMode, degreeOfParallelism);
            searchEngine.Search(startState, 5);
        }
    }
}
