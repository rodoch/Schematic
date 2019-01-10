using System;

namespace Schematic.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SchematicResourceAttribute : Attribute
    {
        public SchematicResourceAttribute(string controllerName = "", bool ordered = false, string route = "")
        {
            ControllerName = controllerName;
            Ordered = ordered;
            Route = route;
        }
            
        public string ControllerName { get; set; }

        public bool Ordered { get; set; }

        public string Route { get; set; }
    }
}