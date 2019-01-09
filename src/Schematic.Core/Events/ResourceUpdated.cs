using MediatR;

namespace Schematic.Core
{
    public class ResourceUpdated<T> : INotification
    {
        public ResourceUpdated(T resource)
        {
            Resource = resource;
        }
    
        public T Resource { get; }
    }
}