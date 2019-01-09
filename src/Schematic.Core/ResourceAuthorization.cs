using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Schematic.Core
{
    public static class ResourceAuthorization
    {
        public static bool IsAuthorized(this ClaimsPrincipal user, Type type)
        {
            // If the type has no SchematicAuthorizeAttribute the user is authorised
            if (!type.IsDefined(typeof(SchematicAuthorizeAttribute), false))
            {
                return true;
            }

            // Otherwise, get attribute value and test against user identity
            string requiredRole = type.GetAttributeValue((SchematicAuthorizeAttribute a) => a.Role);

            ClaimsIdentity ClaimsIdentity = user.Identity as ClaimsIdentity;
            List<Claim> roles = ClaimsIdentity.FindAll(ClaimTypes.Role).ToList();

            foreach (Claim role in roles)
            {
                if (role.Value == requiredRole)
                {
                    return true;
                }
            }

            return false;
        }
    }
}