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
            A.CallTo(() => startState.Turn).Returns(Player.Max);
            A.CallTo(() => deterministicEndState.Turn).Returns(Player.Min);
            A.CallTo(() => probabilisticEndState.Turn).Returns(Player.Min);
        }
        
        [TestMethod]
        public void DeterministicStateTest()
        {
            startState.SetNeigbor(deterministicEndState);

            var engine = new SearchEngine()
            {
                AlternateEvaluation = (s,d,l) => 10,
                SkipEvaluationForFirstNodeSingleNeighbor = false
            };
            var evaluation = engine.Search(startState, 2);

            Assert.AreEqual(10, evaluation.Evaluation);
        }
        
        [TestMethod]
        public void ProbabilisticStateTest()
        {
            startState.SetNeigbor(probabilisticEndState);
            var engine = new SearchEngine()
            {
                AlternateEvaluation = (s, d, l) => 10,
                SkipEvaluationForFirstNodeSingleNeighbor = false
            };
            var evaluation = engine.Search(startState, 2);

            Assert.AreEqual(10, evaluation.Evaluation);
        }
    }
}
