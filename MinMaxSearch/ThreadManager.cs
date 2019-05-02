using System;
using System.Threading;
using System.Threading.Tasks;

namespace MinMaxSearch
{
    public class ThreadManager
    {
        private readonly object threadLock = new object();
        private int threadsRunning = 1;
        private readonly int maxDegreeOfParallelism;

        public ThreadManager(int maxDegreeOfParallelism)
        {
            this.maxDegreeOfParallelism = maxDegreeOfParallelism;
        }

        public Task<T> Invoke<T>(Func<T> func)
        {
            bool startNewThread;
            lock (threadLock)
            {
                startNewThread = threadsRunning < maxDegreeOfParallelism;
            }

            if (startNewThread)
            {
                Interlocked.Increment(ref threadsRunning);
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
