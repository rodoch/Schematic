using MediatR;

namespace Schematic.Core
{
    public class DeleteResource<T> : IRequest<bool>
    {
        public DeleteResource(int resourceID)
        {
            ResourceID = resourceID;
        }

        public int ResourceID { get; }
    }
}