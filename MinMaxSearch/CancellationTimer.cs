using System;
using System.Threading;

namespace MinMaxSearch
{
    class CancellationTimer : IDisposable
    {
        private Timer timeoutTimer;
        private readonly TimeSpan? timeOut;

        public CancellationTimer(TimeSpan? timeOut)
        {
            this.timeOut = timeOut;
        }
        
        public CancellationToken GetCancellationToken(CancellationToken originalToken)
        {
            if (timeOut != null)
            {
                var cancellationSource = CancellationTokenSource.CreateLinkedTokenSource(originalToken);
                timeoutTimer = new Timer(c => cancellationSource.Cancel(), null, timeOut.Value, TimeSpan.MaxValue);
                return cancellationSource.Token;
            }

            return originalToken;
        }

        public void Dispose()
        {
            timeoutTimer?.Dispose();
        }
    }
}
