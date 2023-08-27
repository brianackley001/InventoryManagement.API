using InventoryManagement.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace InventoryManagement.API.DataProvider.SQL
{
    public interface IItemRepository
    {
        Task<RequestResponse<Item>> GetItem(int itemId);
        Task<RequestResponse<ItemCollectionDataCollection>> GetItemCollection(int subscriptionId, SearchRequest searchRequest);
        Task<RequestResponse<ItemByAttributeSearchDataCollection>> GetItemsByIdCollection(int subscriptionId, SearchRequest searchRequest);
        Task<RequestResponse<ItemCollectionDataCollection>> GetLowQuantityItemCollection(int subscriptionId, SearchRequest searchRequest);
        Task<int> UpsertItem(Item item);
        Task<RequestResponse<List<StockItem>>> GetStockItems();
        Task<int> UpsertStockItems(StockItemImport stockItemImport);
    }
}
