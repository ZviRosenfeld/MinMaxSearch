using System.Collections.Generic;

namespace MinMaxSearch.Pruners
{
    /// <summary>
    /// The Pruners are called before examining every node, to determine if we want to prune it
    /// </summary>
    public interface IPruner
    {
        /// <param name="passedThroughStates">This list will only be filled if the RecordPassThrough flag is on in the SearchEngine</param>
        bool ShouldPrune(IState state, int depth, List<IState> passedThroughStates);
    }
}
