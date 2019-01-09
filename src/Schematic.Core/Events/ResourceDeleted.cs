using MediatR;

namespace Schematic.Core
{
    public class ResourceDeleted<T> : INotification
    {
        public ResourceDeleted(int resourceID)
        {
            ResourceID = resourceID;
        }
    
        public int ResourceID { get; }
    }
}