using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Ansa.Extensions;
using Schematic.Identity;

namespace Schematic.Core.Mvc
{
    public class ForgotPasswordController : Controller
    {
        private readonly IEmailValidatorService _emailValidatorService;
        private readonly IEmailSenderService _emailSenderService;
        private readonly IForgotPasswordEmail<User> _forgotPasswordEmail;
        private readonly IUserRepository<User, UserFilter, UserSpecification> _userRepository;
        private readonly IUserPasswordRepository<UserDTO> _userPasswordRepository;
        private readonly IStringLocalizer<ForgotPasswordViewModel> _localizer;
        
        protected User AuthenticationUser;

        public ForgotPasswordController(
            IEmailValidatorService emailValidatorService,
            IEmailSenderService emailSenderService,
            IForgotPasswordEmail<User> forgotPasswordEmail,
            IUserRepository<User, UserFilter, UserSpecification> userRepository,
            IUserPasswordRepository<UserDTO> userPasswordRepository,
            IStringLocalizer<ForgotPasswordViewModel> localizer)
        {
            _emailValidatorService = emailValidatorService;
            _emailSenderService = emailSenderService;
            _forgotPasswordEmail = forgotPasswordEmail;
            _userRepository = userRepository;
            _userPasswordRepository = userPasswordRepository;
            _localizer = localizer;
        }

        [BindProperty]
        public ForgotPasswordViewModel ForgotPasswordData { get; set; }

        [Route("{culture}/in/forgot")]
        [HttpGet]
        public IActionResult Authentication()
        {
            var data = new AuthenticationViewModel
            {
                Mode = "forgot-password"
            };

            return View("/Views/Authentication/Authentication.cshtml", data);
        }

        [Route("{culture}/in/forgot-password")]
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            var data = new ForgotPasswordViewModel();
            return PartialView(data);
        }

        [Route("{culture}/in/forgot-password")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel data)
        {
            ViewData["Email"] = data.Email;
            
            if (!ModelState.IsValid)
                return PartialView(data);

            if (!_emailValidatorService.IsValidEmail(data.Email))
            {
                ModelState.AddModelError("InvalidEmail", _localizer[AuthenticationErrorMessages.InvalidEmail]);
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

            string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            var userDTO = new UserDTO(AuthenticationUser);
            var saveToken = await _userPasswordRepository.SavePasswordTokenAsync(userDTO, token);

            if (!saveToken)
            {
                ModelState.AddModelError("ReminderFailed", _localizer[AuthenticationErrorMessages.PasswordReminderFailed]);
                return PartialView(data);
            }

            var domain = Request.Host.Value;
            domain += (Request.PathBase.Value.HasValue()) ? Request.PathBase.Value : string.Empty;
            var emailSubject = _forgotPasswordEmail.Subject();
            var emailBody = _forgotPasswordEmail.Body(AuthenticationUser, domain, emailSubject, token);

            await _emailSenderService.SendEmailAsync(data.Email, emailSubject, emailBody);

            data.SendReminderSuccess = true;

            return PartialView(data);
        }
    }
}