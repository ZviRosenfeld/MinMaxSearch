namespace MinMaxSearch.Cache
{
    public enum CacheMode
    {
        /// <summary>
        /// In this mode, the enigne won't use any cacheing
        /// </summary>
        NoCache,
        /// <summary>
        /// In this mode, the engine will initalize an use a new cache for every search
        /// </summary>
        NewCache,
        /// <summary>
        /// In this mode, the engine will re-use the same cache between searches.
        /// You can clean the cache by calling the engine's CleanCache method
        /// </summary>
        ReuseCache
    }
}
