using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MinMaxSearch.UnitTests
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
                results.Add(manager.Invoke(slowAction));

            foreach (var result in results)
                Assert.IsTrue(result.IsCompleted, "All tasks should have finished");
        }

        [TestMethod]
        public void Invoke_MaxDegreeOfParallelismIsFour_RunParallel()
        {
            const int degreeOfParallelism = 5;
            var results = new List<Task<int>>();
            var manager = new ThreadManager(degreeOfParallelism);

            for (int i = 0; i < degreeOfParallelism - 1; i++)
                results.Add(manager.Invoke(slowAction));

            foreach (var result in results)
                Assert.IsFalse(result.IsCompleted, "The tasks shouldn't have finished yet");
        }

        [DataRow(2)]
        [DataRow(8)]
        [TestMethod]
        public void Invoke_CheckThatOnlyMaxDegreeOfParallelismThreadsRunParallel(int degreeOfParallelism)
        {
            var results = new List<Task<int>>();
            var manager = new ThreadManager(degreeOfParallelism);

            Task.Run(() =>
            {
                for (int i = 0; i < 50; i++)
                    results.Add(manager.Invoke(slowAction));
            });
            Thread.Sleep(100);

            Assert.AreEqual(degreeOfParallelism - 1, results.Count);
        }
    }
}
