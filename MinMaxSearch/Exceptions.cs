using System;

namespace MinMaxSearch
{
    public class NoNeighborsException : ApplicationException
    {
        public NoNeighborsException(string message) : base(message)
        {
        }
    }

    public class EmptyPlayerException : ApplicationException
    {
        public EmptyPlayerException(string message) : base(message)
        {
        }
    }

    public class BadDegreeOfParallelismException : ApplicationException
    {
        public BadDegreeOfParallelismException(string message) : base(message)
        {
        }
    }

    public class BadStateTypeException : ApplicationException
    {
        public BadStateTypeException(string message) : base(message)
        {
        }
    }
}
