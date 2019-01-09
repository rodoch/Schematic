using System.Collections.Generic;

namespace Schematic.Core
{
    public class ResourceModel<T>
    {
        public ResourceModel()
        {
            _errors = new List<KeyValuePair<string, string>>();
        }

        public int ID { get; set; }

        public T Resource { get; set; }

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

        public List<KeyValuePair<string, string>> _errors { get; set; }
        public IEnumerable<KeyValuePair<string, string>> Errors { get => _errors; }

        protected void UpdateFacetDictionary()
        {
            FacetDictionary = _facets.GetFacets();
        }

        public void AddModelError(string key, string errorMessage)
        {
            var error = new KeyValuePair<string, string>(key, errorMessage);
            _errors.Add(error);
        }
    }
}