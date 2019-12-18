using FakeItEasy;

namespace MinMaxSearch.UnitTests.SampleTrees
{
    /// <summary>
    /// This class provides a search tree for the cach tests to use
    /// </summary>
     class Tree1
    {
        // This class uses search tree:
        //              StartState
        // ChildState1                    ChildState2          
        //  EndState1            EndState2            EndState3

        public IDeterministicState EndState1 = A.Fake<IDeterministicState>();
        public IDeterministicState EndState2 = A.Fake<IDeterministicState>();
        public IDeterministicState EndState3 = A.Fake<IDeterministicState>();
        public IDeterministicState ManyChildrenState = A.Fake<IDeterministicState>();
        public IDeterministicState ChildState1 = A.Fake<IDeterministicState>();
        public IDeterministicState ChildState2 = A.Fake<IDeterministicState>();
        
        public Tree1(Player startPlayer = Player.Max)
        {
            A.CallTo(() => EndState1.ToString()).Returns("EndState1");
            A.CallTo(() => EndState2.ToString()).Returns("EndState2");
            A.CallTo(() => EndState3.ToString()).Returns("EndState3");
            A.CallTo(() => ManyChildrenState.ToString()).Returns("StartState");
            A.CallTo(() => ChildState1.ToString()).Returns("ChildState1");
            A.CallTo(() => ChildState2.ToString()).Returns("ChildState2");

            A.CallTo(() => EndState1.Turn).Returns(startPlayer);
            A.CallTo(() => EndState2.Turn).Returns(startPlayer);
            A.CallTo(() => EndState3.Turn).Returns(startPlayer);
            A.CallTo(() => ManyChildrenState.Turn).Returns(startPlayer);
            A.CallTo(() => ChildState1.Turn).Returns(startPlayer.GetReversePlayer());
            A.CallTo(() => ChildState2.Turn).Returns(startPlayer.GetReversePlayer());

            ManyChildrenState.SetNeigbors(ChildState1, ChildState2);
            ChildState1.SetNeigbor(EndState1);
            ChildState2.SetNeigbors(EndState2, EndState3);
            EndState1.SetAsEndState();
            EndState2.SetAsEndState();
            EndState3.SetAsEndState();
        }
    }
}
