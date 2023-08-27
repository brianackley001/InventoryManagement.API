namespace InventoryManagement.API.Models
{
    public interface IPagedCollection
    {
        public int CollectionTotal { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
