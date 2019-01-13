using System.Collections.Generic;
using Ansa.Extensions;

namespace Schematic.Core
{
    public static class SchematicExtensions
    {
        public static Dictionary<string, string> GetFacets(this string facets)
        {
            var result = new Dictionary<string, string>();

            if (facets.HasValue())
            {
                string[] sets = facets.Split(';');

                foreach (string set in sets)
                {
                    string[] facet = set.Split(':');
                    result.Add(facet[0], facet[1]);
                }
            }

            return result;
        }
    }
}