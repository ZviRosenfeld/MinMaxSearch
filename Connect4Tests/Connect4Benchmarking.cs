using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch;
using System;

namespace Connect4Tests
{
    [TestClass]
    [TestCategory("Benchmarking")]
    public class Connect4Benchmarking
    {
        [TestMethod]
        public void BenchmarkConnect4_EmptyBoard()
        {
            const int searchDepth = 11;
            var emptyBoard = Connect4TestUtils.GetEmptyBoard();
            BenchmarkWithDegreeOfParallelism(1, searchDepth, ParallelismMode.NonParallelism, emptyBoard);
            BenchmarkWithDegreeOfParallelism(1, searchDepth, ParallelismMode.FirstLevelOnly, emptyBoard);
            BenchmarkWithDegreeOfParallelism(4, searchDepth, ParallelismMode.TotalParallelism, emptyBoard);
        }

        [TestMethod]
        public void BenchmarkConnect4_HalfFullBoard()
        {
            var board = Connect4TestUtils.GetHalfFullBoard();
            BenchmarkWithDegreeOfParallelism(1, 12, ParallelismMode.FirstLevelOnly, board);
        }

        private void BenchmarkWithDegreeOfParallelism(int degreeOfParallelism, int searchDepth, ParallelismMode parallelismMode, Player[,] board)
        {
            Console.WriteLine("Running with degreeOfParallelism: " + degreeOfParallelism + ", Mode: " + parallelismMode);
            var engine = Connect4TestUtils.GetSearchEngine(degreeOfParallelism, parallelismMode);
            var startState = new Connect4State(board, Player.Max);

            var results = engine.Search(startState, searchDepth);
            Console.WriteLine("Time: " + results.SearchTime);
            Console.WriteLine("Leaves: " + results.Leaves);
            Console.WriteLine("InternalNodes: " + results.InternalNodes);
        }
    }
}
