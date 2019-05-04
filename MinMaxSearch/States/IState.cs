using System.Collections.Generic;

namespace MinMaxSearch
{
    public interface IState
    {
        double Evaluate(int depth, List<IState> passedThroughStates);

        Player Turn { get; }
    }
}
