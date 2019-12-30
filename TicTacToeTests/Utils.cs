using MinMaxSearch;

namespace TicTacToeTests
{
    public static class Utils
    {
        public static TicTacToeState GetEmptyTicTacToeState() => new TicTacToeState(new[,]
        {
            { Player.Empty, Player.Empty, Player.Empty},
            { Player.Empty, Player.Empty, Player.Empty},
            { Player.Empty, Player.Empty, Player.Empty},
        }, Player.Max);
    }
}
