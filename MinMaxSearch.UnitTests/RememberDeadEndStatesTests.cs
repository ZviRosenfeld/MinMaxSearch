using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MinMaxSearch.UnitTests
{
    [TestClass]
    public class RememberDeadEndStatesTests
    {

        private readonly IDeterministicState state1 = A.Fake<IDeterministicState>();
        private readonly IDeterministicState state2 = A.Fake<IDeterministicState>();
        private readonly IDeterministicState state3 = A.Fake<IDeterministicState>();
        private readonly IDeterministicState endState1 = A.Fake<IDeterministicState>();
        private readonly IDeterministicState endState2 = A.Fake<IDeterministicState>();
        private readonly IDeterministicState endState3 = A.Fake<IDeterministicState>();

        [TestInitialize]
        public void TestInitialize()
        {
            /* State tree:
             *                    state1
             *        
             *   state2 =  1      state3 = 3       endState3 = 4
             *   
             *   endState1 = 8    endState2 = 5
            */

            A.CallTo(() => state1.ToString()).Returns("State1");
            A.CallTo(() => state2.ToString()).Returns("State2");
            A.CallTo(() => state3.ToString()).Returns("State3");
            A.CallTo(() => endState1.ToString()).Returns("EndState1");
            A.CallTo(() => endState2.ToString()).Returns("EndState2");
            A.CallTo(() => endState3.ToString()).Returns("EndState3");

            A.CallTo(() => endState1.GetNeighbors()).Returns(new List<IState>());
            A.CallTo(() => endState2.GetNeighbors()).Returns(new List<IState>());
            A.CallTo(() => endState3.GetNeighbors()).Returns(new List<IState>());
            A.CallTo(() => state1.GetNeighbors()).Returns(new List<IState> { state2, state3, endState3 });
            A.CallTo(() => state2.GetNeighbors()).Returns(new List<IState> { endState1 });
            A.CallTo(() => state3.GetNeighbors()).Returns(new List<IState> { endState2 });

            A.CallTo(() => endState1.Evaluate(A<int>._, A<List<IState>>._)).Returns(8);
            A.CallTo(() => endState2.Evaluate(A<int>._, A<List<IState>>._)).Returns(5);
            A.CallTo(() => endState3.Evaluate(A<int>._, A<List<IState>>._)).Returns(4);
            A.CallTo(() => state2.Evaluate(A<int>._, A<List<IState>>._)).Returns(1);
            A.CallTo(() => state3.Evaluate(A<int>._, A<List<IState>>._)).Returns(3);

            A.CallTo(() => state1.Turn).Returns(Player.Max);
            A.CallTo(() => state2.Turn).Returns(Player.Max);
            A.CallTo(() => state3.Turn).Returns(Player.Max);
            A.CallTo(() => endState1.Turn).Returns(Player.Max);
            A.CallTo(() => endState2.Turn).Returns(Player.Max);
            A.CallTo(() => endState3.Turn).Returns(Player.Max);
        }

        [DataRow(RememberStatesMode.Never)]
        [DataRow(RememberStatesMode.InSameSearch)]
        [TestMethod]
        public void Search_DontRememberStates(RememberStatesMode rememberStatesMode)
        {
            var engine = new SearchEngine {RememberDeadEndStates = rememberStatesMode};
            engine.Search(state1, 5); // Do the first search, so that we remember all states
            var result = engine.Search(state1, 1);

            Assert.AreEqual(endState3, result.NextMove);
            Assert.AreEqual(4, result.Evaluation);
        }

        [TestMethod]
        public void Search_TestRememberStates()
        {
            var engine = new SearchEngine {RememberDeadEndStates = RememberStatesMode.BetweenSearches};
            engine.Search(state1, 5); // Do the first search, so that we remember all states
            var result = engine.Search(state1, 1);

            Assert.AreEqual(endState1, result.StateSequence.Last(), $"We should have remembered that {endState1} is the final state");
            Assert.AreEqual(state2, result.NextMove);
            Assert.AreEqual(8, result.Evaluation);
        }

        [TestMethod]
        public void Search_ClearRemovsRememverdStates()
        {
            var engine = new SearchEngine { RememberDeadEndStates = RememberStatesMode.BetweenSearches };
            engine.Search(state1, 5); // Do the first search, so that we remember all states
            engine.Clear();
            var result = engine.Search(state1, 1);
            
            Assert.AreEqual(endState3, result.NextMove);
            Assert.AreEqual(4, result.Evaluation);
        }

        [TestMethod]
        public void Search_SmartClearOnlyClearsRequiredStrates()
        {
            var engine = new SearchEngine { RememberDeadEndStates = RememberStatesMode.BetweenSearches };
            engine.Search(state1, 5); // Do the first search, so that we remember all states
            engine.SmartClear(s => s == state2 || s == state1, CancellationToken.None);
            var result = engine.Search(state1, 1);

            Assert.AreEqual(state3, result.NextMove, $"Next move == {result.NextMove}, when it should have been {nameof(state3)}");
        }
    }
}
