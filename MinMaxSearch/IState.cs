using System.Collections.Generic;

namespace MinMaxSearch
{
    public interface IState
    {
        IEnumerable<IState> GetNeighbors();

        double Evaluate(int depth, List<IState> passedThroughStates);
    }
}
