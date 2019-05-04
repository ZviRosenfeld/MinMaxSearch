using System;
using System.Collections.Generic;

namespace MinMaxSearch
{
    public interface IProbabilisticState : IState
    {
        IEnumerable<Tuple<double, List<IState>>> GetNeighbors();
    }
}
