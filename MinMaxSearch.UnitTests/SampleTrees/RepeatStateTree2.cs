using FakeItEasy;

namespace MinMaxSearch.UnitTests.SampleTrees
{
    class RepeatStateTree2 : ITree
    {
        // This class contains the following tree:
        //              RootState
        //      State1            State2
        //      State3            State3
        //    EndState1         EndState1s

        public IDeterministicState EndState1 { get; } = A.Fake<IDeterministicState>();
        public IDeterministicState State1 { get; } = A.Fake<IDeterministicState>();
        public IDeterministicState State2 { get; } = A.Fake<IDeterministicState>();
        public IDeterministicState State3 { get; } = A.Fake<IDeterministicState>();

        public IDeterministicState RootState { get; } = A.Fake<IDeterministicState>();

        public RepeatStateTree2(Player startPlayer = Player.Max)
        {
            A.CallTo(() => EndState1.ToString()).Returns(nameof(EndState1));
            A.CallTo(() => State1.ToString()).Returns(nameof(State1));
            A.CallTo(() => State2.ToString()).Returns(nameof(State2));
            A.CallTo(() => State2.ToString()).Returns(nameof(State3));
            A.CallTo(() => RootState.ToString()).Returns(nameof(RootState));


            A.CallTo(() => RootState.Turn).Returns(startPlayer);
            A.CallTo(() => State1.Turn).Returns(startPlayer.GetReversePlayer());
            A.CallTo(() => State2.Turn).Returns(startPlayer.GetReversePlayer());
            A.CallTo(() => State3.Turn).Returns(startPlayer);
            A.CallTo(() => EndState1.Turn).Returns(startPlayer.GetReversePlayer());

            RootState.SetNeigbors(State1, State2);
            EndState1.SetAsEndState();
            State1.SetNeigbor(State3);
            State2.SetNeigbor(State3);
            State3.SetNeigbor(EndState1);
        }
    }
}
