using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Schematic.Identity;

namespace Schematic.Core.Mvc
{
    public class UserCreatedEffect : INotificationHandler<UserCreated>
    {
        private readonly IMediator _mediator;

        public UserCreatedEffect(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(UserCreated notification, CancellationToken cancellationToken)
        {
            var userInvitationEvent = new UserInvitation(notification.User);
            await _mediator.Publish(userInvitationEvent);
        }
    }
}