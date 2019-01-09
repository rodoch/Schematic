using System;

namespace Schematic.Identity
{
    public class UserDTO
    {
        public UserDTO(ISchematicUser user)
        {
            ID = user.ID;
            Forenames = user.Forenames;
            Surnames = user.Surnames;
            FullName = user.FullName;
            Email = user.Email;
        }

        public int ID { get; }

        public string Forenames { get; }

        public string Surnames { get; }

        public string FullName { get; }

        public string Email { get; }
    }
}