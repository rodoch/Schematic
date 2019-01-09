using System.ComponentModel.DataAnnotations;
using Schematic.Identity;

namespace Schematic.Core.Mvc
{
    public class SetPasswordViewModel
    {
        [Required(ErrorMessage = AuthenticationErrorMessages.NoTokenProvided)]
        public string Token { get; set; }

        [Required(ErrorMessage = AuthenticationErrorMessages.EmailRequired)]
        [DataType(DataType.EmailAddress)]
        [Display(Name = AuthenticationLabels.Email)]
        public string Email { get; set; }

        [Required(ErrorMessage = AuthenticationErrorMessages.PasswordRequired)]
        [DataType(DataType.Password)]
        [Display(Name = AuthenticationLabels.NewPassword)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = AuthenticationErrorMessages.PasswordConfirmationRequired)]
        [DataType(DataType.Password)]
        [Display(Name = AuthenticationLabels.ConfirmNewPassword)]
        public string ConfirmNewPassword { get; set; }

        public bool SetPasswordSuccess { get; set; } = false;
    }
}