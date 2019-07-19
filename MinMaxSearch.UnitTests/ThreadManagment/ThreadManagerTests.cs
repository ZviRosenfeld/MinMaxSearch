using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch.ThreadManagment;

namespace MinMaxSearch.UnitTests.ThreadManagment
{
    [TestClass]
    [TestCategory("ThreadManager")]
    public class ThreadManagerTests
    {
        private Func<int> slowAction = (() =>
        {
            Thread.Sleep(2000);
            return 1;
        });

        [TestMethod]
        public void Invoke_MaxDegreeOfParallelismIsOne_DontRunParallel()
        {
            var results = new List<Task<int>>();
            var manager = new ThreadManager(1);

            for (int i = 0; i < 3; i++)
                results.Add(manager.Invoke<int>(slowAction, 1));
            manager.Invoke(() => 1, 1).Wait();

            foreach (var result in results)
                Assert.IsTrue(result.IsCompleted, "All tasks should have finished");
        }

        [TestMethod]
        public void Invoke_MaxDegreeOfParallelismIsGreaterThenThreads_RunAllThreadsParallel()
        {
            const int degreeOfParallelism = 10;
            var results = new List<Task<int>>();
            var manager = new ThreadManager(degreeOfParallelism);

            for (int i = 0; i < 3; i++)
                results.Add(manager.Invoke<int>(slowAction, 1));
            manager.Invoke(() => 1, 1).Wait();

            foreach (var result in results)
                Assert.IsFalse(result.IsCompleted, "The tasks shouldn't have finished yet");
        }
        
        [DataRow(4)]
        [TestMethod]
        public void Invoke_CheckThatOnlyMaxDegreeOfParallelismThreadsRunParallel(int degreeOfParallelism)
        {
            var startedThreads = new List<int>();
            var manager = new ThreadManager(degreeOfParallelism);

            Task.Run(() =>
            {
                for (int i = 0; i < 50; i++)
                    manager.Invoke<int>(() =>
                    {
                        startedThreads.Add(1);
                        return slowAction();
                    }, 1);
            });
            Thread.Sleep(100);

            Assert.AreEqual(degreeOfParallelism, startedThreads.Count);
        }
    }
}
