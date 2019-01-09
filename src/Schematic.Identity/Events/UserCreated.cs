using MediatR;

namespace Schematic.Identity
{
    public class UserCreated : INotification
    {
        public UserCreated(UserDTO user)
        {
            User = user;
        }

        public UserDTO User { get; }
    }
}