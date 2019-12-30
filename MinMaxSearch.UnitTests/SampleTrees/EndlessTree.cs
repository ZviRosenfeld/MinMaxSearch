using FakeItEasy;

namespace MinMaxSearch.UnitTests.SampleTrees
{
    /// <summary>
    /// A search tree used for tests
    /// </summary>
    class EndlessTree : ITree
    {
        // This class contains the following tree:
        //              RootState
        // ChildState1                    ChildState2          
        
        public IDeterministicState RootState { get; } = A.Fake<IDeterministicState>();
        public IDeterministicState ChildState1 { get; } = A.Fake<IDeterministicState>();
        public IDeterministicState ChildState2 { get; } = A.Fake<IDeterministicState>();

        public EndlessTree(Player startPlayer = Player.Max)
        {
            A.CallTo(() => RootState.ToString()).Returns(nameof(RootState));
            A.CallTo(() => ChildState1.ToString()).Returns(nameof(ChildState1));
            A.CallTo(() => ChildState2.ToString()).Returns(nameof(ChildState2));
            
            A.CallTo(() => RootState.Turn).Returns(startPlayer);
            A.CallTo(() => ChildState1.Turn).Returns(startPlayer.GetReversePlayer());
            A.CallTo(() => ChildState2.Turn).Returns(startPlayer.GetReversePlayer());

            RootState.SetNeigbors(ChildState1, ChildState2);
            ChildState1.SetNeigbor(ChildState1);
            ChildState2.SetNeigbors(ChildState2);
        }
    }
}

