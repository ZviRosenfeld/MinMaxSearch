﻿using System;
using System.Collections.Generic;
using FakeItEasy;

namespace MinMaxSearch.UnitTests
{
    static class TestUtils
    {
        public static void SetEvaluationTo(this IState state, double evaluation) =>
            A.CallTo(() => state.Evaluate(A<int>._, A<List<IState>>._)).Returns(evaluation);
        
        public static void SetNeigbors(this IDeterministicState state, IEnumerable<IState> neigbors) =>
            A.CallTo(() => state.GetNeighbors()).Returns(neigbors);

        public static void SetNeigbors(this IDeterministicState state, params IState[] neigbors) =>
            A.CallTo(() => state.GetNeighbors()).Returns(neigbors);

        public static void SetNeigbor(this IDeterministicState state, IState neigbor) =>
            A.CallTo(() => state.GetNeighbors()).Returns(new []{neigbor});

        public static void SetAsEndState(this IDeterministicState state) =>
            A.CallTo(() => state.GetNeighbors()).Returns(new IState[] {});

        public static void SetAsEndState(this IProbabilisticState state) =>
            A.CallTo(() => state.GetNeighbors()).Returns(new List<Tuple<double, List<IState>>>());

        public static SearchEngine GetBasicSearchEngine(
            ParallelismMode parallelismMode = ParallelismMode.FirstLevelOnly, int maxDegreeOfParallelism = 1,
            int parallelismLevel = 1) =>
            new SearchEngine()
            {
                SkipEvaluationForFirstNodeSingleNeighbor = false,
                ParallelismMode = parallelismMode,
                MaxLevelOfParallelism = parallelismLevel,
                MaxDegreeOfParallelism = maxDegreeOfParallelism
            };
    }
}
