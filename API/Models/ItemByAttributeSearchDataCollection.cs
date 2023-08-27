using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace InventoryManagement.API.Models
{
    [ExcludeFromCodeCoverage]
    public class ItemByAttributeSearchDataCollection
    {
        public ItemByAttributeSearchDataCollection(List<Item> items, [AllowNull] List<Tag> tags, [AllowNull] List<ItemTag> itemTags, [AllowNull] List<Group> groups, [AllowNull] List<ItemGroup>  itemGroups)
        {
            Items = items;
            Tags = tags;
            ItemTags = itemTags;
            ItemGroups = itemGroups;
            Groups = groups;
        }

        public List<Item> Items { get; set; }
        public List<Tag> Tags { get; set; }
        public List<Group> Groups { get; set; }
        public List<ItemTag> ItemTags { get; set; }
        public List<ItemGroup> ItemGroups { get; set; }
    }
}
