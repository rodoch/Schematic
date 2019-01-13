using MediatR;

namespace Schematic.Core
{
    public class ResourceCreated<T> : INotification, IResourceNotification<T>
    {
        public ResourceCreated(T resource)
        {
            Resource = resource;
        }
    
        public T Resource { get; }
    }
}