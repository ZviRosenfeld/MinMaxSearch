using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch.Exceptions;
using MinMaxSearch.ThreadManagment;

namespace MinMaxSearch.UnitTests.ThreadManagment
{
    [TestClass]
    [TestCategory("ThreadManager")]
    public class FirstLevelOnlyThreadManagerTests
    {
        [TestMethod]
        [DataRow(1, 1)]
        [DataRow(3, 2)]
        public void Invoke_DepthGreaterThanMaxLevel_DontRunParallel(int depth, int levels) =>
            ThreadManagmentTestUtils.TestThatThreadsRunInSequence(new LevelParallelismThreadManager(levels), depth);

        [TestMethod]
        [DataRow(0, 1)]
        [DataRow(2, 3)]
        public void Invoke_DepthEqualeMaxLevel_RunAllThreadsParallel(int depth, int levels) =>
            ThreadManagmentTestUtils.TestThatAllThreadsRunInParallel(new LevelParallelismThreadManager(levels), depth);

        [TestMethod]
        [ExpectedException(typeof(MinMaxSearchException))]
        [DataRow(-1)]
        [DataRow(0)]
        public void LevelParallelismThreadManager_NagitaveLevel_ThrowException(int level) =>
            new LevelParallelismThreadManager(level);
    }
}
