using System.Collections.Generic;

namespace Schematic.Core
{
    public interface IResourceFilter<T>
    {
        int ActiveResourceID { get; set; }

        string Query { get; set; }
        
        string Facets { get; set; }
        
        Dictionary<string, string> FacetDictionary { get; set; }
    }
}