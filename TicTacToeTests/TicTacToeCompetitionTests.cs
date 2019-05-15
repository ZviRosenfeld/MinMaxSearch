using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch;
using MinMaxSearch.Benckmarking;
using MinMaxSearch.States;

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
            var competitionResult = engine.Compete(new AlternateEvaluationTicTacToeState(new[,]
            {
                { Player.Empty, Player.Empty, Player.Empty},
                { Player.Empty, Player.Empty, Player.Empty},
                { Player.Empty, Player.Empty, Player.Empty},
            }, Player.Max), 9, 100);

            var finalState = (TicTacToeState)competitionResult.States.Last();
            Assert.AreEqual(TicTacToeState.MaxValue, finalState.Evaluate(0, new List<IState>()), "Max should have won");
        }
    }

    class AlternateEvaluationTicTacToeState : TicTacToeState, IAlternateEvaluationState
    {
        public AlternateEvaluationTicTacToeState(Player[,] board, Player turn) : base(board, turn)
        {
        }

        public double AlternateEvaluation(int depth, List<IState> passedThroughStates) => 0;

        public override IEnumerable<IState> GetNeighbors()
        {
            var stateEvaluation = Evaluate(0, new List<IState>());
            if (stateEvaluation >= MaxValue || stateEvaluation <= MinValue)
                return new List<AlternateEvaluationTicTacToeState>();

            var neighbors = new List<AlternateEvaluationTicTacToeState>();
            for (var i = 0; i < 3; i++)
            for (var j = 0; j < 3; j++)
                if (Board[i, j] == Player.Empty)
                {
                    var newBoard = (Player[,])Board.Clone();
                    newBoard[i, j] = Turn;
                    neighbors.Add(new AlternateEvaluationTicTacToeState(newBoard, Turn.GetReversePlayer()));
                }
            return neighbors;
        }
    }
}
