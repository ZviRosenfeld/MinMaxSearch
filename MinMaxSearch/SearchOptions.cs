using System;
using System.Collections.Generic;
using MinMaxSearch.Cache;
using MinMaxSearch.Pruners;

namespace MinMaxSearch
{
    public class SearchOptions
    {
        public SearchOptions(List<IPruner> pruners, Func<IState, int, List<IState>, bool> isUnstableState,
            bool preventLoops, bool favorShortPaths, bool dieEarly, double maxScore,
            double minScore, Func<IState, int, List<IState>, double> alternateEvaluation, bool stateDefinesDepth, CacheMode cacheMode)
        {
            Pruners = pruners;
            IsUnstableState = isUnstableState;
            FavorShortPaths = favorShortPaths;
            DieEarly = dieEarly;
            MaxScore = maxScore;
            MinScore = minScore;
            AlternateEvaluation = alternateEvaluation;
            StateDefinesDepth = stateDefinesDepth;
            CacheMode = cacheMode;

            var preventLoopPruner = new PreventLoopPruner();
            if (preventLoops && !pruners.Contains(preventLoopPruner))
                    pruners.Add(preventLoopPruner);
        }

        public List<IPruner> Pruners { get; }

        public Func<IState, int, List<IState>, bool> IsUnstableState { get; }

        public Func<IState, int, List<IState>, double> AlternateEvaluation { get; }

        public bool FavorShortPaths { get; }

        public bool DieEarly { get; }
        
        public double MaxScore { get; }

        public double MinScore { get; }

        public bool StateDefinesDepth { get; }

        public CacheMode CacheMode { get; }
    }
}
