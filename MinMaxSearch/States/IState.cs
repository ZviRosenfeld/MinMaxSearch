using System.Collections.Generic;

namespace MinMaxSearch
{
    public interface IState
    {
        /// <summary>
        /// returns the state's evaluation (how good it is).
        /// </summary>
        double Evaluate(int depth, List<IState> passedThroughStates);

        /// <summary>
        /// values can be Player.Max and Player.Min (Player is an enum in the code). 
        /// Max is the player trying to get the best score, while Min is the player trying to get the worst score.
        /// While most in most games, turns will alternate between Max and Min, you can really implement any order you want.
        /// </summary>
        Player Turn { get; }
    }
}
