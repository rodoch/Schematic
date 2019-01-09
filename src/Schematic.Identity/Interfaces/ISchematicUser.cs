using System.Collections.Generic;

namespace Schematic.Identity
{
    public interface ISchematicUser
    {
        int ID { get; set; }

        string Forenames { get; set; }

        string Surnames { get; set; }

        string FullName { get; set; }

        string Email { get; set; }

        string PassHash { get; set; }
        
        List<UserRole> Roles { get; set; }
    }
}