using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MinMaxSearch.UnitTests
{
    [TestClass]
    [TestCategory("CancellationTimer")]
    public class CancellationTimerTests
    {
        private Action<CancellationToken> runTillCanceledAction = token =>
        {
            while (true)
                if (token.IsCancellationRequested) break;
        };

        [TestMethod]
        public void CheckThatThreadCancelledWhenTimeIsUp()
        {
            using (var timer = new CancellationTimer(TimeSpan.FromMilliseconds(100)))
            {
                var token = timer.GetCancellationToken(CancellationToken.None);
                var task = Task.Run(() => runTillCanceledAction(token));
                Thread.Sleep(50);
                Assert.IsTrue(task.Status == TaskStatus.Running, "Task shouldn't have been canceled yet");
                Thread.Sleep(100);
                Assert.IsFalse(task.Status == TaskStatus.Running, "Task should have been canceled by now");
            }
        }

        [TestMethod]
        public void CheckThatOriginalCancellationTokenStillworks()
        {
            using (var timer = new CancellationTimer(TimeSpan.FromMilliseconds(100)))
            {
                var cancellationSource = new CancellationTokenSource();
                var token = timer.GetCancellationToken(cancellationSource.Token);
                var task = Task.Run(() => runTillCanceledAction(token));
                cancellationSource.Cancel();
                Thread.Sleep(50);
                Assert.IsFalse(task.Status == TaskStatus.Running, "Task should have been canceled");
            }
        }
    }
}
