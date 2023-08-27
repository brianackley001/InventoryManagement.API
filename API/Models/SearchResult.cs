using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace InventoryManagement.API.Models
{
    [ExcludeFromCodeCoverage]
    public class SearchResult
    {
        public enum SearchResultType
        {
            Item = 0,
            Group = 1,
            List = 2,
            Tag = 3
        };

        public int AmountValue { get; set; }
        public int Id { get; set; }
        public int ResultId { get; set; }
        public string ResultType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int ResultWeight { get; set; }
        public List<Tag> Tags { get; set; }
        public List<Group> Groups { get; set; }
    }
}
