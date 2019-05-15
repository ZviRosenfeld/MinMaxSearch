using System.Collections.Generic;

namespace MinMaxSearch
{
    class ProbablisticStateWrapper : IDeterministicState
    {
        private readonly IEnumerable<IState> neighbors;
        private readonly Player startPlayer;

        public IProbabilisticState InnerState { get; }

        public ProbablisticStateWrapper(IEnumerable<IState> neighbors, IProbabilisticState innerState, Player startPlayer)
        {
            this.neighbors = neighbors;
            InnerState = innerState;
            this.startPlayer = startPlayer;
        }

        public double Evaluate(int depth, List<IState> passedThroughStates) =>
            InnerState.Evaluate(depth, passedThroughStates);

        public Player Turn => InnerState.Turn;

        public IEnumerable<IState> GetNeighbors() => neighbors;
    }
}
