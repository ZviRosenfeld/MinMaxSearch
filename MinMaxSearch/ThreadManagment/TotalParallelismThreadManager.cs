using System;
using System.Threading;
using System.Threading.Tasks;

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
            this.maxDegreeOfParallelism = maxDegreeOfParallelism;
            this.maxSearchDepth = maxSearchDepth;
        }

        public Task<T> Invoke<T>(Func<T> func, int depth)
        {
            bool startNewThread;
            if (depth == maxSearchDepth - 1)
            {
                // It's a waste of resources to start a new thread so close to the buttom of the search tree.
                // But if depth >= maxSearchDepth it means where in an unstable state and don't know for how much deeper the tree will get, so we should start a new thread.
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
