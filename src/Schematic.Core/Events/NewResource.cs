using MediatR;

namespace Schematic.Core
{
    public class NewResource<T> : IRequest<ResourceModel<T>>
    {
        public NewResource(ResourceModel<T> model)
        {
            Model = model;
        }

        public ResourceModel<T> Model { get; }
    }
}