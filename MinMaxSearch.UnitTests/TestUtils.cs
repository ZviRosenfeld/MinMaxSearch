using System.Collections.Generic;
using FakeItEasy;

namespace MinMaxSearch.UnitTests
{
    static class TestUtils
    {
        public static void SetEvaluationTo(this IState state, double evaluation) =>
            A.CallTo(() => state.Evaluate(A<int>._, A<List<IState>>._)).Returns(evaluation);
    }
}
