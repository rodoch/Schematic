using MediatR;

namespace Schematic.Core
{
    public class ResourceCreated<T> : INotification
    {
        public ResourceCreated(T resource)
        {
            Resource = resource;
        }
    
        public T Resource { get; }
    }
}