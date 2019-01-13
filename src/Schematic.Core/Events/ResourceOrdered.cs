using MediatR;

namespace Schematic.Core
{
    public class ResourceOrdered<T> : INotification, IResourceNotification<T>
    {
    }
}