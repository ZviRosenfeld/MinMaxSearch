using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch.UnitTests.TestStates;

namespace MinMaxSearch.UnitTests
{
    [TestClass]
    public class SearchEngineTests
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
            A.CallTo(() => state1.ToString()).Returns("State1");
            A.CallTo(() => state2.ToString()).Returns("State2");
            A.CallTo(() => state3.ToString()).Returns("State3");
            A.CallTo(() => endState1.ToString()).Returns("EndState1");
            A.CallTo(() => endState2.ToString()).Returns("EndState2");
            A.CallTo(() => endState3.ToString()).Returns("EndState3");
            A.CallTo(() => endState1.GetNeighbors()).Returns(new List<IState>());
            A.CallTo(() => endState2.GetNeighbors()).Returns(new List<IState>());
            A.CallTo(() => endState3.GetNeighbors()).Returns(new List<IState>());
            A.CallTo(() => state1.Turn).Returns(Player.Max);
            A.CallTo(() => state2.Turn).Returns(Player.Max);
            A.CallTo(() => state3.Turn).Returns(Player.Max);
            A.CallTo(() => endState1.Turn).Returns(Player.Max);
            A.CallTo(() => endState2.Turn).Returns(Player.Max);
            A.CallTo(() => endState3.Turn).Returns(Player.Max);
        }
        
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(8)]
        [TestMethod]
        public void Search_MaxHasTwoTurnsInARow_FindBestMove(int degreeOfParallelism)
        {
            A.CallTo(() => state1.GetNeighbors()).Returns(new List<IState> { endState1, endState2, endState3});
            
            A.CallTo(() => endState1.Evaluate(A<int>._, A<List<IState>>._)).Returns(2);
            A.CallTo(() => endState2.Evaluate(A<int>._, A<List<IState>>._)).Returns(1);
            A.CallTo(() => endState3.Evaluate(A<int>._, A<List<IState>>._)).Returns(3);

            var engine = new SearchEngine { MaxDegreeOfParallelism = degreeOfParallelism};
            var result = engine.Search(state1, 5);

            Assert.AreEqual(endState3, result.NextMove, "Actually found " + result.NextMove);
        }

        [DataRow(1)]
        [DataRow(2)]
        [DataRow(8)]
        [TestMethod]
        public void Search_RowOfMixedMove_FindBest(int degreeOfParallelism)
        {
            A.CallTo(() => state1.GetNeighbors()).Returns(new List<IState> { endState1, endState2, endState3 });

            A.CallTo(() => state1.Turn).Returns(Player.Max);
            A.CallTo(() => endState1.Turn).Returns(Player.Max);
            A.CallTo(() => endState2.Turn).Returns(Player.Min);
            A.CallTo(() => endState3.Turn).Returns(Player.Max);

            A.CallTo(() => endState1.Evaluate(A<int>._, A<List<IState>>._)).Returns(2);
            A.CallTo(() => endState2.Evaluate(A<int>._, A<List<IState>>._)).Returns(1);
            A.CallTo(() => endState3.Evaluate(A<int>._, A<List<IState>>._)).Returns(3);

            var engine = new SearchEngine { MaxDegreeOfParallelism = degreeOfParallelism };
            var result = engine.Search(state1, 5);

            Assert.AreEqual(endState3, result.NextMove, "Actually found " + result.NextMove);
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
            A.CallTo(() => endState1.Evaluate(A<int>.Ignored, A<List<IState>>.That.IsEmpty()))
                .Throws(new Exception("passedStats list should have been empty"));
            A.CallTo(() => endState1.Evaluate(A<int>.Ignored, A<List<IState>>._))
                .Invokes((int i, List<IState> l) =>
                {
                    Assert.AreEqual(1, l.Count, "passThroughStates should only have one node (state1)");
                    Assert.IsTrue(l.Contains(state1), "passThroughStates should contain state1");
                });

            A.CallTo(() => state1.GetNeighbors()).Returns(new List<IDeterministicState> { endState1 });

            A.CallTo(() => state1.Turn).Returns(Player.Max);
            A.CallTo(() => endState1.Turn).Returns(Player.Min);

            var searchEngine = new SearchEngine {MaxDegreeOfParallelism = degreeOfParallelism};
            searchEngine.Search(state1, 5);
        }
        
        [DataRow(1)]
        [TestMethod]
        public void Search_DieEarllyOptionWorks(int degreeOfParallelism)
        {
            A.CallTo(() => endState1.Evaluate(A<int>.Ignored, A<List<IState>>._)).Returns(10);
            A.CallTo(() => endState2.Evaluate(A<int>.Ignored, A<List<IState>>._)).Returns(15);
            A.CallTo(() => endState3.Evaluate(A<int>.Ignored, A<List<IState>>._)).Returns(0);
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
        
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(8)]
        [TestMethod]
        public void Search_TaskCanceld_DontContinueSearching(int degreeOfParallelism)
        {
            var cancellationSource = new CancellationTokenSource();

            A.CallTo(() => state1.GetNeighbors()).ReturnsLazily(() =>
            {
                cancellationSource.Cancel();
                return new List<IDeterministicState> {endState1};
            });

            A.CallTo(() => state1.Evaluate(A<int>.Ignored, A<List<IState>>._)).Returns(1);
            A.CallTo(() => endState1.Evaluate(A<int>.Ignored, A<List<IState>>._)).Returns(2);
            
            A.CallTo(() => state1.Turn).Returns(Player.Min);
            A.CallTo(() => endState1.Turn).Returns(Player.Max);
            
            var searchEngine = new SearchEngine() { MaxDegreeOfParallelism = degreeOfParallelism };
            var result = searchEngine.Search(state1, 5, cancellationSource.Token);

            Assert.AreEqual(1, result.Evaluation);
            Assert.AreEqual(0, result.StateSequence.Count, "We shouldn't have gotten to state3");
        }
    }
}
