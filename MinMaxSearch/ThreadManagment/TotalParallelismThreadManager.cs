using System;
using System.Threading;
using System.Threading.Tasks;
using MinMaxSearch.Exceptions;

namespace MinMaxSearch.ThreadManagment
{
    public class TotalParallelismThreadManager : IThreadManager
    {
        private readonly object threadLock = new object();
        private int threadsRunning = 1;
        private readonly int maxDegreeOfParallelism;
        private readonly int maxSearchDepth;

        public TotalParallelismThreadManager(int maxDegreeOfParallelism, int maxSearchDepth)
        {
            if (maxDegreeOfParallelism <= 0)
                throw new BadDegreeOfParallelismException(
                    $"{nameof(maxDegreeOfParallelism)} must be at least one. Tried to set it to {maxDegreeOfParallelism}");

            if (maxSearchDepth <= 0)
                throw new InternalException(
                    $"Code 1001 (in {nameof(TotalParallelismThreadManager)}: {nameof(maxSearchDepth)} is {maxSearchDepth})");

            this.maxDegreeOfParallelism = maxDegreeOfParallelism;
            this.maxSearchDepth = maxSearchDepth;
        }

        public Task<T> Invoke<T>(Func<T> func, int depth)
        {
            bool startNewThread;
            if (depth == maxSearchDepth - 1)
            {
                // It's a waste of resources to start a new thread so close to the bottom of the search tree.
                // But if depth >= maxSearchDepth it means we're in an unstable state and don't know for how much deeper the tree will get, so we should start a new thread.
                startNewThread = false;
            }
            else
                lock (threadLock)
                {
                    startNewThread = threadsRunning < maxDegreeOfParallelism;
                    if (startNewThread)
                        Interlocked.Increment(ref threadsRunning);
                }

            if (startNewThread)
            {
                return Task.Run(() =>
                {
                    try
                    {
                        return func();
                    }
                    finally
                    {
                        Interlocked.Decrement(ref threadsRunning);
                    }
                });
            }
            return Task.FromResult(func());
        }
    }
}
