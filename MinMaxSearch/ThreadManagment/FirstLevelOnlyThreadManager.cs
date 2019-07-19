using System;
using System.Threading.Tasks;

namespace MinMaxSearch.ThreadManagment
{
    class FirstLevelOnlyThreadManager : IThreadManager
    {
        public Task<T> Invoke<T>(Func<T> func, int depth)
        {
            return depth == 0 ? Task.Run(func) : Task.FromResult(func());
        }
    }
}
