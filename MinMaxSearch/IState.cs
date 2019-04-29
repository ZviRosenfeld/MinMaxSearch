using System.Collections.Generic;

namespace MinMaxSearch
{
    public interface IState
    {
        IEnumerable<IState> GetNeighbors();

        /// <param name="passedThroughStates">This list will only be filled if the RecordPassThrough flag is on in the SearchEngine</param>
        double Evaluate(int depth, List<IState> passedThroughStates);
    }
}
