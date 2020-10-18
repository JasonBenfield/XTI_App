using System;
using System.Linq;
using System.Threading.Tasks;

namespace XTI_Core
{
    public interface DataRepository<T> where T : class
    {
        Task Create(T record);
        IQueryable<T> Retrieve();
        Task Update(T record, Action<T> a);
        Task Delete(T record);
        Task Transaction(Func<Task> action);

    }
}
