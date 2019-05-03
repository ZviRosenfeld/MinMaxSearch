using System.Collections.Generic;
using System.Linq;

namespace MinMaxSearch
{
    public class SearchResult
    {
        public SearchResult(double evaluation, List<IState> stateSequence, int leaves, int internalNodes)
        {
            Evaluation = evaluation;
            StateSequence = stateSequence;
            Leaves = leaves;
            InternalNodes = internalNodes;
        }

        public IDeterministicState NextMove => (IDeterministicState) StateSequence.First(s => s is IDeterministicState);
        public double Evaluation { get; }
        public List<IState> StateSequence { get; }
        public int Leaves { get; }
        public int InternalNodes { get; }
    }
}
