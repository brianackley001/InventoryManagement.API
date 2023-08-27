

namespace InventoryManagement.API.DataProvider
{
    public interface IRequestResponse
    {
        bool Success { get; set; }
        object Item { get; set; }
        Models.PagedCollection PagedCollection { get; set; }
    }
}
