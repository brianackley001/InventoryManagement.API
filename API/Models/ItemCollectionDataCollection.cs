using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace InventoryManagement.API.Models
{
    [ExcludeFromCodeCoverage]
    public class ItemCollectionDataCollection
    {
        public List<Item> Items { get; set; }
        public List<ItemTag> ItemTags { get; set; }
        public List<ItemGroup> ItemGroups { get; set; }
    }
}
