using System;
using System.Collections.Generic;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch.UnitTests.TestStates;

namespace MinMaxSearch.UnitTests
{
    class SearchExceptionTests
    {
        [TestMethod]
        [ExpectedException(typeof(NoNeighborsException))]
        public void Search_StartStateHasNoNeighbors_ExceptionThrown()
        {
            var state = A.Fake<IDeterministicState>();
            A.CallTo(() => state.GetNeighbors()).Returns(new List<IDeterministicState>());
            A.CallTo(() => state.Turn).Returns(Player.Empty);

            var searchEngine = new SearchEngine();
            searchEngine.Search(state, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(EmptyPlayerException))]
        public void Search_StartEmptyPlayer_ExceptionThrown()
        {
            var state = A.Fake<IDeterministicState>();
            A.CallTo(() => state.GetNeighbors()).Returns(new List<IDeterministicState> { state });
            A.CallTo(() => state.Turn).Returns(Player.Empty);

            var searchEngine = new SearchEngine();
            searchEngine.Search(state, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(EmptyPlayerException))]
        public void Search_EmptyPlayerStateInSearch_ExceptionThrown()
        {
            var state = A.Fake<IDeterministicState>();
            A.CallTo(() => state.GetNeighbors()).Returns(new List<IDeterministicState> { state });
            A.CallTo(() => state.Turn).Returns(Player.Empty);

            var startState = A.Fake<IDeterministicState>();
            A.CallTo(() => startState.GetNeighbors()).Returns(new List<IDeterministicState> { state });
            A.CallTo(() => startState.Turn).Returns(Player.Min);

            var searchEngine = new SearchEngine();
            searchEngine.Search(startState, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(BadDegreeOfParallelismException))]
        public void Search_StartWithZeroDegreeOfParallelism_ExceptionThrown()
        {
            var state = A.Fake<IDeterministicState>();
            A.CallTo(() => state.GetNeighbors()).Returns(new List<IDeterministicState> { state });
            A.CallTo(() => state.Turn).Returns(Player.Max);

            var searchEngine = new SearchEngine() { MaxDegreeOfParallelism = 0 };
            searchEngine.Search(state, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(BadStateTypeException))]
        public void Search_ContainsAnIState_ExceptionThrown()
        {
            var istate = A.Fake<IState>();
            var state = A.Fake<IDeterministicState>();
            A.CallTo(() => state.GetNeighbors()).Returns(new List<IState> { istate });
            A.CallTo(() => state.Turn).Returns(Player.Max);
            A.CallTo(() => istate.Turn).Returns(Player.Max);

            var searchEngine = new SearchEngine() { MaxDegreeOfParallelism = 1 };
            searchEngine.Search(state, 5);
        }


        [TestMethod]
        [ExpectedException(typeof(Exception), "Shouldn't have gotten so far into the search")]
        public void Search_CheckThatAfterSettingPreventLoopsToTrueItCanBeTurnedBackToFalse()
        {
            var searchEngine = new SearchEngine() { PreventLoops = true };
            searchEngine.PreventLoops = false;
            searchEngine.Search(new ThrowExceptionAtDepthThreeState(0, Player.Max), 5);
        }
    }
}
