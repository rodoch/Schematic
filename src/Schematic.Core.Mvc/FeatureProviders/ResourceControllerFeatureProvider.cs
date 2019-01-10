using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Schematic.Core.Mvc
{
    public class ResourceControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            var schematicAssembly = Assembly.GetEntryAssembly();
            var resources = new List<Type>();
            var orderedResources = new List<Type>();

            // Get resources defined within Schematic assembly
            var schematicAssemblyExportedTypes = schematicAssembly.GetExportedTypes()
                .Where(type => type.GetCustomAttributes<SchematicResourceAttribute>().Where(a => !a.Ordered).Any());
            var schematicAssemblyExportedOrderedTypes = schematicAssembly.GetExportedTypes()
                .Where(type => type.GetCustomAttributes<SchematicResourceAttribute>().Where(a => a.Ordered).Any());

            foreach (var type in schematicAssemblyExportedTypes)
            {
                resources.Add(type);
            }

            foreach (var type in schematicAssemblyExportedOrderedTypes)
            {
                orderedResources.Add(type);
            }

            // Get resources defined in referenced assemblies
            foreach (var assemblyName in schematicAssembly.GetReferencedAssemblies()) 
            {
                var assembly = Assembly.Load(assemblyName);
                var exportedTypes = assembly.GetExportedTypes()
                    .Where(type => type.GetCustomAttributes<SchematicResourceAttribute>().Where(a => !a.Ordered).Any());
                var exportedTypesOrdered = assembly.GetExportedTypes()
                    .Where(type => type.GetCustomAttributes<SchematicResourceAttribute>().Where(a => a.Ordered).Any());
                    
                foreach (var type in exportedTypes) 
                {
                    resources.Add(type);
                }

                foreach (var type in exportedTypesOrdered)
                {
                    orderedResources.Add(type);
                }
            }
                
            // Generate resource controllers
            foreach (var resource in resources)
            {
                string typeName = resource.Name;

                Type filterType;
                //TODO: Find filters based on interface implementation rather than naming convention
                filterType = schematicAssembly.GetType("Schematic.Filters." + typeName + "Filter");

                if (filterType is null)
                {
                    filterType = typeof(ResourceFilter<>).MakeGenericType(resource).GetTypeInfo();
                }

                feature.Controllers.Add(typeof(ResourceController<,>)
                    .MakeGenericType(resource, filterType)
                    .GetTypeInfo());
            }

            foreach (var resource in orderedResources)
            {
                string typeName = resource.Name;

                Type filterType;
                //TODO: Find filters based on interface implementation rather than naming convention
                filterType = schematicAssembly.GetType("Schematic.Filters." + typeName + "Filter");

                if (filterType is null)
                {
                    filterType = typeof(ResourceFilter<>).MakeGenericType(resource).GetTypeInfo();
                }

                feature.Controllers.Add(typeof(OrderedResourceController<,>)
                    .MakeGenericType(resource, filterType)
                    .GetTypeInfo());
            }
        }
    }
}