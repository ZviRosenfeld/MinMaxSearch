using System.Collections.Generic;

namespace MinMaxSearch
{
    public interface IDeterministicState : IState
    {
        /// <summary>
        /// returns a list of the state's neighbors. Note that a win state shouldn't return any neighbors.
        /// </summary>
        IEnumerable<IState> GetNeighbors();
    }
}
