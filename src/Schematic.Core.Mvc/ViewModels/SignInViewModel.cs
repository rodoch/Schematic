using System.ComponentModel.DataAnnotations;
using Schematic.Identity;

namespace Schematic.Core.Mvc
{
    public class SignInViewModel
    {
        public int ID { get; set; }
        
        [Required(ErrorMessage = AuthenticationErrorMessages.EmailRequired)]
        [DataType(DataType.EmailAddress)]
        [Display(Name = AuthenticationLabels.Email)]
        public string Email { get; set; }

        [Required(ErrorMessage = AuthenticationErrorMessages.PasswordRequired)]
        [DataType(DataType.Password)]
        [Display(Name = AuthenticationLabels.Password)]
        public string Password { get; set; }

        [Display(Name = AuthenticationLabels.RememberMe)]
        public bool RememberMe { get; set; }
    }
}