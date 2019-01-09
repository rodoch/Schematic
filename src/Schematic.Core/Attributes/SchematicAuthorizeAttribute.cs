using System;

namespace Schematic.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class SchematicAuthorizeAttribute : Attribute
    {
        public SchematicAuthorizeAttribute() {}

        public string Role { get; set; }
    }
}