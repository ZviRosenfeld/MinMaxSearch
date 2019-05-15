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
        public void Compete_EgngineNineDepthsBeetsEngineOneDepth()
        {
            var engine = TicTacToeBassicTests.GetSearchEngine(1);
            var competitionResult = CompetitionManager.Compete(engine, engine,
                TicTacToeBassicTests.GetEmptyTicTacToeState(), 1, 9, 100);

            var finalState = (TicTacToeState) competitionResult.States.Last();
            Assert.AreEqual(TicTacToeState.MinValue, finalState.Evaluate(0, new List<IState>()), "Min should have won");
            Assert.AreEqual(finalState, competitionResult.FinalState, $"{nameof(competitionResult.FinalState)} is different than the last state in {nameof(competitionResult.States)}");
            Assert.IsTrue(competitionResult.MaxTotalTime < competitionResult.MinTotalTime, "Min should have searched for longer then Max");
        }

        [TestMethod]
        public void Compete_TestAlternateEvaluationStrategie()
        {
            var engine = TicTacToeBassicTests.GetSearchEngine(1);
            var competitionResult = engine.Compete(TicTacToeBassicTests.GetEmptyTicTacToeState(), 9, (s, d, l) => s.Evaluate(d, l), (s, d, l) => 0);

            var finalState = (TicTacToeState)competitionResult.States.Last();
            Assert.AreEqual(TicTacToeState.MaxValue, finalState.Evaluate(0, new List<IState>()), "Max should have won");
        }
    }
}
