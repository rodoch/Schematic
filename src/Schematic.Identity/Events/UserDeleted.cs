using MediatR;

namespace Schematic.Identity
{
    public class UserDeleted: INotification
    {
        public UserDeleted(int userID)
        {
            UserID = userID;
        }

        public int UserID { get; }
    }
}