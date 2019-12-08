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
        /// In this mode, the entire search tree will be calculated in parallel, up to the "MaxDegreeOfParallelism"
        /// </summary>
        TotalParallelism,

        /// <summary>
        /// In this mode only the first level of the search tree will be calculated in parallel.
        /// In this mode "MaxDegreeOfParallelism" and "MaxLevelOfParallelism" will be ignored.
        /// </summary>
        FirstLevelOnly,

        /// <summary>
        /// In this mode, the first x levels of the search will be carried out in parallel.
        /// You can determine x by setting "MaxLevelOfParallelism" in the SearchEngine.
        /// </summary>
        ParallelismByLevel,

        /// <summary>
        /// No parallelism. In this mode "MaxDegreeOfParallelism" and "MaxLevelOfParallelism" will be ignored.
        /// </summary>
        NonParallelism
    }
}
