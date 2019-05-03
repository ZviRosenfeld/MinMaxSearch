using System.Collections.Generic;

namespace MinMaxSearch
{
    public interface IDeterministicState : IState
    {
        IEnumerable<IState> GetNeighbors();

        double Evaluate(int depth, List<IState> passedThroughStates);

        Player Turn { get; }
    }
}
