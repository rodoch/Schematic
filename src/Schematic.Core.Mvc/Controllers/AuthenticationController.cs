using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Schematic.Identity;

namespace Schematic.Core.Mvc
{
    public class AuthenticationController : Controller
    {
        private readonly IPasswordHasherService<User> _passwordHasherService;
        private readonly IUserRepository<User, UserFilter, UserSpecification> _userRepository;
        private readonly IStringLocalizer<SignInViewModel> _localizer;

        protected User AuthenticationUser;

        public AuthenticationController(
            IPasswordHasherService<User> passwordHasherService,
            IUserRepository<User, UserFilter, UserSpecification> userRepository,
            IStringLocalizer<SignInViewModel> localizer)
        {
            _passwordHasherService = passwordHasherService;
            _userRepository = userRepository;
            _localizer = localizer;
        }
        
        protected bool IsValidUser { get; set; } = true;

        [BindProperty]
        public SignInViewModel SignInData { get; set; }

        [Route("{culture?}/{in?}")]
        [HttpGet]
        public IActionResult Authentication()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToRoute("default");
            }

            var data = new AuthenticationViewModel
            {
                Mode = "sign-in"
            };

            return View(data);
        }

        [Route("{culture}/in/sign-in")]
        [HttpGet]
        public IActionResult SignIn()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToRoute("default");

            return PartialView();
        }

        [Route("{culture}/in/sign-in")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(SignInViewModel data)
        {
            ViewData["Email"] = data.Email;
            
            if (!ModelState.IsValid)
                return PartialView();

            var userSpecification = new UserSpecification() { Email = data.Email };
            AuthenticationUser = await _userRepository.ReadAsync(userSpecification);

            if (AuthenticationUser is null)
            {
                ModelState.AddModelError("Invalid", _localizer[AuthenticationErrorMessages.InvalidData]);
                return PartialView();
            }

            var passwordVerification = _passwordHasherService.VerifyHashedPassword(
                    user: AuthenticationUser,
                    hashedPassword: AuthenticationUser.PassHash,
                    providedPassword: SignInData.Password);

            if (passwordVerification == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("Invalid", _localizer[AuthenticationErrorMessages.InvalidData]);
                return PartialView();
            }

            var identity = new ClaimsIdentity(
                    authenticationType: CookieAuthenticationDefaults.AuthenticationScheme,
                    nameType: ClaimTypes.Name,
                    roleType: ClaimTypes.Role);

            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, AuthenticationUser.ID.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Name, AuthenticationUser.FullName));
            identity.AddClaim(new Claim(ClaimTypes.Email, AuthenticationUser.Email));

            foreach (var role in AuthenticationUser.Roles.Where(r => r.HasRole))
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role.Name));
            }

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                scheme: CookieAuthenticationDefaults.AuthenticationScheme,
                principal: principal,
                properties: new AuthenticationProperties { IsPersistent = data.RememberMe });
            
            return Json(new { Route = Url.RouteUrl("default") });
        }

        [Route("{culture}/out")]
        [HttpPost]
        public async Task<IActionResult> SignOutAsync()
        { 
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Authentication");
        }
    }
}