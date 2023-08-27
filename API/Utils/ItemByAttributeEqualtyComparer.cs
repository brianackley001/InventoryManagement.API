using InventoryManagement.API.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace InventoryManagement.API.Utils
{
    public class ItemByAttributeEqualtyComparer : IEqualityComparer<SearchResult>
    {
        public bool Equals([AllowNull] SearchResult x, [AllowNull] SearchResult y)
        {
            // Two items are equal if their keys are equal.
            return x.Id == y.Id;
        }

        public int GetHashCode([DisallowNull] SearchResult obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
