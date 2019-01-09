using System.Collections.Generic;
using System.Threading.Tasks;

namespace Schematic.Identity
{
    public interface IUserRoleRepository<TUserRole>
    {
        Task<List<TUserRole>> ListAsync();
    }
}