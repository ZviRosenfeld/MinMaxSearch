using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MinMaxSearch.UnitTests
{
    [TestClass]
    public class AlternateEvaluationTests
    {
        private readonly IDeterministicState deterministicEndState = A.Fake<IDeterministicState>();
        private readonly IProbabilisticState probabilisticEndState = A.Fake<IProbabilisticState>();
        private readonly IDeterministicState startState = A.Fake<IDeterministicState>();

        [TestInitialize]
        public void TestInitialize()
        {
            A.CallTo(() => deterministicEndState.ToString()).Returns(nameof(deterministicEndState)); 
            deterministicEndState.SetAsEndState();
            deterministicEndState.SetEvaluationTo(0);

            A.CallTo(() => probabilisticEndState.ToString()).Returns(nameof(probabilisticEndState));
            probabilisticEndState.SetAsEndState();
            probabilisticEndState.SetEvaluationTo(0);

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

            var engine = new SearchEngine()
            {
                MaxAlternateEvaluation = (s,d,l) => 10,
                MinAlternateEvaluation = (s,l,d) => -10
            };
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

            var engine = new SearchEngine()
            {
                MaxAlternateEvaluation = (s, d, l) => 10,
                MinAlternateEvaluation = (s, l, d) => -10
            };
            var evaluation = engine.Search(startState, 2);

            Assert.AreEqual(startPlayer == Player.Max ? 10 : -10, evaluation.Evaluation);
        }
    }
}
