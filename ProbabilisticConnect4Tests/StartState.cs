using System.Collections.Generic;
using System.Linq;
using Connect4Tests;
using MinMaxSearch;

namespace ProbabilisticConnect4Tests
{
    class StartState : IDeterministicState
    {
        private readonly Connect4State connect4State;

        public StartState(Connect4State connect4State)
        {
            this.connect4State = connect4State;
        }

        public double Evaluate(int depth, List<IState> passedThroughStates) =>
            connect4State.Evaluate(depth, passedThroughStates);

        public Player Turn => connect4State.Turn;

        public IEnumerable<IState> GetNeighbors() => 
            connect4State.GetNeighbors().Select(s => new ProbabilisticConnect4State((Connect4State) s));

        public override string ToString() => connect4State.ToString();
    }
}
