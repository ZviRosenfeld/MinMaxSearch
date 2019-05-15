using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch.States;

namespace MinMaxSearch.UnitTests
{
    [TestClass]
    public class AlternateEvaluationTests
    {
        private AlternateEvaluationState endState = A.Fake<AlternateEvaluationState>();
        private AlternateEvaluationState startState = A.Fake<AlternateEvaluationState>();

        [TestInitialize]
        public void TestInitialize()
        {
            A.CallTo(() => endState.ToString()).Returns(nameof(endState)); 
            endState.SetAsEndState();
            endState.SetEvaluationTo(10);
            endState.SetAlternateEvaluationTo(-10);

            A.CallTo(() => startState.ToString()).Returns(nameof(startState));
            startState.SetNeigbor(endState);
        }
        
        [DataRow(Player.Max)]
        [DataRow(Player.Min)]
        [TestMethod]
        public void DepthTwpTest(Player startPlayer)
        {
            A.CallTo(() => startState.Turn).Returns(startPlayer);
            A.CallTo(() => endState.Turn).Returns(startPlayer.GetReversePlayer());

            var engine = new SearchEngine();
            var evaluation = engine.Search(startState, 2);

            Assert.AreEqual(startPlayer == Player.Max ? 10 : -10, evaluation.Evaluation);
        }
    }

    public interface AlternateEvaluationState : IDeterministicState, IAlternateEvaluationState
    {        
    }
}
