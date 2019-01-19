using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Ansa.Extensions;
using Schematic.Identity;

namespace Schematic.Core.Mvc
{
    public class UserViewModel<TUser> where TUser : ISchematicUser
    {
        public int ID { get; set; }

        public TUser User { get; set; }
        
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        public string ConfirmationPassword { get; set; }

        public bool HasIdentity(int userID) => User.ID == userID;

        public bool IsNew { get => ID == 0; }

        public bool IsVerified { get; set; }
    }
}