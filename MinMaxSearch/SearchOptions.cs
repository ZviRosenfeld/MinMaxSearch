using System;
using System.Collections.Generic;
using MinMaxSearch.Pruners;

namespace MinMaxSearch
{
    class SearchOptions
    {
        public SearchOptions(List<IPruner> pruners, Func<IState, int, List<IState>, bool> isUnstableState,
            bool preventLoops, bool favorShortPaths, bool dieEarly, bool rememberDeadEndStates, double maxScore,
            double minScore, int maxDegreeOfParallelism)
        {
            Pruners = pruners;
            IsUnstableState = isUnstableState;
            FavorShortPaths = favorShortPaths;
            DieEarly = dieEarly;
            RememberDeadEndStates = rememberDeadEndStates;
            MaxScore = maxScore;
            MinScore = minScore;
            MaxDegreeOfParallelism = maxDegreeOfParallelism >= 1
                ? maxDegreeOfParallelism
                : throw new BadDegreeOfParallelismException(
                    "DegreeOfParallelism must be at least one. Tried to set it to " + maxDegreeOfParallelism);

            var preventLoopPruner = new PreventLoopPruner();
            if (preventLoops && !pruners.Contains(preventLoopPruner))
                    pruners.Add(preventLoopPruner);
        }

        public List<IPruner> Pruners { get; }

        public Func<IState, int, List<IState>, bool> IsUnstableState { get; }
        
        public bool FavorShortPaths { get; }

        public bool DieEarly { get; }

        public bool RememberDeadEndStates { get; }

        public double MaxScore { get; }

        public double MinScore { get; }
        
        public int MaxDegreeOfParallelism { get; }
    }
}
