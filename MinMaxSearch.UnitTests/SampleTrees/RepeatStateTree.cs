using FakeItEasy;

namespace MinMaxSearch.UnitTests.SampleTrees
{
    class RepeatStateTree
    {
        // This class uses search tree:
        //                                  StartState
        //       ChildState1                                childState1
        //       ChildState2                                childState2
        //ChildState4      ChildState5            ChildState4         ChildState5
        //EndState1     EndState2 EndState3         EndState1      EndState2 EndState3

        public IDeterministicState EndState1 = A.Fake<IDeterministicState>();
        public IDeterministicState EndState2 = A.Fake<IDeterministicState>();
        public IDeterministicState EndState3 = A.Fake<IDeterministicState>();
        public IDeterministicState StartState = A.Fake<IDeterministicState>();
        public IDeterministicState ChildState1 = A.Fake<IDeterministicState>();
        public IDeterministicState ChildState2 = A.Fake<IDeterministicState>();
        public IDeterministicState ChildState4 = A.Fake<IDeterministicState>();
        public IDeterministicState ChildState5 = A.Fake<IDeterministicState>();

        public RepeatStateTree(Player startPlayer = Player.Max)
        {
            A.CallTo(() => EndState1.ToString()).Returns("EndState1");
            A.CallTo(() => EndState2.ToString()).Returns("EndState2");
            A.CallTo(() => EndState3.ToString()).Returns("EndState3");
            A.CallTo(() => StartState.ToString()).Returns("StartState");
            A.CallTo(() => ChildState1.ToString()).Returns("ChildState1");
            A.CallTo(() => ChildState2.ToString()).Returns("ChildState2");
            A.CallTo(() => ChildState4.ToString()).Returns("ChildState4");
            A.CallTo(() => ChildState5.ToString()).Returns("ChildState5");
            
            A.CallTo(() => StartState.Turn).Returns(startPlayer);
            A.CallTo(() => ChildState1.Turn).Returns(startPlayer.GetReversePlayer());
            A.CallTo(() => ChildState2.Turn).Returns(startPlayer);
            A.CallTo(() => ChildState4.Turn).Returns(startPlayer.GetReversePlayer());
            A.CallTo(() => ChildState5.Turn).Returns(startPlayer.GetReversePlayer());
            A.CallTo(() => EndState1.Turn).Returns(startPlayer);
            A.CallTo(() => EndState2.Turn).Returns(startPlayer);
            A.CallTo(() => EndState3.Turn).Returns(startPlayer);

            StartState.SetNeigbors(ChildState1, ChildState1);
            ChildState1.SetNeigbor(ChildState2);
            ChildState2.SetNeigbors(ChildState4, ChildState5);
            ChildState4.SetNeigbor(EndState1);
            ChildState5.SetNeigbors(EndState2, EndState3);
            EndState1.SetAsEndState();
            EndState2.SetAsEndState();
            EndState3.SetAsEndState();
        }
    }
}
