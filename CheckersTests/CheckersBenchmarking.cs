using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch;
using MinMaxSearch.Cache;

namespace CheckersTests
{
    [TestClass]
    [TestCategory("Benchmarking")]
    public class CheckersBenchmarking
    {
        [TestMethod]
        public void BenchmarkCheckers()
        {
            var searchDepth = 8;
            var board = TestUtils.GetStartBoard();
            Benchmark(board, searchDepth, ParallelismMode.NonParallelism);
            Benchmark(board, searchDepth, ParallelismMode.FirstLevelOnly);
            Benchmark(board, searchDepth, ParallelismMode.ParallelismByLevel, levelOfParallelism: 2);
            Benchmark(board, searchDepth, ParallelismMode.TotalParallelism, 4);
        }

        [TestMethod]
        public void BenchmarkCheckersAllKings()
        {
            var searchDepth = 8;
            var board = TestUtils.GetStartBoardFullOfKings();
            Benchmark(board, searchDepth, ParallelismMode.NonParallelism);
            Benchmark(board, searchDepth, ParallelismMode.FirstLevelOnly);
            Benchmark(board, searchDepth, ParallelismMode.ParallelismByLevel, levelOfParallelism: 2);
            Benchmark(board, searchDepth, ParallelismMode.TotalParallelism, 4);
        }

        private void Benchmark(CheckerPiece[,] startBoard, int searchDepth,
            ParallelismMode parallelismMode = ParallelismMode.FirstLevelOnly, int degreeOfParallelism = 1,
            int levelOfParallelism = 1)
        {
            Console.WriteLine(GetTestMessage(parallelismMode, degreeOfParallelism, levelOfParallelism));
            var engine = TestUtils.GetCheckersSearchEngine(degreeOfParallelism, parallelismMode, levelOfParallelism);
            engine.CacheMode = CacheMode.NewCache;
            var startState = new CheckersState(startBoard, Player.Max);

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
