using System.ComponentModel.DataAnnotations;
using Schematic.Identity;

namespace Schematic.Core.Mvc
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = AuthenticationErrorMessages.EmailRequired)]
        [DataType(DataType.EmailAddress)]
        [Display(Name = AuthenticationLabels.Email)]
        public string Email { get; set; }

        public bool SendReminderSuccess { get; set; } = false;
    }
}