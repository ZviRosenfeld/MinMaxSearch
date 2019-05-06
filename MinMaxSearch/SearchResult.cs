using System.Collections.Generic;
using System.Linq;

namespace MinMaxSearch
{
    public class SearchResult
    {
        public SearchResult(double evaluation, List<IState> stateSequence, int leaves, int internalNodes, bool allChildrenAreDeadEnds)
        {
            Evaluation = evaluation;
            StateSequence = stateSequence;
            Leaves = leaves;
            InternalNodes = internalNodes;
            AllChildrenAreDeadEnds = allChildrenAreDeadEnds;
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
    }
}
