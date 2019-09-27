using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MinMaxSearch.UnitTests
{
    /// <summary>
    /// This SearchEngine is used for tests. It checks that the searchContext is passed with a certain maxDepth value 
    /// </summary>
    class TestSearchEngineCheckMaxDepth : ISearchEngine
    {
        private readonly int[] expectedDepths;
        private int index = 0;

        public TestSearchEngineCheckMaxDepth(int[] expectedDepths)
        {
            this.expectedDepths = expectedDepths;
        }
        
        public SearchResult Search(IDeterministicState startState, int maxDepth, CancellationToken cancellationToken)
        {
            Assert.AreEqual(expectedDepths[index], maxDepth, "Didn't get expected depth");
            index++;

            return new SearchResult(startState.Evaluate(1, new List<IState>()), startState);
        }
    }
}
