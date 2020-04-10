namespace MinMaxSearch.Cache
{
    public enum CacheMode
    {
        /// <summary>
        /// In this mode, the engine won't use any caching
        /// </summary>
        NoCache,
        /// <summary>
        /// In this mode, the engine will initialize and use a new cache for every search
        /// </summary>
        NewCache,
        /// <summary>
        /// In this mode, the engine will re-use the same cache between searches.
        /// You can clean the cache by calling the engine's CacheManager.CleanCache method.
        /// </summary>
        ReuseCache
    }
}
