using System;

namespace MinMaxSearch
{
    public class NoNeighborsException : ApplicationException
    {
        public NoNeighborsException(IState startState) : base("start state has no nighbors " + startState)
        {
        }
    }

    public class EmptyPlayerException : ApplicationException
    {
        public EmptyPlayerException(string message) : base(message)
        {
        }
    }
}
