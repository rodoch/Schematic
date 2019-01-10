using System.Collections.Generic;
using System.Threading.Tasks;

namespace Schematic.Core
{
    public interface IOrderedResourceRepository<T, TResourceFilter> : IResourceRepository<T, TResourceFilter>
    {
        Task<List<T>> GetOrderAsync(ResourceOrderModel<T> resourceOrderModel);

        Task<int> SetOrderAsync(ResourceOrderModel<T> resourceOrderModel);
    }
}