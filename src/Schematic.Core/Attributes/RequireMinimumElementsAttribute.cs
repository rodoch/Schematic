using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Schematic.Core.Attributes
{
    public class RequireMinimumItemsAttribute : ValidationAttribute
    {
        private readonly int _minimumItems;

        public RequireMinimumItemsAttribute(int minimumItems) => _minimumItems = minimumItems;

        public override bool IsValid(object value)
        {
            var list = value as IList;
            
            if (list is null)
            {
                return false;
            }
            
            return list.Count >= _minimumItems;
        }
    }
}