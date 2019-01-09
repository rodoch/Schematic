using MediatR;

namespace Schematic.Identity
{
    public class UserInvitation : INotification
    {
        public UserInvitation(UserDTO user)
        {
            User = user;
        }

        public UserDTO User { get; }
    }
}