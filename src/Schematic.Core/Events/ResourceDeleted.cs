using MediatR;

namespace Schematic.Core
{
    public class ResourceDeleted<T> : INotification, IResourceNotification<T>
    {
        public ResourceDeleted(int resourceID)
        {
            ResourceID = resourceID;
        }
    
        public int ResourceID { get; }
    }
}