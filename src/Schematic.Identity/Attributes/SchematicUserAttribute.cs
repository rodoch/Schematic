using System;

namespace Schematic.Identity
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SchematicUserAttribute : Attribute
    {
        public SchematicUserAttribute() {}
    }
}