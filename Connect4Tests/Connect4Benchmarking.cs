using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch;
using System;
using System.Text;
using MinMaxSearch.Cache;

namespace Connect4Tests
{
    [TestClass]
    [TestCategory("Benchmarking")]
    public class Connect4Benchmarking
    {
        [TestMethod]
        public void BenchmarkConnect4()
        {
            var searchDepth = 11;
            var board = Connect4TestUtils.GetEmptyBoard();
            Benchmark(board, searchDepth, ParallelismMode.NonParallelism);
            Benchmark(board, searchDepth, ParallelismMode.FirstLevelOnly);
            Benchmark(board, searchDepth, ParallelismMode.ParallelismByLevel, levelOfParallelism: 2);
            Benchmark(board, searchDepth, ParallelismMode.TotalParallelism, 4);
        }

        [TestMethod]
        public void BenchmarkConnect4_HalfFullBoard() =>
            Benchmark(Connect4TestUtils.GetHalfFullBoard(), 20);

        private void Benchmark(Player[,] startBoard, int searchDepth,
            ParallelismMode parallelismMode = ParallelismMode.FirstLevelOnly, int degreeOfParallelism = 1,
            int levelOfParallelism = 1)
        {
            Console.WriteLine(GetTestMessage(parallelismMode, degreeOfParallelism, levelOfParallelism));
            var engine = Connect4TestUtils.GetSearchEngine(degreeOfParallelism, parallelismMode, levelOfParallelism);
            var startState = new Connect4State(startBoard, Player.Max);

            var results = engine.Search(startState, searchDepth);
            Console.WriteLine("Time: " + results.SearchTime);
            Console.WriteLine("Leaves: " + results.Leaves);
            Console.WriteLine("InternalNodes: " + results.InternalNodes);
        }

        private string GetTestMessage(ParallelismMode parallelismMode, int degreeOfParallelism, int levelOfParallelism)
        {
            var stringBuilder = new StringBuilder("Running Mode " + parallelismMode);
            if (parallelismMode == ParallelismMode.TotalParallelism)
                stringBuilder.Append($" {nameof(degreeOfParallelism)} == {degreeOfParallelism}");
            if (parallelismMode == ParallelismMode.ParallelismByLevel)
                stringBuilder.Append($" {nameof(levelOfParallelism)} == {levelOfParallelism}");

            return stringBuilder.ToString();
        }
    }
}
