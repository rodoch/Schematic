using System;
using System.Linq;

namespace Schematic.Core
{

    public static class SchematicAttributeExtensions
    {
        public static TValue GetAttributeValue<TAttribute, TValue>(this Type type, Func<TAttribute, TValue> valueSelector) 
            where TAttribute : Attribute
        {
            var attribute = type.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault() as TAttribute;

            if (attribute != null)
            {
                return valueSelector(attribute);
            }

            return default(TValue);
        }
    }
}