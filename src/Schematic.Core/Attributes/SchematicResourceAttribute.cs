using System;

namespace Schematic.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SchematicResourceAttribute : Attribute
    {
        public SchematicResourceAttribute(string controllerName = "", string route = "")
        {
            ControllerName = controllerName;
            Route = route;
        }
            
        public string ControllerName { get; set; }
        public string Route { get; set; }
    }
}