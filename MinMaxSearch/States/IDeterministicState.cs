using System.Collections.Generic;

namespace MinMaxSearch
{
    public interface IDeterministicState : IState
    {
        IEnumerable<IState> GetNeighbors();
    }
}
