using FakeItEasy;

namespace MinMaxSearch.UnitTests.SampleTrees
{
    /// <summary>
    /// A search tree used for tests
    /// </summary>
    class DeterministicTree : ITree
    {
        // This class contains the following tree:
        //              RootState
        // ChildState1                    ChildState2          
        //  EndState1            EndState2            EndState3

        public IDeterministicState EndState1 { get; }= A.Fake<IDeterministicState>();
        public IDeterministicState EndState2 { get; } = A.Fake<IDeterministicState>();
        public IDeterministicState EndState3 { get; }= A.Fake<IDeterministicState>();
        public IDeterministicState RootState { get; }= A.Fake<IDeterministicState>();
        public IDeterministicState ChildState1 { get; } = A.Fake<IDeterministicState>();
        public IDeterministicState ChildState2 { get; }= A.Fake<IDeterministicState>();
        
        public DeterministicTree(Player startPlayer = Player.Max)
        {
            A.CallTo(() => EndState1.ToString()).Returns(nameof(EndState1));
            A.CallTo(() => EndState2.ToString()).Returns(nameof(EndState2));
            A.CallTo(() => EndState3.ToString()).Returns(nameof(EndState3));
            A.CallTo(() => RootState.ToString()).Returns(nameof(RootState));
            A.CallTo(() => ChildState1.ToString()).Returns(nameof(ChildState1));
            A.CallTo(() => ChildState2.ToString()).Returns(nameof(ChildState2));

            A.CallTo(() => EndState1.Turn).Returns(startPlayer);
            A.CallTo(() => EndState2.Turn).Returns(startPlayer);
            A.CallTo(() => EndState3.Turn).Returns(startPlayer);
            A.CallTo(() => RootState.Turn).Returns(startPlayer);
            A.CallTo(() => ChildState1.Turn).Returns(startPlayer.GetReversePlayer());
            A.CallTo(() => ChildState2.Turn).Returns(startPlayer.GetReversePlayer());

            RootState.SetNeigbors(ChildState1, ChildState2);
            ChildState1.SetNeigbor(EndState1);
            ChildState2.SetNeigbors(EndState2, EndState3);
            EndState1.SetAsEndState();
            EndState2.SetAsEndState();
            EndState3.SetAsEndState();
        }
    }
}
