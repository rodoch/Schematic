using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Ansa.Extensions;

namespace Schematic.Core.Mvc
{
    public class ResourceControllerRouteConvention : IControllerModelConvention
    {
        protected string DetermineResourceControllerName(Type genericType, SchematicResourceAttribute attribute)
        {
            return (attribute != null && attribute.ControllerName.HasValue()) 
                ? attribute.ControllerName
                : genericType.Name;
        }

        public void Apply(ControllerModel controller)
        {
            if (!controller.ControllerType.IsGenericType)
                return;

            var genericType = controller.ControllerType.GenericTypeArguments[0];
            var customNameAttribute = genericType.GetCustomAttribute<SchematicResourceAttribute>();
                
            controller.ControllerName = DetermineResourceControllerName(genericType, customNameAttribute);
    
            if (customNameAttribute != null && customNameAttribute.Route.HasValue())
            {
                var routeAttribute = new RouteAttribute(customNameAttribute.Route);

                controller.Selectors.Add(new SelectorModel
                {
                    AttributeRouteModel = new AttributeRouteModel(routeAttribute),
                });
            }
        }
    }
}