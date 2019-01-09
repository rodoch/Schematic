using System;

namespace Schematic.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SchematicRepeatableAttribute : Attribute
    {
        public SchematicRepeatableAttribute(string route = "")
        {
            Route = route;
        }
            
        public string Route { get; set; }
    }
}