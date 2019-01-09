using Microsoft.AspNetCore.Http;
using Ansa.Extensions;

namespace Schematic.Core.Mvc
{
    public class SchematicUrlProvider
    {
        private readonly IHttpContextAccessor _accessor;

        public SchematicUrlProvider(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public string GetBaseUrl()
        {
            var request = _accessor.HttpContext.Request;
            var domain = request.Host.Value;
            domain += (request.PathBase.Value.HasValue()) ? request.PathBase.Value : string.Empty;
            return domain;
        }
    }
}