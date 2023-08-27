using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace InventoryManagement.API.Models
{
    [ExcludeFromCodeCoverage]
    public class SearchRequest
    {
        public PagedCollection PagedCollection { get; set; }
        public string SearchTerm { get; set; }
        public string SearchType { get; set; }
        public List<int> IdCollection { get; set; }
        public string SortBy { get; set; }
        public bool SortAscending { get; set; }
    }
}
