using MediatR;

namespace Schematic.Core
{
    public class OrderResource<T> : IRequest<ResourceOrderModel<T>>
    {
        public OrderResource(ResourceOrderModel<T> model)
        {
            Model = model;
        }

        public ResourceOrderModel<T> Model { get; }
    }
}