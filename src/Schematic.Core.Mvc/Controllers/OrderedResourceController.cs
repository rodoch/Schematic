using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MediatR;

namespace Schematic.Core.Mvc
{
    [Route("{culture}/resource/[controller]")]
    [Authorize]
    public class OrderedResourceController<TResource, TFilter> : ResourceController<TResource, TFilter>
        where TResource : class, new()
        where TFilter : IResourceFilter<TResource>, new()
    {
        private readonly IOrderedResourceRepository<TResource, TFilter> _repository;
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public OrderedResourceController(
            IOrderedResourceRepository<TResource, TFilter> repository,
            IMediator mediator,
            ILogger<ResourceController<TResource, TFilter>> logger) : base(repository, mediator, logger)
        {
            _repository = repository;
            _mediator = mediator;
            _logger = logger;
        }

        [Route("order")]
        [HttpGet]
        public async virtual Task<IActionResult> GetOrderAsync(string facets = "")
        {
            if (!User.IsAuthorized(ResourceType)) 
            {
                return Unauthorized();
            }

            var model = new ResourceOrderModel<TResource>()
            {
                Facets = facets
            };

            var resourceList = await _repository.GetOrderAsync(model);

            if (resourceList is null)
            {
                return NotFound();
            }

            model.Resources = resourceList;

            return PartialView("_Order", model);
        }

        [Route("order")]
        [HttpPost]
        public async virtual Task<IActionResult> SetOrderAsync(ResourceOrderModel<TResource> resourceOrderModel)
        {
            if (!User.IsAuthorized(ResourceType)) 
            {
                return Unauthorized();
            }
            
            var orderResourceEvent = new OrderResource<TResource>(resourceOrderModel);
            var context = await _mediator.Send(orderResourceEvent);

            if (context != null)
            {
                resourceOrderModel = context;
            }

            if (resourceOrderModel.Errors.Any())
            {
                foreach (var error in resourceOrderModel.Errors)
                {
                    ModelState.AddModelError(error.Key, error.Value);
                }
            }

            if (!ModelState.IsValid)
            {
                return PartialView("_Order", resourceOrderModel);
            }

            var update = await _repository.SetOrderAsync(resourceOrderModel);

            if (update <= 0)
            {
                return BadRequest();
            }

            _logger.LogResourceOrdered(ResourceType);
            
            var resourceOrderedEvent = new ResourceOrdered<TResource>();
            await _mediator.Publish(resourceOrderedEvent);
            var resourcePersistedEvent = new ResourcePersisted<TResource>();
            await _mediator.Publish(resourcePersistedEvent);

            return Ok();
        }
    }
}