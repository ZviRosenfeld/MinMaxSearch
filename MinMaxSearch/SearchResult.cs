using System.Collections.Generic;

namespace MinMaxSearch
{
    public class SearchResult
    {
        public SearchResult(IState nextMove, double evaluation, List<IState> stateSequence, int leaves, int intarnalNodes)
        {
            NextMove = nextMove;
            Evaluation = evaluation;
            StateSequence = stateSequence;
            Leaves = leaves;
            IntarnalNodes = intarnalNodes;
        }

        public IState NextMove { get; }
        public double Evaluation { get; }
        public List<IState> StateSequence { get; }
        public int Leaves { get; }
        public int IntarnalNodes { get; }
    }
}
