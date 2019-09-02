using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MinMaxSearch.UnitTests
{
    /// <summary>
    /// This SearchWorker is used for tests. It checks that the searchContext is passed with a certain maxDepth value 
    /// </summary>
    class TestSearchWorkerCheckMaxDepth : ISearchWorker
    {
        private readonly int[] expectedDepths;
        private int index = 0;

        public TestSearchWorkerCheckMaxDepth(int[] expectedDepths)
        {
            this.expectedDepths = expectedDepths;
        }

        public SearchResult Evaluate(IState startState, SearchContext searchContext)
        {
            Assert.AreEqual(expectedDepths[index], searchContext.MaxDepth, "Didn't get expected depth");
            index++;

            return new SearchResult(startState.Evaluate(1, new List<IState>()), startState);
        }
    }
}
