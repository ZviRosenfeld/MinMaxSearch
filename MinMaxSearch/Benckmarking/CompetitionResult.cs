using System;
using System.Collections.Generic;
using System.Linq;

namespace MinMaxSearch.Benckmarking
{
    public class CompetitionResult
    {
        public CompetitionResult(int gameDepth, List<IState> states, TimeSpan maxTotalTime,
            TimeSpan minTotalTime)
        {
            GameDepth = gameDepth;
            States = states;
            MaxTotalTime = maxTotalTime;
            MinTotalTime = minTotalTime;
        }

        public IState FinalState => States.Last();

        public int GameDepth { get; }

        public List<IState> States { get; }

        public TimeSpan MaxTotalTime { get; }

        public TimeSpan MinTotalTime { get; }
    }
}
