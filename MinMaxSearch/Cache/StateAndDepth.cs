namespace MinMaxSearch.Cache
{
    public class StateAndDepth
    {
        public StateAndDepth(IState state, int depth)
        {
            State = state;
            Depth = depth;
        }

        public IState State { get; }

        public int Depth { get; }

        public override int GetHashCode() => State.GetHashCode() + Depth.GetHashCode();

        public override bool Equals(object obj) =>
            obj is StateAndDepth other &&
            State.Equals(other.State) &&
            Depth == other.Depth;
    }
}
