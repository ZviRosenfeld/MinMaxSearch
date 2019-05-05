using System;
using System.Collections.Generic;
using MinMaxSearch;

namespace Connect4Tests
{
    /// <summary>
    /// This class represents a probabilistic version of connect 4.
    /// This version is the same as the regaler connect 4, with the addition that the player rolls a dice before every move.
    /// Then, the player can only place a game-piece in a location larger then what he rolled minus one (so if he rolled one he can put a piece anywhere, and if he rolled six he can only put the piece in the sixth row).
    /// </summary>
    class ProbabilisticConnect4State : IProbabilisticState
    {
        private readonly Connect4State connect4State;

        public ProbabilisticConnect4State(Connect4State connect4State)
        {
            this.connect4State = connect4State;
        }

        public double Evaluate(int depth, List<IState> passedThroughStates) =>
            connect4State.Evaluate(depth, passedThroughStates);

        public Player Turn => connect4State.Turn;

        public IEnumerable<Tuple<double, List<IState>>> GetNeighbors()
        {
            if (BoardEvaluator.IsWin(connect4State.Board, Utils.GetReversePlayer(Turn)))
                return new List<Tuple<double, List<IState>>>();

            var result = new List<Tuple<double, List<IState>>>();
            for (int i = 0; i < Connect4State.BoardSize; i++)
                result.Add(new Tuple<double, List<IState>>(1.0 / Connect4State.BoardSize, GetNeighborsFrom(i)));

            return result;
        }

        public Player[,] Board => connect4State.Board;

        private List<IState> GetNeighborsFrom(int start)
        {
            var result = new List<IState>();
            for (int i = start; i <Connect4State.BoardSize; i++)
            {
                var newState = connect4State.AddPieceTo(i);
                if (newState != null)
                    result.Add(new ProbabilisticConnect4State(newState));
            }

            return result;
        }

        public override string ToString() => connect4State.ToString();

        public override bool Equals(object obj)
        {
            if (!(obj is ProbabilisticConnect4State probabilisticConnect4State))
                return false;

            return connect4State.Equals(probabilisticConnect4State.connect4State);
        }

        public override int GetHashCode() => connect4State.GetHashCode();
    }
}

