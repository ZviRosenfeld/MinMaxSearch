using System.Collections.Generic;
using CheckersTests;

namespace MinMaxSearch.Samples
{
    /// <summary>
    /// An example of using an IsUnstableState delegate.
    /// This delegate works on a Checkers state. It returns true if there is an available jump.
    /// </summary>
    class IsUnstableStateSample
    {
        public ISearchEngine GetEngine()
        {
            var engine = new SearchEngine
            {
                IsUnstableState = IsUnstableCheckersState
            };
            return engine;
        }

        private bool IsUnstableCheckersState(IState state, int depth, List<IState> passedThroghStates)
        {
            var checkersState = (CheckersState) state;
            return checkersState.CanJump();
        }
    }
}
