using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AgileObjects.NetStandardPolyfills;
using MediatR;

namespace Schematic.Core.Mvc
{
    public class ResourcePipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private ResourceHandlerService _resourceHandlerService;

        public ResourcePipelineBehaviour(ResourceHandlerService resourceHandlerService)
        {
            _resourceHandlerService = resourceHandlerService;
        }
        
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var handlerInterfaces = _resourceHandlerService.ResourceHandlerInterfaces;
            var resultType = typeof(TRequest).GetInterfaces()
                .Single(i => i.IsClosedTypeOf(typeof(IRequest<>)) && i.GetGenericArguments().Any())
                .GetGenericArguments()
                .First();
            var handlerType = (resultType == typeof(Unit))
                ? typeof(IRequestHandler<>).MakeGenericType(typeof(TRequest))
                : typeof(IRequestHandler<,>).MakeGenericType(typeof(TRequest), resultType);

            if (!handlerInterfaces.Any(t => t == handlerType))
            {
                return default(TResponse);
            }

            return await next();
        }
    }
}