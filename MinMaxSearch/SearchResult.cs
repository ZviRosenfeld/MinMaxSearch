using System.Collections.Generic;
using System.Linq;

namespace MinMaxSearch
{
    public class SearchResult
    {
        public SearchResult(double evaluation, List<IState> stateSequence, int leaves, int intarnalNodes, bool deadEnd)
        {
            Evaluation = evaluation;
            StateSequence = stateSequence;
            Leaves = leaves;
            IntarnalNodes = intarnalNodes;
            DeadEnd = deadEnd;
        }

        public IState NextMove => StateSequence.First();
        public double Evaluation { get; }
        public List<IState> StateSequence { get; }
        public int Leaves { get; }
        public int IntarnalNodes { get; }
        public bool DeadEnd { get; }
    }
}
