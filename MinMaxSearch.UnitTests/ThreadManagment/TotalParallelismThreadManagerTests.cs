using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch.Exceptions;
using MinMaxSearch.ThreadManagment;

namespace MinMaxSearch.UnitTests.ThreadManagment
{
    [TestClass]
    [TestCategory("ThreadManager")]
    public class TotalParallelismThreadManagerTests
    {
        [TestMethod]
        public void Invoke_MaxDegreeOfParallelismIsOne_DontRunParallel() =>
            ThreadManagmentTestUtils.TestThatThreadsRunInSequence(new TotalParallelismThreadManager(1, 1000));

        [TestMethod]
        public void Invoke_MaxDegreeOfParallelismIsGreaterThenThreads_RunAllThreadsParallel()
        {
            const int degreeOfParallelism = 10;
            var manager = new TotalParallelismThreadManager(degreeOfParallelism, 1000);

            ThreadManagmentTestUtils.TestThatAllThreadsRunInParallel(manager);
        }
        
        [DataRow(4)]
        [TestMethod]
        public void Invoke_CheckThatOnlyMaxDegreeOfParallelismThreadsRunParallel(int degreeOfParallelism)
        {
            var startedThreads = new List<int>();
            var manager = new TotalParallelismThreadManager(degreeOfParallelism, 1000);

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

        [TestMethod]
        [ExpectedException(typeof(BadDegreeOfParallelismException))]
        [DataRow(-1)]
        [DataRow(0)]
        public void LevelParallelismThreadManager_NagitaveParallelismDegree_ThrowException(int degree) =>
            new TotalParallelismThreadManager(degree, 10);

        [TestMethod]
        [ExpectedException(typeof(InternalException), "Code 1001")]
        [DataRow(-1)]
        [DataRow(0)]
        public void LevelParallelismThreadManager_NagitaveMaxSearchDepth_ThrowException(int depth) =>
            new TotalParallelismThreadManager(2, depth);
    }
}
