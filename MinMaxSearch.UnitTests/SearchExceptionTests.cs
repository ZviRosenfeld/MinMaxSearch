﻿using System;
using System.Collections.Generic;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch.UnitTests.TestStates;

namespace MinMaxSearch.UnitTests
{
    [TestClass]
    public class SearchExceptionTests
    {
        [TestMethod]
        [ExpectedException(typeof(NoNeighborsException))]
        public void Search_StartStateHasNoNeighbors_ExceptionThrown()
        {
            var state = A.Fake<IDeterministicState>();
            A.CallTo(() => state.GetNeighbors()).Returns(new List<IDeterministicState>());
            A.CallTo(() => state.Turn).Returns(Player.Empty);

            var searchEngine = new SearchEngineBuilder().Build();
            searchEngine.Search(state, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(EmptyPlayerException))]
        public void Search_StartEmptyPlayer_ExceptionThrown()
        {
            var state = A.Fake<IDeterministicState>();
            A.CallTo(() => state.GetNeighbors()).Returns(new List<IDeterministicState> { state });
            A.CallTo(() => state.Turn).Returns(Player.Empty);

            var searchEngine = new SearchEngineBuilder().Build();
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

            var searchEngine = new SearchEngineBuilder { ParallelismMode = ParallelismMode.NonParallelism}.Build();
            searchEngine.Search(startState, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(BadDegreeOfParallelismException))]
        public void Search_StartWithZeroDegreeOfParallelism_ExceptionThrown()
        {
            var state = A.Fake<IDeterministicState>();
            A.CallTo(() => state.GetNeighbors()).Returns(new List<IDeterministicState> { state });
            A.CallTo(() => state.Turn).Returns(Player.Max);

            var searchEngine = new SearchEngineBuilder() { MaxDegreeOfParallelism = 0 }.Build();
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

            var searchEngine = new SearchEngineBuilder() { ParallelismMode = ParallelismMode.NonParallelism }.Build();
            searchEngine.Search(state, 5);
        }

        [TestMethod]
        [DataRow(-2)]
        [DataRow(0)]
        [ExpectedException(typeof(ArgumentException))]
        public void Search_NegativeDepth_ThrowException(int depth)
        {
            var engine = new SearchEngineBuilder().Build();
            engine.Search(new IncreasingNumberState(1, Player.Max), depth);
        }
    }
}
