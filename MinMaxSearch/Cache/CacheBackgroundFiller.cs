using System.Threading;
using System.Threading.Tasks;

namespace MinMaxSearch.Cache
{
    /// <summary>
    /// This class can fill the cache in the background - say, while the user is considering their next turn
    /// </summary>
    public static class CacheBackgroundFiller
    {
        /// <summary>
        /// This method will fill the cache.
        /// It can be useful to run while the user is considering their turn (when the program would otherwise be idel).
        /// Note that this is only helpful if you're using the ReuseCache CasheMode.
        /// </summary>
        public static void FillCache(this SearchEngine searchEngine, IDeterministicState startState, CancellationToken cancellationToken, ParallelismMode parallelismMode = ParallelismMode.NonParallelism, int maxDegreeOfParallelism = 1)
        {
            var newEngine = searchEngine.Clone();
            newEngine.ParallelismMode = parallelismMode;
            newEngine.MaxDegreeOfParallelism = maxDegreeOfParallelism;
            newEngine.SetCustomCache(searchEngine.GetCacheManager());

            // Running this will fill the cache
            new IterativeSearchWrapper(newEngine).IterativeSearch(startState, 1, 100, cancellationToken);
        }

        /// <summary>
        /// This method will fill the cache.
        /// It can be useful to run while the user is considering their turn (when the program would otherwise be idel).
        /// Note that this is only helpful if you're using the ReuseCache CasheMode.
        /// </summary>
        public static Task FillCacheAsync(this SearchEngine searchEngine, IDeterministicState startState, CancellationToken cancellationToken, ParallelismMode parallelismMode = ParallelismMode.NonParallelism, int maxDegreeOfParallelism = 1)
        {
            return Task.Run(() => searchEngine.FillCache(startState, cancellationToken, parallelismMode, maxDegreeOfParallelism));
        }
    }
}
