using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ansa.Extensions;
using MediatR;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NJsonSchema;

namespace Schematic.Core.Mvc
{
    [Route("{culture}/resource/[controller]")]
    [Authorize]
    public class ResourceController<TResource, TFilter> : Controller
        where TResource : class, new()
        where TFilter : IResourceFilter<TResource>, new()
    {
        private readonly IResourceRepository<TResource, TFilter> _repository;
        private readonly IMediator _mediator;
        private readonly ILogger _logger;

        public ResourceController(
            IResourceRepository<TResource, TFilter> repository,
            IMediator mediator,
            ILogger<ResourceController<TResource, TFilter>> logger)
        {
            _repository = repository;
            _mediator = mediator;
            _logger = logger;
        }

        protected ClaimsIdentity ClaimsIdentity => User.Identity as ClaimsIdentity;
        protected int UserID => int.Parse(ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        
        public static Type ResourceType = typeof(TResource);
        public static string ResourceName = ResourceType.GetAttributeValue((SchematicResourceAttribute r) => r.ControllerName).HasValue() 
            ? ResourceType.GetAttributeValue((SchematicResourceAttribute r) => r.ControllerName).ToLower()
            : ResourceType.Name.ToLower();

        [HttpGet]
        public virtual IActionResult Explorer(int id = 0, string name = "", string facets = "")
        {
            if (!User.IsAuthorized(ResourceType))
                return Unauthorized();

            var explorer = new ResourceExplorerModel()
            {
                ResourceID = id,
                ResourceName = ResourceName,
                Facets = facets
            };

            var resourceName = (name.HasValue()) ? name : ResourceName;
            ViewData["ResourceName"] = resourceName;

            return View(explorer);
        }

        [Route("create")]
        [HttpGet]
        public async virtual Task<IActionResult> NewAsync(string facets = "")
        {
            if (!User.IsAuthorized(ResourceType))
                return Unauthorized();
            
            var resourceModel = new ResourceModel<TResource>() 
            {
                Resource = new TResource(),
                Facets = facets
            };

            var newResourceEvent = new NewResource<TResource>(resourceModel);
            var context = await _mediator.Send(newResourceEvent);

            if (context != null)
                resourceModel = context;

            return PartialView("_Editor", resourceModel);
        }

        [Route("create")]
        [HttpPost]
        public async virtual Task<IActionResult> CreateAsync(ResourceModel<TResource> resourceModel)
        {
            if (!User.IsAuthorized(ResourceType))
                return Unauthorized();
            
            var createResourceEvent = new CreateResource<TResource>(resourceModel);
            var context = await _mediator.Send(createResourceEvent);

            if (context != null)
                resourceModel = context;

            if (resourceModel.Errors.Any())
            {
                foreach (var error in resourceModel.Errors)
                    ModelState.AddModelError(error.Key, error.Value);
            }
            
            if (!ModelState.IsValid)
                return PartialView("_Editor", resourceModel);

            int newResourceID = await _repository.CreateAsync(resourceModel.Resource, UserID);

            if (newResourceID == 0)
                return NoContent();

            _logger.LogResourceCreated(ResourceType, resourceModel.ID);

            var resourceCreatedEvent = new ResourceCreated<TResource>(resourceModel.Resource);
            await _mediator.Publish(resourceCreatedEvent);
            var resourcePersistedEvent = new ResourcePersisted<TResource>();
            await _mediator.Publish(resourcePersistedEvent);

            var controllerName = ControllerContext.RouteData.Values["controller"].ToString();
            return Created(Url.Action("ReadAsync", controllerName, new { id = newResourceID }), newResourceID);
        }

        [Route("read")]
        [HttpGet("{id:int}")]
        public async virtual Task<IActionResult> ReadAsync(int id, string facets = "")
        {
            if (!User.IsAuthorized(ResourceType))
                return Unauthorized();

            var resource = await _repository.ReadAsync(id);

            if (resource is null)
                return NotFound();
            
            var resourceModel = new ResourceModel<TResource>()
            { 
                ID = id,
                Resource = resource,
                Facets = facets
            };

            return PartialView("_Editor", resourceModel);
        }

        [Route("update")]
        [HttpPost]
        public async virtual Task<IActionResult> UpdateAsync(ResourceModel<TResource> resourceModel)
        {
            if (!User.IsAuthorized(ResourceType))
                return Unauthorized();
            
            var updateResourceEvent = new UpdateResource<TResource>(resourceModel);
            var context = await _mediator.Send(updateResourceEvent);

            if (context != null)
                resourceModel = context;

            if (resourceModel.Errors.Any())
            {
                foreach (var error in resourceModel.Errors)
                    ModelState.AddModelError(error.Key, error.Value);
            }
            
            if (!ModelState.IsValid)
                return PartialView("_Editor", resourceModel);

            var update = await _repository.UpdateAsync(resourceModel.Resource, UserID);

            if (update <= 0)
                return BadRequest();

            _logger.LogResourceUpdated(ResourceType, resourceModel.ID);

            var updatedResource = await _repository.ReadAsync(resourceModel.ID);
            
            var resourceUpdatedEvent = new ResourceUpdated<TResource>(updatedResource);
            await _mediator.Publish(resourceUpdatedEvent);
            var resourcePersistedEvent = new ResourcePersisted<TResource>();
            await _mediator.Publish(resourcePersistedEvent);

            var result = new ResourceModel<TResource>() 
            { 
                ID = resourceModel.ID,
                Resource = updatedResource 
            };

            return PartialView("_Editor", result);
        }

        [Route("delete")]
        [HttpPost]
        public async virtual Task<IActionResult> DeleteAsync(int id)
        {   
            if (!User.IsAuthorized(ResourceType))
                return Unauthorized();
            
            var deleteResourceEvent = new DeleteResource<TResource>(id);
            var cancelDelete = await _mediator.Send(deleteResourceEvent);

            if (cancelDelete)
                return Forbid();

            var delete = await _repository.DeleteAsync(id, UserID);

            if (delete <= 0)
                return BadRequest();

            _logger.LogResourceDeleted(ResourceType, id);

            var resourceDeletedEvent = new ResourceDeleted<TResource>(id);
            await _mediator.Publish(resourceDeletedEvent);
            var resourcePersistedEvent = new ResourcePersisted<TResource>();
            await _mediator.Publish(resourcePersistedEvent);

            return NoContent();
        }

        [Route("filter")]
        [HttpGet]
        public virtual IActionResult Filter(string facets = "")
        {
            if (!User.IsAuthorized(ResourceType))
                return Unauthorized();

            var filter = new TFilter()
            {
                Facets = facets
            };

            return PartialView("_Filter", filter);
        }

        [Route("list")]
        [HttpPost]
        public async virtual Task<IActionResult> ListAsync(TFilter filter)
        {   
            if (!User.IsAuthorized(ResourceType))
                return Unauthorized();

            var list = await _repository.ListAsync(filter);

            if (list.Count == 0)
                return NoContent();

            var resourceList = new ResourceListModel<TResource>()
            {
                List = list,
                ActiveResourceID = filter.ActiveResourceID
            };

            return PartialView("_List", resourceList);
        }

        [Route("schema")]
        [HttpGet]
        public virtual async Task<IActionResult> SchemaAsync()
        {
            if (!User.IsAuthorized(ResourceType))
                return Unauthorized();

            var schema = await JsonSchema4.FromTypeAsync<TResource>();
            var schemaData = schema;
            var serializerSettings = new JsonSerializerSettings();
            serializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            return Json(schemaData, serializerSettings);
        }
    }
}