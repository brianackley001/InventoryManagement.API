using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace InventoryManagement.API.Models
{
    [ExcludeFromCodeCoverage]
    public class SearchResultDataCollection
    {
        public List<SearchResult> SearchResults { get; set; }
        public List<ItemTag> ItemTags { get; set; }
        public List<ItemGroup> ItemGroups { get; set; }
        public List<Group> Groups { get; set; }
        public List<Tag> Tags { get; set; }
    }
}
