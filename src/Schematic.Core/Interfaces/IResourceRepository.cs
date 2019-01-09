using System.Collections.Generic;
using System.Threading.Tasks;

namespace Schematic.Core
{
    public interface IResourceRepository<T, TResourceFilter>
    {
        Task<int> CreateAsync(T resource, int userID);

        Task<T> ReadAsync(int id);

        Task<int> UpdateAsync(T resource, int userID);

        Task<int> DeleteAsync(int id, int userID);
        
        Task<List<T>> ListAsync(TResourceFilter filter);
    }
}