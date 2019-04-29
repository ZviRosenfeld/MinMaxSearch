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

            var searchEngine = new SearchEngine(1);
            searchEngine.Evaluate(state, Player.Max);
        }

        [TestMethod]
        [ExpectedException(typeof(EmptyPlayerException))]
        public void Evaluate_StartEmptyPlayer_ExceptionThrown()
        {
            var state = A.Fake<IState>();
            A.CallTo(() => state.GetNeighbors()).Returns(new List<IState>{state});

            var searchEngine = new SearchEngine(1);
            searchEngine.Evaluate(state, Player.Empty);
        }

        [TestMethod]
        public void Evaluate_DontStopWithUnstableState()
        {
            var searchEngine = new SearchEngine(1) {IsUnstableState = (s, d, l) => s.Evaluate(d, l) < 10};
            var result = searchEngine.Evaluate(new IncreasingNumberState(0), Player.Max);

            Assert.AreEqual(10, result.Evaluation, "Engine seems to have stopped before reaching a stable state");
        }
        
        [TestMethod]
        public void Evaluate_CheckThatRecordPassThroughStatesOptionIsWorking()
        {
            var state2 = A.Fake<IState>();
            A.CallTo(() => state2.GetNeighbors()).Returns(new List<IState>());
            A.CallTo(() => state2.Evaluate(A<int>.Ignored, A<List<IState>>.That.IsEmpty()))
                .Throws(new Exception("passedStats list should have been empty"));

            var state1 = A.Fake<IState>();
            A.CallTo(() => state1.GetNeighbors()).Returns(new List<IState> { state2 });

            var searchEngine = new SearchEngine(5);
            searchEngine.Evaluate(state1, Player.Max);
        }

        [TestMethod]
        public void Evaluate_CheckThatStateSequenceIsCorrectWhenRememberDeadEndOptionIsWorking()
        {
            var endState = A.Fake<IState>();
            A.CallTo(() => endState.GetNeighbors()).Returns(new List<IState>());
            A.CallTo(() => endState.Evaluate(A<int>.Ignored, A<List<IState>>.That.IsEmpty())).Returns(10);

            var state2 = A.Fake<IState>();
            A.CallTo(() => state2.GetNeighbors()).Returns(new List<IState> { endState });
            
            var state3 = A.Fake<IState>();
            A.CallTo(() => state3.GetNeighbors()).Returns(new List<IState> { state2 });

            var state1 = A.Fake<IState>();
            A.CallTo(() => state1.GetNeighbors()).Returns(new List<IState> { state3, state2 });

            var searchEngine = new SearchEngine(5) { RemeberDeadEndStates = true, FavorShortPaths = true};
            var evaluation = searchEngine.Evaluate(state1, Player.Max);
            
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

            var searchEngine = new SearchEngine(2) { DieEarly = true, MaxScore = 5, MinScore = 5};
            var evaluation = searchEngine.Evaluate(state1, Player.Min);

            Assert.AreEqual(endState1, evaluation.StateSequence.Last(), "Should have ended with endState1; found: " + evaluation.StateSequence.Last());
        }

        [TestMethod]
        public void Evaluate_CheckPreventLoopPrunerWorks()
        {
            var searchEngine = new SearchEngine(5) {PreventLoops = true};
            searchEngine.Evaluate(new NeverEndingState(0), Player.Max);
        }
    }
}
