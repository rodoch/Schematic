using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
 
namespace Schematic.Core.Mvc
{
    public class DataAnnotationLocalizer : IStringLocalizer
    {
        private readonly IStringLocalizer _resourceManagerStringLocalizer;
        private readonly IStringLocalizer _sharedLocalizer;
        
        public DataAnnotationLocalizer(Type type, Type sharedResourceType, IStringLocalizerFactory factory)
        {
            _resourceManagerStringLocalizer = factory.Create(type);
            _sharedLocalizer = factory.Create(sharedResourceType);
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