using System;
using System.Threading.Tasks;

namespace MinMaxSearch.ThreadManagment
{
    class SequencelThreadManager : IThreadManager
    {
        public Task<T> Invoke<T>(Func<T> func, int depth) => Task.FromResult(func());
    }
}
