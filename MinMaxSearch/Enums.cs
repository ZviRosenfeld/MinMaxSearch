namespace MinMaxSearch
{
    public enum Player
    {
        Empty = 0,
        Min = 1,
        Max = 2,
    }

    public enum ParallelismMode
    {
        /// <summary>
        /// In this mode, the entire search tree will be calclated in parallel, up to the 
        /// "maxDegreeOfParallelism"
        /// </summary>
        TotalParallelism,
        /// <summary>
        /// In this mode only the first level of the search tree will be calclated in parallel.
        /// In this mode "maxDegreeOfParallelism" will be ignored.
        /// </summary>
        FirstLevelOnly,
        /// <summary>
        /// No parallelism. In this mode "maxDegreeOfParallelism" will be ignored.
        /// </summary>
        NonParallelism
    }
}
