using System;
using System.Collections.Generic;
using System.Linq;

namespace MinMaxSearch.Benchmarking
{
    public class CompetitionResult
    {
        public CompetitionResult(int gameDepth, List<IState> states, TimeSpan maxTotalTime,
            TimeSpan minTotalTime, TimeSpan maxLongestSearch, TimeSpan minLongestSearch)
        {
            GameDepth = gameDepth;
            States = states;
            MaxTotalTime = maxTotalTime;
            MinTotalTime = minTotalTime;
            MaxLongestSearch = maxLongestSearch;
            MinLongestSearch = minLongestSearch;
        }

        public IState FinalState => States.Last();

        public int GameDepth { get; }

        public List<IState> States { get; }

        public TimeSpan MaxTotalTime { get; }

        public TimeSpan MinTotalTime { get; }

        public TimeSpan MaxLongestSearch { get;  }

        public TimeSpan MinLongestSearch { get; }
    }
}
