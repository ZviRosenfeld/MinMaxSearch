using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch.ThreadManagment;

namespace MinMaxSearch.UnitTests.ThreadManagment
{
    [TestClass]
    [TestCategory("ThreadManager")]
    public class FirstLevelOnlyThreadManagerTests
    {
        [TestMethod]
        public void Invoke_MaxDegreeOfParallelismIsOne_DontRunParallel()
        {
            ThreadManagmentTestUtils.TestThatThreadsRunInSequence(new FirstLevelOnlyThreadManager());
        }

        [TestMethod]
        public void Invoke_MaxDegreeOfParallelismIsGreaterThenThreads_RunAllThreadsParallel()
        {
            ThreadManagmentTestUtils.TestThatAllThreadsRunInParallel(new FirstLevelOnlyThreadManager());
        }
    }
}
