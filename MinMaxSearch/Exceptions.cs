using System;

namespace MinMaxSearch
{
    public class MinMaxSearchException : ApplicationException
    {
        public MinMaxSearchException(string message) : base(message)
        { 
        }
    }

    public class NoNeighborsException : MinMaxSearchException
    {
        public NoNeighborsException(string message) : base(message)
        {
        }
    }

    public class EmptyPlayerException : MinMaxSearchException
    {
        public EmptyPlayerException(string message) : base(message)
        {
        }
    }

    public class BadDegreeOfParallelismException : MinMaxSearchException
    {
        public BadDegreeOfParallelismException(string message) : base(message)
        {
        }
    }

    public class BadStateTypeException : MinMaxSearchException
    {
        public BadStateTypeException(string message) : base(message)
        {
        }
    }

    public class InternalException : MinMaxSearchException {
        public InternalException(string message) : base(message)
        {
        }
    }
}
