using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch.Cache;
using MinMaxSearch.UnitTests.SampleTrees;

namespace MinMaxSearch.UnitTests.CacheTests
{
    /// <summary>
    /// This class checks that the search used the cached data
    /// </summary>
    [TestClass]
    public class UseCahcedDataTests
    {
        [TestMethod]
        public void ExactValueInCache_UseValue()
        {
            var searchTree = new DeterministicTree();
            var engine = new SearchEngine(CacheMode.ReuseCache, CacheKeyType.StateOnly)
            {
                SkipEvaluationForFirstNodeSingleNeighbor = false
            };
            engine.CacheManager.AddExactEvaluation(searchTree.EndState1,0, null, 3);
            Assert.AreEqual(3, engine.Search(searchTree.ChildState1, 4).Evaluation);
        }

        [TestMethod]
        public void CachedMaxSmallerThanAlpha()
        {
            var searchTree = new DeterministicTree();
            searchTree.EndState1.SetEvaluationTo(-2);
            var engine = new SearchEngine(CacheMode.ReuseCache, CacheKeyType.StateOnly)
            {
                ParallelismMode = ParallelismMode.NonParallelism,
                SkipEvaluationForFirstNodeSingleNeighbor = false
            };
            engine.CacheManager.AddMaxEvaluation(searchTree.ChildState2, 0, null, -4);
            Assert.AreEqual(-2, engine.Search(searchTree.RootState, 5).Evaluation);
        }

        [TestMethod]
        public void CachedMinGreatrThanBeta()
        {
            var searchTree = new DeterministicTree();
            searchTree.EndState2.SetEvaluationTo(2);
            searchTree.EndState2.SetEvaluationTo(4);
            var engine = new SearchEngine(CacheMode.ReuseCache, CacheKeyType.StateOnly)
            {
                ParallelismMode = ParallelismMode.NonParallelism,
                SkipEvaluationForFirstNodeSingleNeighbor = false
            };
            engine.CacheManager.AddMinEvaluation(searchTree.EndState3, 0, null, 8);
            Assert.AreEqual(4, engine.Search(searchTree.RootState, 5).Evaluation);
        }

        [TestMethod]
        [DataRow(Player.Max)]
        [DataRow(Player.Min)]
        public void MinCachedValueEqualeMaxScore_UseValue(Player firstPlayer)
        {
            var maxEvaluation = 10;
            var searchTree = new DeterministicTree(firstPlayer);
            var engine = new SearchEngine(CacheMode.ReuseCache, CacheKeyType.StateOnly)
            {
                MaxScore = maxEvaluation,
                SkipEvaluationForFirstNodeSingleNeighbor = false
            };
            engine.CacheManager.AddMinEvaluation(searchTree.EndState1, 0, null, maxEvaluation);
            Assert.AreEqual(maxEvaluation, engine.Search(searchTree.ChildState1, 4).Evaluation);
        }

        [TestMethod]
        [DataRow(Player.Max)]
        [DataRow(Player.Min)]
        public void MaxCachedValueEqualeMinScore_UseValue(Player firstPlayer)
        {
            var minEvaluation = -10;
            var searchTree = new DeterministicTree(firstPlayer);
            var engine = new SearchEngine(CacheMode.ReuseCache, CacheKeyType.StateOnly)
            {
                MinScore = minEvaluation,
                SkipEvaluationForFirstNodeSingleNeighbor = false
            };
            engine.CacheManager.AddMaxEvaluation(searchTree.EndState1, 0, null, minEvaluation);
            Assert.AreEqual(minEvaluation, engine.Search(searchTree.ChildState1, 4).Evaluation);
        }
    }
}
