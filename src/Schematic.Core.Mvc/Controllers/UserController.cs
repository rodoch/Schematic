using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Ansa.Extensions;
using MediatR;
using Schematic.Identity;

namespace Schematic.Core.Mvc
{
    [Route("{culture}/user")]
    [Authorize]
    public class UserController<TUser> : Controller where TUser : class, ISchematicUser, new()
    {
        private readonly IPasswordValidatorService _passwordValidatorService;
        private readonly IPasswordHasherService<TUser> _passwordHasherService;
        private readonly IEmailValidatorService _emailValidatorService;
        private readonly IUserRepository<TUser, UserFilter, UserSpecification> _userRepository;
        private readonly IUserRoleRepository<UserRole> _userRoleRepository;
        private readonly IMediator _mediator;
        private readonly IStringLocalizer<TUser> _localizer;

        public UserController(
            IPasswordValidatorService passwordValidatorService,
            IPasswordHasherService<TUser> passwordHasherService,
            IEmailValidatorService emailValidatorService,
            IUserRepository<TUser, UserFilter, UserSpecification> userRepository,
            IUserRoleRepository<UserRole> userRoleRepository,
            IMediator mediator,
            IStringLocalizer<TUser> localizer)
        {
            _passwordValidatorService = passwordValidatorService;
            _passwordHasherService = passwordHasherService;
            _emailValidatorService = emailValidatorService;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _mediator = mediator;
            _localizer = localizer;
        }
        
