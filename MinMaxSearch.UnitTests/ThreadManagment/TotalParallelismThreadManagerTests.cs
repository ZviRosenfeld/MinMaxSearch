using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch.ThreadManagment;

namespace MinMaxSearch.UnitTests.ThreadManagment
{
    [TestClass]
    [TestCategory("ThreadManager")]
    public class TotalParallelismThreadManagerTests
    {
        [TestMethod]
        public void Invoke_MaxDegreeOfParallelismIsOne_DontRunParallel()
        {
            ThreadManagmentTestUtils.TestThatThreadsRunInSequence(new TotalParallelismThreadManager(1));
        }

        [TestMethod]
        public void Invoke_MaxDegreeOfParallelismIsGreaterThenThreads_RunAllThreadsParallel()
        {
            const int degreeOfParallelism = 10;
            var manager = new TotalParallelismThreadManager(degreeOfParallelism);

            ThreadManagmentTestUtils.TestThatAllThreadsRunInParallel(manager);
        }
        
        [DataRow(4)]
        [TestMethod]
        public void Invoke_CheckThatOnlyMaxDegreeOfParallelismThreadsRunParallel(int degreeOfParallelism)
        {
            var startedThreads = new List<int>();
            var manager = new TotalParallelismThreadManager(degreeOfParallelism);

            Task.Run(() =>
            {
                for (int i = 0; i < 50; i++)
                    manager.Invoke(() =>
                    {
                        startedThreads.Add(1);
                        return ThreadManagmentTestUtils.slowAction();
                    }, 1);
            });
            Thread.Sleep(100);

            Assert.AreEqual(degreeOfParallelism, startedThreads.Count);
        }
    }
}
