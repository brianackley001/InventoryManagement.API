using InventoryManagement.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryManagement.API.Managers
{
    public interface IItemManager
    {
        Task<RequestResponse<Item>> UpsertItem(Item Item);
        Task<RequestResponse<Item>> GetItem(int itemId);
        Task<RequestResponse<List<Item>>> GetItemCollection(int subscriptionId, SearchRequest searchRequest);
        Task<RequestResponse<List<Item>>> GetItemsByIdCollection(int subscriptionId, SearchRequest searchRequest);
        Task<RequestResponse<List<Item>>> GetLowQuantityItemCollection(int subscriptionId, SearchRequest searchRequest);
        Task<RequestResponse<List<StockItem>>> GetStockItems();
        Task<RequestResponse<bool>> UpsertStockItems(StockItemImport stockItemImport);
    }
}
