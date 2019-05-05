﻿using System;
using System.Collections.Generic;

namespace MinMaxSearch
{
    class ProbablisticStateWrapper : IDeterministicState
    {
        private IEnumerable<IState> neighbors;
        private IProbabilisticState innerState;

        public ProbablisticStateWrapper(IEnumerable<IState> neighbors, IProbabilisticState innerState)
        {
            this.neighbors = neighbors;
            this.innerState = innerState;
        }

        public double Evaluate(int depth, List<IState> passedThroughStates) => innerState.Evaluate(depth, passedThroughStates);

        public Player Turn => innerState.Turn;
        public IEnumerable<IState> GetNeighbors() => neighbors;
    }
}