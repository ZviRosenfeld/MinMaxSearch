using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch.ThreadManagment;

namespace MinMaxSearch.UnitTests.ThreadManagment
{
    [TestClass]
    [TestCategory("ThreadManager")]
    public class FirstLevelOnlyThreadManagerTests
    {
        [TestMethod]
        public void Invoke_MaxDegreeOfParallelismIsOne_DontRunParallel() =>
            ThreadManagmentTestUtils.TestThatThreadsRunInSequence(new LevelParallelismThreadManager(1));

        [TestMethod]
        public void Invoke_MaxDegreeOfParallelismIsGreaterThenThreads_RunAllThreadsParallel() =>
            ThreadManagmentTestUtils.TestThatAllThreadsRunInParallel(new LevelParallelismThreadManager(1));

        [TestMethod]
        [ExpectedException(typeof(MinMaxSearchException))]
        [DataRow(-1)]
        [DataRow(0)]
        public void LevelParallelismThreadManager_NagitaveLevel_ThrowException(int level) =>
            new LevelParallelismThreadManager(level);
    }
}
