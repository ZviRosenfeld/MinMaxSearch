using System;
using System.Threading.Tasks;

namespace MinMaxSearch.ThreadManagment
{
    /// <summary>
    /// This thread manager runs all the searches up to "level" in parallel.
    /// So if, for example, "level" is 2, we'll run all the searches from depths 0 and 1 run in parallel. 
    /// </summary>
    public class LevelParallelismThreadManager : IThreadManager
    {
        private readonly int levels;

        public LevelParallelismThreadManager(int levels)
        {
            if (levels <= 0)
                throw new MinMaxSearchException($"{nameof(levels)} must be greater than 0. It was {levels}.");
            this.levels = levels;
        }

        public Task<T> Invoke<T>(Func<T> func, int depth)
        {
            return depth == levels - 1 ? Task.Run(func) : Task.FromResult(func());
        }
    }
}
