namespace MinMaxSearch
{
    public static class Utils
    {
        public static Player GetReversePlayer(Player player) =>
            (Player)((int)player * -1);
    }
}
