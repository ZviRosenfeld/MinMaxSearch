using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch.ThreadManagment;

namespace MinMaxSearch.UnitTests.ThreadManagment
{
    class ThreadManagmentTestUtils
    {
        public static Func<int> slowAction = (() =>
        {
            Thread.Sleep(2000);
            return 1;
        });

        public static void TestThatThreadsRunInSequence(IThreadManager manager, int depth = 1)
        {
            var results = new List<Task<int>>();

            for (int i = 0; i < 3; i++)
                results.Add(manager.Invoke(slowAction, depth));
            manager.Invoke(() => 1, 1).Wait();

            foreach (var result in results)
                Assert.IsTrue(result.IsCompleted, "All tasks should have finished");
        }

        public static void TestThatAllThreadsRunInParallel(IThreadManager manager, int depth = 0)
        {
            var results = new List<Task<int>>();
            for (int i = 0; i < 3; i++)
                results.Add(manager.Invoke(slowAction, depth));
            manager.Invoke(() => 1, 1).Wait();

            foreach (var result in results)
                Assert.IsFalse(result.IsCompleted, "The tasks shouldn't have finished yet");
        }
    }
}
