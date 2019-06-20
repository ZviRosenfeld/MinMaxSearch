using System;
using System.Collections.Generic;
using System.Linq;

namespace MinMaxSearch
{
    public class SearchResult
    {
        public SearchResult(double evaluation, List<IState> stateSequence, int leaves, int internalNodes,
            bool allChildrenAreDeadEnds)
        {
            Evaluation = evaluation;
            StateSequence = stateSequence;
            Leaves = leaves;
            InternalNodes = internalNodes;
            AllChildrenAreDeadEnds = allChildrenAreDeadEnds;
            SearchTime = TimeSpan.Zero;
        }

        public SearchResult(double evaluation, IState endState)
        {
            Evaluation = evaluation;
            StateSequence = new List<IState> {endState};
            Leaves = 1;
            InternalNodes = 0;
            AllChildrenAreDeadEnds = true;
            SearchTime = TimeSpan.Zero;
        }

        public SearchResult(SearchResult other, TimeSpan searchTime)
        {
            Evaluation = other.Evaluation;
            StateSequence = other.StateSequence.ToList();
            Leaves = other.Leaves;
            InternalNodes = other.InternalNodes;
            AllChildrenAreDeadEnds = other.AllChildrenAreDeadEnds;
            SearchTime = searchTime;
        }

        public IState NextMove => StateSequence.First();

        public double Evaluation { get; }
        
        /// <summary>
        /// Note that this will be empty for probabilistic states 
        /// </summary>
        public List<IState> StateSequence { get; }

        public int Leaves { get; }

        public int InternalNodes { get; }

        public bool AllChildrenAreDeadEnds { get; }

        public TimeSpan SearchTime { get; }
    }
}
