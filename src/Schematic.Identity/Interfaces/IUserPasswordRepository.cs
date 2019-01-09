using System.Threading.Tasks;

namespace Schematic.Identity
{
    public interface IUserPasswordRepository<TUser>
    {
        Task<bool> CreatePasswordTokenAsync(TUser user, string token);

        Task<bool> SavePasswordTokenAsync(TUser user, string token);

        Task<bool> SetPasswordAsync(TUser user, string passHash);

        Task<TokenVerificationResult> ValidatePasswordTokenAsync(string email, string token);
    }
}