using System;
using System.Threading.Tasks;

namespace MinMaxSearch.ThreadManagment
{
    public interface IThreadManager
    {
        Task<T> Invoke<T>(Func<T> func, int depth);
    }
}