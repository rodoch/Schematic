using MediatR;

namespace Schematic.Core
{
    public class ResourceUpdated<T> : INotification, IResourceNotification<T>
    {
        public ResourceUpdated(T resource)
        {
            Resource = resource;
        }
    
        public T Resource { get; }
    }
}