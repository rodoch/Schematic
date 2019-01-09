using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
 
namespace Schematic.Core.Mvc
{
    public class LocalizerService : IStringLocalizer
    {
        private readonly IStringLocalizer _resourceManagerStringLocalizer;
        private readonly IStringLocalizer _sharedLocalizer;
        
        public LocalizerService(
            IStringLocalizer resourceManagerStringLocalizerByType, 
            IStringLocalizer sharedLocalizer)
        {
            _resourceManagerStringLocalizer = resourceManagerStringLocalizerByType;
            _sharedLocalizer = sharedLocalizer;
        }
 
        public LocalizedString this[string name]
        {
            get
            {
                var result = _resourceManagerStringLocalizer.GetString(name);

                if (result.ResourceNotFound)
                {
                    result = _sharedLocalizer.GetString(name);
                }

                return result;
            }
        }
 
        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var result = _resourceManagerStringLocalizer.GetString(name, arguments);
                
                if (result.ResourceNotFound)
                {
                    result = _sharedLocalizer.GetString(name, arguments);
                }

                return result;
            }
        }
 
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var result = _resourceManagerStringLocalizer.GetAllStrings(includeParentCultures);

            foreach (var item in result)
            {
                if (item.ResourceNotFound)
                {
                    var i = _sharedLocalizer.GetString(item.Name);
                    yield return i;
                }

                yield return item;
            }
        }
 
        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return _resourceManagerStringLocalizer.WithCulture(culture);
        }
    }
}