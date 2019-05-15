using System;
using System.Collections.Generic;

namespace MinMaxSearch.Benckmarking
{
    public class CompetitionResult
    {
        public CompetitionResult(IState finalState, int gameDepth, List<IState> states, TimeSpan maxTotalTime,
            TimeSpan minTotalTime)
        {
            FinalState = finalState;
            GameDepth = gameDepth;
            States = states;
            MaxTotalTime = maxTotalTime;
            MinTotalTime = minTotalTime;
        }

        public IState FinalState { get; }

        public int GameDepth { get; }

        public List<IState> States { get; }

        public TimeSpan MaxTotalTime { get; }

        public TimeSpan MinTotalTime { get; }
    }
}
