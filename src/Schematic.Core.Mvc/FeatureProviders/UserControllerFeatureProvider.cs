using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Schematic.Identity;

namespace Schematic.Core.Mvc
{
    public class UserControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            var schematicAssembly = Assembly.Load("Schematic");
            var candidates = new List<Type>();

            foreach (var assemblyName in schematicAssembly.GetReferencedAssemblies()) 
            {
                var assembly = Assembly.Load(assemblyName);
                var exportedTypes = assembly.GetExportedTypes()
                    .Where(type => type.GetCustomAttributes<SchematicUserAttribute>().Any());
                
                candidates.AddRange(exportedTypes);
            }
            
            feature.Controllers.Add(typeof(UserController<>).MakeGenericType(candidates[0]).GetTypeInfo());
        }
    }
}