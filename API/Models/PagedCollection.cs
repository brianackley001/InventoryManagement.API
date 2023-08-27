namespace InventoryManagement.API.Models
{
    public class PagedCollection : IPagedCollection
    {
        public int CollectionTotal { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
