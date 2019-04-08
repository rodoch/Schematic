using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AgileObjects.NetStandardPolyfills;
using MediatR;

namespace Schematic.Core.Mvc
{
    public class ResourceHandlerService
    {
        public ResourceHandlerService()
        {
            ResourceHandlerInterfaces = GetResourceHandlerInterfaces();
        }

        public List<Type> ResourceHandlerInterfaces { get; }

        private List<Type> GetResourceHandlerInterfaces()
        {
            return Assembly.GetEntryAssembly().GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract)
                .SelectMany(t => t.GetInterfaces().Where(i => 
                    i.IsClosedTypeOf(typeof(IRequestHandler<>)) || i.IsClosedTypeOf(typeof(IRequestHandler<,>))))
                .ToList();
        }
    }
}