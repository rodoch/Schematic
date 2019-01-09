using MediatR;

namespace Schematic.Core
{
    public class UpdateResource<T> : IRequest<ResourceModel<T>>
    {
        public UpdateResource(ResourceModel<T> model)
        {
            Model = model;
        }

        public ResourceModel<T> Model { get; }
    }
}