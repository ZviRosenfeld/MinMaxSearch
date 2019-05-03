using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MinMaxSearch.UnitTests
{
    [TestClass]
    public class SearchEngineTests
    {
        [TestMethod]
        [ExpectedException(typeof(NoNeighborsException))]
        public void Search_StartStateHasNoNeighbors_ExceptionThrown()
        {
            var state = A.Fake<IState>();
            A.CallTo(() => state.GetNeighbors()).Returns(new List<IState>());
            A.CallTo(() => state.Turn).Returns(Player.Empty);

            var searchEngine = new SearchEngine();
            searchEngine.Search(state, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(EmptyPlayerException))]
        public void Search_StartEmptyPlayer_ExceptionThrown()
        {
            var state = A.Fake<IState>();
            A.CallTo(() => state.GetNeighbors()).Returns(new List<IState>{state});
            A.CallTo(() => state.Turn).Returns(Player.Empty);

            var searchEngine = new SearchEngine();
            searchEngine.Search(state, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(EmptyPlayerException))]
        public void Search_EmptyPlayerStateInSearch_ExceptionThrown()
        {
            var state = A.Fake<IState>();
            A.CallTo(() => state.GetNeighbors()).Returns(new List<IState> { state });
            A.CallTo(() => state.Turn).Returns(Player.Empty);

            var startState = A.Fake<IState>();
            A.CallTo(() => startState.GetNeighbors()).Returns(new List<IState> { state });
            A.CallTo(() => startState.Turn).Returns(Player.Min);

            var searchEngine = new SearchEngine();
            searchEngine.Search(startState, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(BadDegreeOfParallelismException))]
        public void Search_StartWithZeroDegreeOfParallelism_ExceptionThrown()
        {
            var state = A.Fake<IState>();
            A.CallTo(() => state.GetNeighbors()).Returns(new List<IState> { state });
            A.CallTo(() => state.Turn).Returns(Player.Empty);

            var searchEngine = new SearchEngine() { MaxDegreeOfParallelism = 0};
            searchEngine.Search(state, 1);
        }

        [DataRow(1)]
        [DataRow(2)]
        [DataRow(8)]
        [TestMethod]
        public void Search_DontStopWithUnstableState(int degreeOfParallelism)
        {
            var searchEngine = new SearchEngine() {IsUnstableState = (s, d, l) => s.Evaluate(d, l) < 10, MaxDegreeOfParallelism = degreeOfParallelism};
            var result = searchEngine.Search(new IncreasingNumberState(0, Player.Max), 1);

            Assert.AreEqual(10, result.Evaluation, "Engine seems to have stopped before reaching a stable state");
        }

        [DataRow(1)]
        [DataRow(2)]
        [DataRow(8)]
        [TestMethod]
        public void Search_CheckThatRecordPassThroughStatesIsWorking(int degreeOfParallelism)
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

            A.CallTo(() => state1.Turn).Returns(Player.Max);
            A.CallTo(() => state2.Turn).Returns(Player.Min);

            var searchEngine = new SearchEngine {MaxDegreeOfParallelism = degreeOfParallelism};
            searchEngine.Search(state1, 5);
        }
        
        [DataRow(1)]
        //[DataRow(2)]
        //[DataRow(8)]
        [TestMethod]
        public void Search_DieEarllyOptionWorks(int degreeOfParallelism)
        {
            var endState1 = A.Fake<IState>();
            A.CallTo(() => endState1.GetNeighbors()).Returns(new List<IState>());
            A.CallTo(() => endState1.Evaluate(A<int>.Ignored, A<List<IState>>._)).Returns(10);
            A.CallTo(() => endState1.ToString()).Returns("endState1");

            var endState2 = A.Fake<IState>();
            A.CallTo(() => endState2.GetNeighbors()).Returns(new List<IState>());
            A.CallTo(() => endState2.Evaluate(A<int>.Ignored, A<List<IState>>._)).Returns(15);
            A.CallTo(() => endState2.ToString()).Returns("endState2");

            var endState3 = A.Fake<IState>();
            A.CallTo(() => endState3.GetNeighbors()).Returns(new List<IState>());
            A.CallTo(() => endState3.Evaluate(A<int>.Ignored, A<List<IState>>._)).Returns(0);
            A.CallTo(() => endState3.ToString()).Returns("endState3");

            var state1 = A.Fake<IState>();
            A.CallTo(() => state1.GetNeighbors()).Returns(new List<IState> { endState1, endState2, endState3 });

            A.CallTo(() => state1.Turn).Returns(Player.Min);
            A.CallTo(() => endState1.Turn).Returns(Player.Max);
            A.CallTo(() => endState2.Turn).Returns(Player.Max);
            A.CallTo(() => endState3.Turn).Returns(Player.Max);

            var searchEngine = new SearchEngine() { DieEarly = true, MaxScore = 5, MinScore = 5, MaxDegreeOfParallelism = degreeOfParallelism};
            var evaluation = searchEngine.Search(state1, 2);

            Assert.AreEqual(endState1, evaluation.StateSequence.Last(), "Should have ended with endState1; found: " + evaluation.StateSequence.Last());
        }

        [DataRow(1)]
        [DataRow(2)]
        [DataRow(8)]
        [TestMethod]
        public void Search_CheckPreventLoopPrunerWorks(int degreeOfParallelism)
        {
            var searchEngine = new SearchEngine() {PreventLoops = true, MaxDegreeOfParallelism = degreeOfParallelism};
            searchEngine.Search(new ThrowExceptionAtDepthThreeState(0, Player.Max), 5);
        }
        
        [TestMethod]
        [ExpectedException(typeof(Exception), "Shouldn't have gotten so far into the search")]
        public void Search_CheckThatAfterSettingPreventLoopsToTrueItCanBeTurnedBackToFalse()
        {
            var searchEngine = new SearchEngine() { PreventLoops = true};
            searchEngine.PreventLoops = false;
            searchEngine.Search(new ThrowExceptionAtDepthThreeState(0, Player.Max), 5);
        }

        [DataRow(1)]
        [DataRow(2)]
        [DataRow(8)]
        [TestMethod]
        public void Search_TaskCanceld_DontContinueSearching(int degreeOfParallelism)
        {
            var cancellationSource = new CancellationTokenSource();

            var state1 = A.Fake<IState>();
            var state2 = A.Fake<IState>();
            var state3 = A.Fake<IState>();
            A.CallTo(() => state1.GetNeighbors()).Returns(new List<IState> { state2 });
            A.CallTo(() => state2.GetNeighbors()).ReturnsLazily(() =>
            {
                cancellationSource.Cancel();
                return new List<IState> {state3};
            });
            A.CallTo(() => state3.GetNeighbors()).Returns(new List<IState>());

            A.CallTo(() => state1.Evaluate(A<int>.Ignored, A<List<IState>>._)).Returns(1);
            A.CallTo(() => state2.Evaluate(A<int>.Ignored, A<List<IState>>._)).Returns(2);
            A.CallTo(() => state3.Evaluate(A<int>.Ignored, A<List<IState>>._)).Returns(3);

            A.CallTo(() => state1.Turn).Returns(Player.Min);
            A.CallTo(() => state2.Turn).Returns(Player.Max);
            A.CallTo(() => state3.Turn).Returns(Player.Min);

            A.CallTo(() => state1.ToString()).Returns("State1");
            A.CallTo(() => state2.ToString()).Returns("State2");
            A.CallTo(() => state3.ToString()).Returns("State3");

            var searchEngine = new SearchEngine() { MaxDegreeOfParallelism = degreeOfParallelism };
            var result = searchEngine.Search(state1, 5, cancellationSource.Token);

            Assert.AreEqual(2, result.Evaluation);
            Assert.AreEqual(1, result.StateSequence.Count, "We shouldn't have gotten to state3");
        }
    }
}
