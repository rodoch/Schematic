using MediatR;

namespace Schematic.Core
{
    public class CreateResource<T> : IRequest<ResourceModel<T>>
    {
        public CreateResource(ResourceModel<T> model)
        {
            Model = model;
        }

        public ResourceModel<T> Model { get; }
    }
}