        protected ClaimsIdentity ClaimsIdentity => User.Identity as ClaimsIdentity;
        protected int UserID => int.Parse(ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        [HttpGet]
        public IActionResult Explorer(int id = 0)
        {
            var explorer = new ResourceExplorerModel()
            {
                ResourceID = id,
                ResourceName = typeof(TUser).Name.ToLower()
            };

            ViewData["ResourceName"] = "Users";

            return View(explorer);
        }

        [Route("create")]
        [HttpGet]
        public async Task<IActionResult> NewAsync()
        {   
            var result = new UserViewModel<TUser>() 
            { 
                User = new TUser()
            };

            result.User.Roles = await _userRoleRepository.ListAsync() ?? new List<UserRole>();

            return PartialView("_Editor", result);
        }

        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync(UserViewModel<TUser> userModel)
        {
            var roles = await _userRoleRepository.ListAsync();

            // populate the role list data not returned in post request 
            foreach (var userRole in userModel.User.Roles)
            {
                var role = roles.Where(r => r.ID == userRole.ID).FirstOrDefault();
                userRole.Name = role.Name;
                userRole.DisplayTitle = role.DisplayTitle;
            }
            
            var email = userModel.User.Email;

            // validate user e-mail address
            if (email.HasValue())
            {
                if (!_emailValidatorService.IsValidEmail(email))
                {
                    ModelState.AddModelError("InvalidEmail", _localizer[UserErrorMessages.InvalidEmail]);
                }

                var userSpecification = new UserSpecification()
                {
                    Email = email
                };

                var duplicateUser = await _userRepository.ReadAsync(userSpecification);

                if (duplicateUser != null)
                {
                    ModelState.AddModelError("DuplicateUser", _localizer[UserErrorMessages.DuplicateUser]);
                }
            }

            if (!ModelState.IsValid)
            {
                return PartialView("_Editor", userModel);
            }

            // persist new user to data store
            var newUserID = await _userRepository.CreateAsync(userModel.User, UserID);

            if (newUserID == 0)
            {
                return NoContent();
            }

            // publish user creation and invitation events so notification services can be called
            userModel.User.ID = newUserID;
            var userDTO = new UserDTO(userModel.User);
            var userCreatedEvent = new UserCreated(userDTO);
            await _mediator.Publish(userCreatedEvent);

            var uri = Url.Action("ReadAsync", "User", new { id = newUserID });
            return Created(uri, newUserID);
        }

        [Route("read")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> ReadAsync(int id)
        {
            var userSpecification = new UserSpecification()
            {
                ID = id
            };

            var user = await _userRepository.ReadAsync(userSpecification);

            if (user is null)
            {
                return NotFound();
            }
            
            var result = new UserViewModel<TUser>() 
            { 
                ID = id,
                User = user,
                IsVerified = user.PassHash.HasValue()
            };

            return PartialView("_Editor", result);
        }

        [Route("update")]
        [HttpPost]
        public async Task<IActionResult> UpdateAsync(UserViewModel<TUser> userModel)
        {
            var userSpecification = new UserSpecification()
            {
                ID = userModel.User.ID,
                Email = userModel.User.Email
            };

            var savedUser = await _userRepository.ReadAsync(userSpecification);
            var roles = await _userRoleRepository.ListAsync();

            if (savedUser.PassHash.HasValue())
            {
                userModel.IsVerified = true;
            }

            foreach (var userRole in userModel.User.Roles)
            {
                var role = roles.Where(r => r.ID == userRole.ID).FirstOrDefault();
                userRole.Name = role.Name;
                userRole.DisplayTitle = role.DisplayTitle;
            }
            
            var email = userModel.User.Email;

            if (email.HasValue())
            {
                if (!_emailValidatorService.IsValidEmail(email))
                {
                    ModelState.AddModelError("InvalidEmail", _localizer[UserErrorMessages.InvalidEmail]);
                }

                if (email != savedUser.Email)
                {
                    var duplicateUser = await _userRepository.ReadAsync(userSpecification);

                    if (duplicateUser != null)
                    {
                        ModelState.AddModelError("DuplicateUser", _localizer[UserErrorMessages.DuplicateUser]);
                    }
                }
            }

            if (userModel.HasIdentity(UserID) && userModel.Password.HasValue()
                || userModel.HasIdentity(UserID) && userModel.ConfirmationPassword.HasValue())
            {
                if (userModel.Password.IsNullOrWhiteSpace() || userModel.ConfirmationPassword.IsNullOrWhiteSpace())
                {
                    ModelState.AddModelError("Invalid", _localizer[UserErrorMessages.TwoPasswordsRequired]);
                    return PartialView("_Editor", userModel);
                }

                if (userModel.Password != userModel.ConfirmationPassword)
                {
                    ModelState.AddModelError("Invalid", _localizer[UserErrorMessages.PasswordsDoNotMatch]);
                    return PartialView("_Editor", userModel);
                }

                var passwordValidationErrors = _passwordValidatorService.ValidatePassword(userModel.Password);

                if (passwordValidationErrors.Count > 0)
                {
                    ModelState.AddModelError(
                        key: "PasswordValidationErrors",
                        errorMessage: _localizer[_passwordValidatorService.GetPasswordValidationErrorMessage()]
                    );
                    return PartialView("_Editor", userModel);
                }

                var passHash = _passwordHasherService.HashPassword(userModel.User, userModel.Password);
                userModel.User.PassHash = passHash;
            }
            else
            {
                userModel.User.PassHash = savedUser.PassHash;
            }

            if (!ModelState.IsValid)
            {
                return PartialView("_Editor", userModel);
            }

            var update = await _userRepository.UpdateAsync(userModel.User, UserID);

            if (update <= 0)
            {
                return BadRequest();
            }

            var updatedUser = await _userRepository.ReadAsync(userSpecification);

            var userDTO = new UserDTO(updatedUser);
            var userUpdatedEvent = new UserUpdated(userDTO);
            await _mediator.Publish(userUpdatedEvent);
            
            var result = new UserViewModel<TUser>() 
            { 
                ID = userModel.ID,
                User = updatedUser,
                IsVerified = updatedUser.PassHash.HasValue()
            };
            
            return PartialView("_Editor", result);
        }

        [Route("delete")]
        [HttpPost]
        public async Task<IActionResult> DeleteAsync(int id)
        {   
            var deleteUser = await _userRepository.DeleteAsync(id, UserID);

            if (deleteUser <= 0)
            {
                return BadRequest();
            }

            var userDeletedEvent = new UserDeleted(id);
            await _mediator.Publish(userDeletedEvent);

            return NoContent();
        }

        [Route("filter")]
        [HttpGet]
        public IActionResult Filter()
        {
            var filter = new UserFilter();
            return PartialView("_Filter", filter);
        }

        [Route("list")]
        [HttpPost]
        public async Task<IActionResult> ListAsync(UserFilter filter)
        {
            var list = await _userRepository.ListAsync(filter);

            if (list.Count == 0)
            {
                return NoContent();
            }

            var resourceList = new ResourceListModel<TUser>()
            {
                List = list,
                ActiveResourceID = filter.ActiveResourceID
            };

            return PartialView("_List", resourceList);
        }

        [Route("invite")]
        [HttpPost]
        public async Task<IActionResult> InviteAsync(int userID)
        {
            var userSpecification = new UserSpecification()
            {
                ID = userID
            };

            var user = await _userRepository.ReadAsync(userSpecification);
            var userDTO = new UserDTO(user);
            var userInvitationEvent = new UserInvitation(userDTO);
            await _mediator.Publish(userInvitationEvent);

            return Ok();
        }
    }
}