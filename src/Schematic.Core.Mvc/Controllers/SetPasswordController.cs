using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Ansa.Extensions;
using Schematic.Identity;

namespace Schematic.Core.Mvc
{
    public class SetPasswordController : Controller
    {
        private readonly IEmailValidatorService _emailValidatorService;
        private readonly IPasswordValidatorService _passwordValidatorService;
        private readonly IPasswordHasherService<User> _passwordHasherService;
        private readonly IUserRepository<User, UserFilter, UserSpecification> _userRepository;
        private readonly IUserPasswordRepository<UserDTO> _userPasswordRepository;
        private readonly IStringLocalizer<SetPasswordViewModel> _localizer;

        protected User AuthenticationUser;

        public SetPasswordController(
            IEmailValidatorService emailValidatorService,
            IPasswordValidatorService passwordValidatorService,
            IPasswordHasherService<User> passwordHasherService,
            IUserRepository<User, UserFilter, UserSpecification> userRepository,
            IUserPasswordRepository<UserDTO> userPasswordRepository,
            IStringLocalizer<SetPasswordViewModel> localizer)
        {
            _emailValidatorService = emailValidatorService;
            _passwordValidatorService = passwordValidatorService;
            _passwordHasherService = passwordHasherService;
            _userRepository = userRepository;
            _userPasswordRepository = userPasswordRepository;
            _localizer = localizer;
        }

        [BindProperty]
        public SetPasswordViewModel SetPasswordData { get; set; }

        [Route("{culture}/in/set")]
        [HttpGet]
        public IActionResult Authentication(string token)
        {
            var data = new AuthenticationViewModel();
            data.Mode = "set-password";
            data.Token = token;

            return View("/Views/Authentication/Authentication.cshtml", data);
        }

        [Route("{culture}/in/set-password")]
        [HttpGet]
        public IActionResult SetPassword(string token)
        {
            var data = new SetPasswordViewModel();
            data.Token = token;

            return PartialView(data);
        }

        [Route("{culture}/in/set-password")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel data)
        {
            ViewData["Email"] = data.Email;

            if (!ModelState.IsValid)
            {
                return PartialView(data);
            }

            if (!_emailValidatorService.IsValidEmail(data.Email))
            {
                ModelState.AddModelError("InvalidEmail", _localizer[AuthenticationErrorMessages.InvalidEmail]);
                return PartialView(data);
            }

            if (data.NewPassword.HasValue() && data.ConfirmNewPassword.HasValue() 
                && data.NewPassword != data.ConfirmNewPassword)
            {
                ModelState.AddModelError("PasswordsDoNotMatch", _localizer[AuthenticationErrorMessages.PasswordsDoNotMatch]);
                return PartialView(data);
            }

            var tokenResult = await _userPasswordRepository.ValidatePasswordTokenAsync(data.Email, data.Token);

            if (tokenResult == TokenVerificationResult.Invalid)
            {
                ModelState.AddModelError("InvalidToken", _localizer[AuthenticationErrorMessages.InvalidToken]);
                return PartialView(data);
            }

            if (tokenResult == TokenVerificationResult.Expired)
            {
                ModelState.AddModelError("ExpiredToken", _localizer[AuthenticationErrorMessages.ExpiredToken]);
                return PartialView(data);
            }

            var userSpecification = new UserSpecification()
            {
                Email = data.Email
            };

            AuthenticationUser = await _userRepository.ReadAsync(userSpecification);

            if (AuthenticationUser is null)
            {
                ModelState.AddModelError("UserDoesNotExist", _localizer[AuthenticationErrorMessages.UserDoesNotExist]);
                return PartialView(data);
            }

            var passwordValidationErrors = _passwordValidatorService.ValidatePassword(data.NewPassword);

            if (passwordValidationErrors.Count > 0)
            {
                ModelState.AddModelError(
                    key: "PasswordValidationErrors",
                    errorMessage: _localizer[_passwordValidatorService.GetPasswordValidationErrorMessage()]
                );
                return PartialView(data);
            }

            string hashedPassword = _passwordHasherService.HashPassword(AuthenticationUser, data.NewPassword);
            var userDTO = new UserDTO(AuthenticationUser);
            var setPassword = await _userPasswordRepository.SetPasswordAsync(userDTO, hashedPassword);

            if (!setPassword)
            {
                ModelState.AddModelError("PasswordSetFailed", _localizer[AuthenticationErrorMessages.PasswordSetFailed]);
                return PartialView();
            }

            data.SetPasswordSuccess = true;

            return PartialView(data);
        }
    }
}