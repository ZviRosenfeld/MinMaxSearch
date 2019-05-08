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
        /// Remember states we've encountered in the current search, but don't remember states between searches.
        /// </summary>
        InSameSearch,

        /// <summary>
        /// If a state is encountered in one search, we'll remember it for the next searches performed by the same SearchEngine.
        /// If the remembered search table gets to heavy, you can use the SearchEngin's Clear method to clear it.
        /// </summary>
        BetweenSearches,
    }
}
