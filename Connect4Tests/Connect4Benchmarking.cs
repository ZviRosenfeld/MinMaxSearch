using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinMaxSearch;
using System;
using System.Text;

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
            BenchmarkWithDegreeOfParallelism(board, searchDepth, ParallelismMode.NonParallelism);
            BenchmarkWithDegreeOfParallelism(board, searchDepth, ParallelismMode.FirstLevelOnly);
            BenchmarkWithDegreeOfParallelism(board, searchDepth, ParallelismMode.ParallelismByLevel, levelOfParallelism: 2);
            BenchmarkWithDegreeOfParallelism(board, searchDepth, ParallelismMode.TotalParallelism, 4);
        }

        [TestMethod]
        public void BenchmarkConnect4_HalfFullBoard() =>
            BenchmarkWithDegreeOfParallelism(Connect4TestUtils.GetHalfFullBoard(), 13);

        private void BenchmarkWithDegreeOfParallelism(Player[,] startBoard, int searchDepth,
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
