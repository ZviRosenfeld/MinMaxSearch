using System;
using System.Collections.Generic;

namespace MinMaxSearch
{
    public interface IProbabilisticState : IState
    {
        /// <summary>
        /// returns a tuple containing a probability, and a list of neighbors for that probability
        /// Note that a win state shouldn't return any neighbors.
        /// </summary>
        IEnumerable<Tuple<double, IEnumerable<IState>>> GetNeighbors();
    }
}
