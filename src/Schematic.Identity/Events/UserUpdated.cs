using MediatR;

namespace Schematic.Identity
{
    public class UserUpdated : INotification
    {
        public UserUpdated(UserDTO user)
        {
            User = user;
        }

        public UserDTO User { get; }
    }
}