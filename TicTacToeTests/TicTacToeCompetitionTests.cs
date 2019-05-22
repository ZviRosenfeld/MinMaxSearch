﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch;
using MinMaxSearch.Benckmarking;

namespace TicTacToeTests
{
    [TestClass]
    public class TicTacToeCompetitionTests
    {
        [TestMethod]
        public void Compete_EngineNineDepthsBeetsEngineOneDepth()
        {
            var engine = TicTacToeBassicTests.GetSearchEngine(1);
            var competitionResult = CompetitionManager.Compete(engine, engine,
                TicTacToeBassicTests.GetEmptyTicTacToeState(), 1, 9, 100);

            var finalState = (TicTacToeState) competitionResult.States.Last();
            Assert.AreEqual(TicTacToeState.MinValue, finalState.Evaluate(0, new List<IState>()), "Min should have won");
            Assert.AreEqual(finalState, competitionResult.FinalState, $"{nameof(competitionResult.FinalState)} is different than the last state in {nameof(competitionResult.States)}");
            Assert.IsTrue(competitionResult.MaxTotalTime < competitionResult.MinTotalTime, "Min should have searched for longer then Max");
            Assert.IsTrue(competitionResult.MaxLongestSearch < competitionResult.MinLongestSearch, "Min's longest should have searched for longer then Max's longest");
            Assert.IsTrue(competitionResult.MaxLongestSearch < competitionResult.MaxTotalTime, "Total time should always be bigger than logest search time");
        }

        [TestMethod]
        public void Compete_TestAlternateEvaluationStrategie()
        {
            var engine = TicTacToeBassicTests.GetSearchEngine(1);
            var competitionResult = engine.Compete(TicTacToeBassicTests.GetEmptyTicTacToeState(), 9, (s, d, l) => s.Evaluate(d, l), (s, d, l) => 0);

            var finalState = (TicTacToeState)competitionResult.States.Last();
            Assert.AreEqual(TicTacToeState.MaxValue, finalState.Evaluate(0, new List<IState>()), "Max should have won");
        }

        [TestMethod]
        public void Compete_GameEndsAtMaxDepth()
        {
            var engine = TicTacToeBassicTests.GetSearchEngine(1);
            var competitionResult = engine.Compete(TicTacToeBassicTests.GetEmptyTicTacToeState(), 2, 2, maxPayDepth: 2);

            Assert.AreEqual(2, competitionResult.GameDepth);
            Assert.AreEqual(2, competitionResult.States.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Compete_MinAndMaxAlternateEvaluation_ThrowException()
        {
            var engine = TicTacToeBassicTests.GetSearchEngine(1);
            engine.Compete(TicTacToeBassicTests.GetEmptyTicTacToeState(), 2);           
        }
    }
}
