using System.Collections.Generic;
using System.Linq;

namespace MinMaxSearch.Cache
{
    class StateAndPassedThroughStates
    {
        public StateAndPassedThroughStates(IState state, IList<IState> passedThroughStates)
        {
            State = state;
            PassedThroughStates = passedThroughStates;
        }

        public IState State { get; }

        public IList<IState> PassedThroughStates { get; }

        public override bool Equals(object obj)
        {
            if (!(obj is StateAndPassedThroughStates other))
                return false;

            if (!State.Equals(other.State))
                return false;

            if (PassedThroughStates.Count != other.PassedThroughStates.Count)
                return false;

            for (int i = 0; i < PassedThroughStates.Count; i++)
                if (!PassedThroughStates[i].Equals(other.PassedThroughStates[i]))
                    return false;

            return true;
        }

        public override int GetHashCode() => 
            State.GetHashCode() + PassedThroughStates.Sum(passedThroughState => passedThroughState.GetHashCode());

        public override string ToString() =>
            $"{nameof(State)} = {State}; {nameof(PassedThroughStates)} Count = {PassedThroughStates.Count}";
    }
}
