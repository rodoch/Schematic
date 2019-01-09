using System;
using System.Resources;

namespace Schematic.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SchematicResourceNameAttribute : Attribute
    {
        public SchematicResourceNameAttribute() {}

        public string Name { get; set; }
    }
}