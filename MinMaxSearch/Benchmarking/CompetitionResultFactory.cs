using System;
using System.Collections.Generic;

namespace MinMaxSearch.Benchmarking
{
    class CompetitionResultFactory
    {
        private int gameDepth = 0;
        private readonly List<IState> states = new List<IState>();
        private TimeSpan maxTotalTime = TimeSpan.Zero;
        private TimeSpan minTotalTime = TimeSpan.Zero;
        private TimeSpan maxLongestSearch = TimeSpan.Zero;
        private TimeSpan minLongestSearch = TimeSpan.Zero;

        public CompetitionResult GetCompetitionResult() =>
            new CompetitionResult(gameDepth, states, maxTotalTime, minTotalTime, maxLongestSearch, minLongestSearch);

        public void AddState(IState state)
        {
            states.Add(state);
            gameDepth++;
        }

        public void AddTime(TimeSpan searchTime, Player player)
        {
            if (player == Player.Max)
            {
                maxTotalTime += searchTime;
                if (searchTime > maxLongestSearch)
                    maxLongestSearch = searchTime;
            }
            else
            {
                minTotalTime += searchTime;
                if (searchTime > minLongestSearch)
                    minLongestSearch = searchTime;
            }
        }
    }
}
