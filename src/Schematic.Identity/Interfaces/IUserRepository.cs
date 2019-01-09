using System.Collections.Generic;
using System.Threading.Tasks;

namespace Schematic.Identity
{
    public interface IUserRepository<TUser, TFilter, TSpecification>
    {
        Task<int> CreateAsync(TUser user, int userID);

        Task<TUser> ReadAsync(TSpecification user);

        Task<int> UpdateAsync(TUser user, int userID);

        Task<int> DeleteAsync(int id, int userID);

        Task<List<TUser>> ListAsync(TFilter filter);
    }
}