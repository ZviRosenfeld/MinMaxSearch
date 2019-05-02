using System;
using System.Threading;
using System.Threading.Tasks;

namespace MinMaxSearch
{
    class ThreadManager
    {
        private readonly object threadLock = new object();
        private int threadsRunning = 1;
        private readonly int maxDegreeOfParallelism;

        public ThreadManager(int maxDegreeOfParallelism)
        {
            this.maxDegreeOfParallelism = maxDegreeOfParallelism;
        }

        public Task<T> Invoke<T>(Func<T> action, CancellationToken cancellationToken)
        {
            lock (threadLock)
            {
                if (threadsRunning < maxDegreeOfParallelism)
                {
                    threadsRunning++;
                    return Task.Run(() =>
                    {
                        try
                        {
                            return action();
                        }
                        finally
                        {
                            threadsRunning--;
                        }
                    }, cancellationToken);
                }
            }
            var result = action();
            return Task.FromResult(result);
        }
    }
}
