using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch.States;

namespace MinMaxSearch.UnitTests
{
    [TestClass]
    public class AlternateEvaluationTests
    {
        private readonly DeterministicAlternateEvaluationState deterministicEndState = A.Fake<DeterministicAlternateEvaluationState>();
        private readonly ProbabilisticAlternateEvaluationState probabilisticEndState = A.Fake<ProbabilisticAlternateEvaluationState>();
        private readonly DeterministicAlternateEvaluationState startState = A.Fake<DeterministicAlternateEvaluationState>();

        [TestInitialize]
        public void TestInitialize()
        {
            A.CallTo(() => deterministicEndState.ToString()).Returns(nameof(deterministicEndState)); 
            deterministicEndState.SetAsEndState();
            deterministicEndState.SetEvaluationTo(10);
            deterministicEndState.SetAlternateEvaluationTo(-10);

            A.CallTo(() => probabilisticEndState.ToString()).Returns(nameof(probabilisticEndState));
            probabilisticEndState.SetAsEndState();
            probabilisticEndState.SetEvaluationTo(10);
            probabilisticEndState.SetAlternateEvaluationTo(-10);

            A.CallTo(() => startState.ToString()).Returns(nameof(startState));
        }
        
        [DataRow(Player.Max)]
        [DataRow(Player.Min)]
        [TestMethod]
        public void DeterministicStateTest(Player startPlayer)
        {
            startState.SetNeigbor(deterministicEndState);
            A.CallTo(() => startState.Turn).Returns(startPlayer);
            A.CallTo(() => deterministicEndState.Turn).Returns(startPlayer.GetReversePlayer());

            var engine = new SearchEngine();
            var evaluation = engine.Search(startState, 2);

            Assert.AreEqual(startPlayer == Player.Max ? 10 : -10, evaluation.Evaluation);
        }

        [DataRow(Player.Max)]
        [DataRow(Player.Min)]
        [TestMethod]
        public void ProbabilisticStateTest(Player startPlayer)
        {
            startState.SetNeigbor(probabilisticEndState);
            A.CallTo(() => startState.Turn).Returns(startPlayer);
            A.CallTo(() => probabilisticEndState.Turn).Returns(startPlayer.GetReversePlayer());

            var engine = new SearchEngine();
            var evaluation = engine.Search(startState, 2);

            Assert.AreEqual(startPlayer == Player.Max ? 10 : -10, evaluation.Evaluation);
        }
    }

    public interface DeterministicAlternateEvaluationState : IDeterministicState, IAlternateEvaluationState
    {        
    }

    public interface ProbabilisticAlternateEvaluationState : IProbabilisticState, IAlternateEvaluationState
    {
    }
}
