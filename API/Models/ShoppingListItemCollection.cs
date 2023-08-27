using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace InventoryManagement.API.Models
{
    [ExcludeFromCodeCoverage]
    public class ShoppingListItemCollection
    {
        public List<ShoppingListItem> ShoppingListItems { get; set; }
        public List<ItemTag> ItemTags { get; set; }
        public List<ItemGroup> ItemGroups { get; set; }
    }
}
