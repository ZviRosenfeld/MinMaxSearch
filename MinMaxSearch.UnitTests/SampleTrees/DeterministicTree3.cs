using FakeItEasy;

namespace MinMaxSearch.UnitTests.SampleTrees
{
    /// <summary>
    /// A search tree used for tests
    /// </summary>
    class DeterministicTree3 : ITree
    {
        // This class contains the following tree:
        //                  RootState       
        //  EndState1       EndState2         EndState3

        public IDeterministicState EndState1 { get; } = A.Fake<IDeterministicState>();
        public IDeterministicState EndState2 { get; } = A.Fake<IDeterministicState>();
        public IDeterministicState EndState3 { get; } = A.Fake<IDeterministicState>();
        public IDeterministicState RootState { get; } = A.Fake<IDeterministicState>();

        public DeterministicTree3(Player startPlayer = Player.Max)
        {
            A.CallTo(() => EndState1.ToString()).Returns(nameof(EndState1));
            A.CallTo(() => EndState2.ToString()).Returns(nameof(EndState2));
            A.CallTo(() => EndState3.ToString()).Returns(nameof(EndState3));
            A.CallTo(() => RootState.ToString()).Returns(nameof(RootState));

            A.CallTo(() => EndState1.Turn).Returns(startPlayer.GetReversePlayer());
            A.CallTo(() => EndState2.Turn).Returns(startPlayer.GetReversePlayer());
            A.CallTo(() => EndState3.Turn).Returns(startPlayer.GetReversePlayer());
            A.CallTo(() => RootState.Turn).Returns(startPlayer);

            RootState.SetNeigbors(EndState1, EndState2, EndState3);
            EndState1.SetAsEndState();
            EndState2.SetAsEndState();
            EndState3.SetAsEndState();
        }
    }
}
