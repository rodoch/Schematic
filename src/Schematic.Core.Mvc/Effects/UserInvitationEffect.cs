using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Schematic.Identity;

namespace Schematic.Core.Mvc
{
    public class UserInvitationEffect : INotificationHandler<UserInvitation>
    {
        private readonly IEmailSenderService _emailSenderService;
        private readonly IUserInvitationEmail<UserDTO> _userInvitationEmail;
        private readonly IUserPasswordRepository<UserDTO> _userPasswordRepository;
        private readonly SchematicUrlProvider _urlProvider;

        public UserInvitationEffect(
            IEmailSenderService emailSenderService,
            IUserInvitationEmail<UserDTO> userInvitationEmail,
            IUserPasswordRepository<UserDTO> userPasswordRepository,
            SchematicUrlProvider urlProvider)
        {
            _emailSenderService = emailSenderService;
            _userInvitationEmail = userInvitationEmail;
            _userPasswordRepository = userPasswordRepository;
            _urlProvider = urlProvider;
        }

        public async Task Handle(UserInvitation notification, CancellationToken cancellationToken)
        {
            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            var domain = _urlProvider.GetBaseUrl();
            var createPasswordToken = await _userPasswordRepository.CreatePasswordTokenAsync(notification.User, token);

            if (!createPasswordToken)
                return;

            var emailSubject = _userInvitationEmail.Subject();
            var emailBody = _userInvitationEmail.Body(notification.User, domain, emailSubject, token);
            await _emailSenderService.SendEmailAsync(notification.User.Email, emailSubject, emailBody);
        }
    }
}