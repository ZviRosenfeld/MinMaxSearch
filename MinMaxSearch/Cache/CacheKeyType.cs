namespace MinMaxSearch.Cache
{
    /// <summary>
    /// When using a cache, we need to make sure that the cache key takes into account all the data that affects the state's evaluation.
    /// For most searches, the state alone will be enough, but some will also need information about the depths in the search tree, or the states we've passed through.
    /// </summary>
    public enum CacheKeyType
    {
        Unknown,
        /// <summary>
        /// The key will contain only the state
        /// </summary>
        StateOnly,
        /// <summary>
        /// The key will contain the state, and it's depths
        /// </summary>
        StateAndDepth,
        /// <summary>
        /// The key will contain the state, and the states it's passed through.
        /// Note that due to the time it takes to calculate the hash for all the passed through states using a cache with this key probably won't improve performance.
        /// </summary>
        StateAndPassedThroughStates
    }
}
