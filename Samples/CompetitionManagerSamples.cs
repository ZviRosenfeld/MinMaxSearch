using System;
using System.Collections.Generic;
using MinMaxSearch;
using MinMaxSearch.Benchmarking;
using TicTacToeTests;

namespace Samples
{
    class CompetitionManagerSamples
    {
        public void DifferentDepths()
        {
            IDeterministicState startState = Utils.GetEmptyTicTacToeState();
            int minSearchDepth = 2;
            int maxSearchDepth = 5;
            SearchEngine engine = new SearchEngine();
            
            CompetitionResult competitionResult = engine.Compete(startState, maxSearchDepth, minSearchDepth);

            // Print some of the results
            Console.WriteLine("Max Search Time " + competitionResult.MaxTotalTime);
            Console.WriteLine("Min Search Time " + competitionResult.MinTotalTime);
            Console.WriteLine("Final Score " + competitionResult.FinalState.Evaluate(0, new List<IState>()));
        }

        public void MinAlternateEvaluation()
        {
            IDeterministicState startState = Utils.GetEmptyTicTacToeState();
            int searchDepth = 6;
            SearchEngine engine = new SearchEngine();

            CompetitionResult competitionResult = engine.Compete(startState, searchDepth, minAlternateEvaluation: (s, d, l) => {
                // Some alternate evaluation goes here - you probably don't really want to return 0
                return 0;
            });

            // Print some of the results
            Console.WriteLine("Max Search Time " + competitionResult.MaxTotalTime);
            Console.WriteLine("Min Search Time " + competitionResult.MinTotalTime);
            Console.WriteLine("Final Score " + competitionResult.FinalState.Evaluate(0, new List<IState>()));

        }

        public void DifferentEngines()
        {
            IDeterministicState startState = Utils.GetEmptyTicTacToeState();
            int searchDepth = 5;
            int playDepth = 100;
            SearchEngine engine1 = new SearchEngine();
            SearchEngine engine2 = new SearchEngine();

            CompetitionResult competitionResult = CompetitionManager.Compete(engine1, engine2, startState, searchDepth, searchDepth, playDepth);

            // Print some of the results
            Console.WriteLine("Max Search Time " + competitionResult.MaxTotalTime);
            Console.WriteLine("Min Search Time " + competitionResult.MinTotalTime);
            Console.WriteLine("Final Score " + competitionResult.FinalState.Evaluate(0, new List<IState>()));
        }
    }
}
