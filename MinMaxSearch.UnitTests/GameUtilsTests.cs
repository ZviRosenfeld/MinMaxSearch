using System;
using System.Collections.Generic;
using System.Linq;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MinMaxSearch.UnitTests
{
    [TestClass]
    public class GameUtilsTests
    {
        [TestMethod]
        public void GetReversePlayer_PlayerMin_ReturnsMax()
        {
            var reversePlayer = Player.Min.GetReversePlayer();
            Assert.AreEqual(Player.Max, reversePlayer);
        }

        [TestMethod]
        public void GetReversePlayer_PlayerMax_ReturnsMin()
        {
            var reversePlayer = Player.Max.GetReversePlayer();
            Assert.AreEqual(Player.Min, reversePlayer);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetReversePlayer_PlayerEmpty_ThrowException() => 
            Player.Empty.GetReversePlayer();

        // This is a probablistic test, so you might need to give it some leeway
        [TestMethod]
        public void GetNextState_GetLikelyNeigborsMoreOften()
        {
            var probabilisticState = A.Fake<IProbabilisticState>();
            var state1 = A.Fake<IState>();
            var state2 = A.Fake<IState>();
            var state3 = A.Fake<IState>();

            A.CallTo(() => probabilisticState.GetNeighbors()).Returns(new List<Tuple<double, List<IState>>>()
            {
                new Tuple<double, List<IState>>(0.1, new List<IState> {state1}),
                new Tuple<double, List<IState>>(0.3, new List<IState> {state2}),
                new Tuple<double, List<IState>>(0.6, new List<IState> {state3}),
            });

            int timesGotState1 = 0, timesGotState2 = 0, timesGotState3 = 0;

            for (int i = 0; i < 100; i++)
            {
                var nextState = probabilisticState.GetNextState();
                if (nextState.GetNeighbors().Contains(state1))
                    timesGotState1++;
                if (nextState.GetNeighbors().Contains(state2))
                    timesGotState2++;
                if (nextState.GetNeighbors().Contains(state3))
                    timesGotState3++;
            }

            Assert.IsTrue(timesGotState1 < 20, nameof(state1) + " was returned " + timesGotState1 + " times");
            Assert.IsTrue(timesGotState1 > 5, nameof(state1) + " was returned " + timesGotState1 + " times");
            Assert.IsTrue(timesGotState2 < 45, nameof(state2) + " was returned " + timesGotState2 + " times");
            Assert.IsTrue(timesGotState2 > 15, nameof(state2) + " was returned " + timesGotState2 + " times");
            Assert.IsTrue(timesGotState3 < 80, nameof(state3) + " was returned " + timesGotState3 + " times");
            Assert.IsTrue(timesGotState3 > 40, nameof(state3) + " was returned " + timesGotState3 + " times");
        }
    }
}
