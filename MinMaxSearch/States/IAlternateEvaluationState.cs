using System.Collections.Generic;

namespace MinMaxSearch.States
{
    /// <summary>
    /// If your states implement IAlternateEvaluationState then any time min started the search we'll use the IAlternateEvaluationState.AlternateEvaluation
    /// to evaluate the states rather then the IState.Evaluation method.
    /// </summary>
    public interface IAlternateEvaluationState
    {
        /// <summary>
        /// returns an alternate evaluation of the state (how good it is).
        /// This method will be called to evaluate states when the search starts with min for any state that implements IAlternateEvaluationState.
        /// AlternateEvaluation must return a value smaller then double.MaxValue and greater then double.MinValue
        /// </summary>
        double AlternateEvaluation(int depth, List<IState> passedThroughStates);
    }
}
