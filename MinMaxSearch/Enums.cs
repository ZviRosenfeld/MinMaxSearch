namespace MinMaxSearch
{
    public enum Player
    {
        Empty = 0,
        Min = 1,
        Max = 2,
    }

    public enum RememberStatesMode
    {
        /// <summary>
        /// Never remember states
        /// </summary>
        Never,

        /// <summary>
        /// Remember states that we've encountered in this search
        /// </summary>
        InSameSearch,

        /// <summary>
        /// Remember all states we've encountered - even between searches.
        /// If the remembered search table gets to heavy, you can use the SearchEngin's Clear method to clear it.
        /// </summary>
        BetweenSearches,
    }
}
