using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch.ThreadManagment;

namespace MinMaxSearch.UnitTests.ThreadManagment
{
    [TestClass]
    [TestCategory("ThreadManager")]
    public class SequencelThreadManagerTests
    {
        [TestMethod]
        public void Invoke_MaxDegreeOfParallelismIsOne_DontRunParallel()
        {
            ThreadManagmentTestUtils.TestThatThreadsRunInSequence(new SequencelThreadManager());
        }
    }
}
