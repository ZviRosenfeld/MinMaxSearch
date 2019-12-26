using FakeItEasy;

namespace MinMaxSearch.UnitTests.SampleTrees
{
    class UnaryDeterministicTree : ITree
    {
        public IDeterministicState RootState { get; } = A.Fake<IDeterministicState>();
        public IDeterministicState State2 { get; } = A.Fake<IDeterministicState>();
        public IDeterministicState State3 { get; } = A.Fake<IDeterministicState>();
        public IDeterministicState EndState { get; } = A.Fake<IDeterministicState>();

        /// <summary>
        /// This class contains the following tree:
        /// RootState -> Sate2 -> State3 -> EndState
        /// </summary>
        public UnaryDeterministicTree(Player rootPlayer = Player.Max)
        {
            A.CallTo(() => RootState.ToString()).Returns(nameof(RootState));
            A.CallTo(() => State2.ToString()).Returns(nameof(State2));
            A.CallTo(() => State3.ToString()).Returns(nameof(State3));
            A.CallTo(() => EndState.ToString()).Returns(nameof(EndState));

            A.CallTo(() => RootState.Turn).Returns(rootPlayer);
            A.CallTo(() => State2.Turn).Returns(rootPlayer.GetReversePlayer());
            A.CallTo(() => State3.Turn).Returns(rootPlayer);
            A.CallTo(() => EndState.Turn).Returns(rootPlayer.GetReversePlayer());

            RootState.SetNeigbor(State2);
            State2.SetNeigbor(State3);
            State3.SetNeigbor(EndState);
            EndState.SetAsEndState();
        }
    }
}
