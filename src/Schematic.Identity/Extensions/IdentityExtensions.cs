using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace Schematic.Identity
{
    public static class IdentityExtensions
    {
        public static string GetSpecificClaim(this IIdentity identity, string claimType)
        {
            var claim = (identity as ClaimsIdentity).Claims.FirstOrDefault(c => c.Type == claimType);
            return (claim is null) ? string.Empty : claim.Value;
        }

        public static int GetUserID(this IIdentity identity)
        {
            var identifier = (identity as ClaimsIdentity).FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int.TryParse(identifier, out int id);
            return id;
        }
    }
}