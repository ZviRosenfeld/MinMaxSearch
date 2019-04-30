using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MinMaxSearch.UnitTests
{
    [TestClass]
    public class SearchEngineTests
    {
        [TestMethod]
        [ExpectedException(typeof(NoNeighborsException))]
        public void Evaluate_StartStateHasNoNeighbors_ExceptionThrown()
        {
            var state = A.Fake<IState>();
            A.CallTo(() => state.GetNeighbors()).Returns(new List<IState>());

            var searchEngine = new SearchEngine();
            searchEngine.Evaluate(state, Player.Max, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(EmptyPlayerException))]
        public void Evaluate_StartEmptyPlayer_ExceptionThrown()
        {
            var state = A.Fake<IState>();
            A.CallTo(() => state.GetNeighbors()).Returns(new List<IState>{state});

            var searchEngine = new SearchEngine();
            searchEngine.Evaluate(state, Player.Empty, 1);
        }

        [TestMethod]
        public void Evaluate_DontStopWithUnstableState()
        {
            var searchEngine = new SearchEngine() {IsUnstableState = (s, d, l) => s.Evaluate(d, l) < 10};
            var result = searchEngine.Evaluate(new IncreasingNumberState(0), Player.Max, 1);

            Assert.AreEqual(10, result.Evaluation, "Engine seems to have stopped before reaching a stable state");
        }
        
        [TestMethod]
        public void Evaluate_CheckThatRecordPassThroughStatesOptionIsWorking()
        {
            var state2 = A.Fake<IState>();
            var state1 = A.Fake<IState>();
            A.CallTo(() => state2.GetNeighbors()).Returns(new List<IState>());
            A.CallTo(() => state2.Evaluate(A<int>.Ignored, A<List<IState>>.That.IsEmpty()))
                .Throws(new Exception("passedStats list should have been empty"));
            A.CallTo(() => state2.Evaluate(A<int>.Ignored, A<List<IState>>._))
                .Invokes((int i, List<IState> l) =>
                {
                    Assert.AreEqual(1, l.Count, "passThroughStates should only have one node (state1)");
                    Assert.IsTrue(l.Contains(state1), "passThroughStates should contain state1");
                });

            A.CallTo(() => state1.GetNeighbors()).Returns(new List<IState> { state2 });

            var searchEngine = new SearchEngine();
            searchEngine.Evaluate(state1, Player.Max, 5);
        }

        [TestMethod]
        public void Evaluate_CheckThatStateSequenceIsCorrectWhenRememberDeadEndOptionIsWorking()
        {
            var endState = A.Fake<IState>();
            A.CallTo(() => endState.GetNeighbors()).Returns(new List<IState>());
            A.CallTo(() => endState.Evaluate(A<int>.Ignored, A<List<IState>>.That.IsEmpty())).Returns(10);
            A.CallTo(() => endState.ToString()).Returns("endState");

            var state2 = A.Fake<IState>();
            A.CallTo(() => state2.GetNeighbors()).Returns(new List<IState> { endState });
            A.CallTo(() => state2.ToString()).Returns("state2");

            var state3 = A.Fake<IState>();
            A.CallTo(() => state3.GetNeighbors()).Returns(new List<IState> { state2 });
            A.CallTo(() => state3.ToString()).Returns("state3");

            var state1 = A.Fake<IState>();
            A.CallTo(() => state1.GetNeighbors()).Returns(new List<IState> { state3, state2 });
            A.CallTo(() => state1.ToString()).Returns("state1");

            var searchEngine = new SearchEngine() { RememberDeadEndStates = true, FavorShortPaths = true};
            var evaluation = searchEngine.Evaluate(state1, Player.Max, 5);
            
            Assert.AreEqual(2, evaluation.StateSequence.Count, "StateSequence doesn't  contain all the states is should");
        }

        [DataRow(Player.Max)]
        [DataRow(Player.Min)]
        [TestMethod]
        public void Evaluate_DieEarllyOptionWorks(Player player)
        {
            var endState1 = A.Fake<IState>();
            A.CallTo(() => endState1.GetNeighbors()).Returns(new List<IState>());
            A.CallTo(() => endState1.Evaluate(A<int>.Ignored, A<List<IState>>.That.IsEmpty())).Returns(10);
            A.CallTo(() => endState1.ToString()).Returns("endState1");

            var endState2 = A.Fake<IState>();
            A.CallTo(() => endState2.GetNeighbors()).Returns(new List<IState>());
            A.CallTo(() => endState2.Evaluate(A<int>.Ignored, A<List<IState>>.That.IsEmpty())).Returns(15);
            A.CallTo(() => endState2.ToString()).Returns("endState2");

            var endState3 = A.Fake<IState>();
            A.CallTo(() => endState3.GetNeighbors()).Returns(new List<IState>());
            A.CallTo(() => endState3.Evaluate(A<int>.Ignored, A<List<IState>>.That.IsEmpty())).Returns(0);
            A.CallTo(() => endState3.ToString()).Returns("endState3");

            var state1 = A.Fake<IState>();
            A.CallTo(() => state1.GetNeighbors()).Returns(new List<IState> { endState1, endState2, endState3 });

            var searchEngine = new SearchEngine() { DieEarly = true, MaxScore = 5, MinScore = 5};
            var evaluation = searchEngine.Evaluate(state1, Player.Min, 2);

            Assert.AreEqual(endState1, evaluation.StateSequence.Last(), "Should have ended with endState1; found: " + evaluation.StateSequence.Last());
        }

        [TestMethod]
        public void Evaluate_CheckPreventLoopPrunerWorks()
        {
            var searchEngine = new SearchEngine() {PreventLoops = true};
            searchEngine.Evaluate(new NeverEndingState(0), Player.Max, 5);
        }
    }
}
