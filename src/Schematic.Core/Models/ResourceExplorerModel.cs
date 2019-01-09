using System.Collections.Generic;

namespace Schematic.Core
{
    public class ResourceExplorerModel
    {
        public int? ResourceID { get; set; }

        public string ResourceName { get; set; }

        private string _facets;
        public string Facets
        {
            get => _facets;
            set
            {
                _facets = value;
                UpdateFacetDictionary();
            }
        }

        public Dictionary<string, string> FacetDictionary { get; set; }

        protected void UpdateFacetDictionary()
        {
            FacetDictionary = _facets.GetFacets();
        }
    }
}