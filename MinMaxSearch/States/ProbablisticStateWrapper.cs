using System;
using System.Collections.Generic;

namespace MinMaxSearch
{
    class ProbablisticStateWrapper : IDeterministicState
    {
        private IEnumerable<IState> neighbors;

        public ProbablisticStateWrapper(Player turn, IEnumerable<IState> neighbors)
        {
            Turn = turn;
            this.neighbors = neighbors;
        }
        
        public double Evaluate(int depth, List<IState> passedThroughStates) => 
            throw new Exception("This should never be called");

        public Player Turn { get; }
        public IEnumerable<IState> GetNeighbors() => neighbors;
    }
}
