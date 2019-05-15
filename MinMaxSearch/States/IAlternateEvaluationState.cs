using System.Collections.Generic;

namespace MinMaxSearch.States
{
    interface IAlternateEvaluationState
    {
        /// <summary>
        /// returns an alternate evaluation of the state  (how good it is).
        /// AlternateEvaluation must return a value smaller then double.MaxValue and greater then double.MinValue
        /// </summary>
        double AlternateEvaluation(int depth, List<IState> passedThroughStates);
    }
}
