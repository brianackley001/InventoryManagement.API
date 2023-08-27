using InventoryManagement.API.Models;
using System.Threading.Tasks;


namespace InventoryManagement.API.DataProvider.SQL
{
    public interface ISearchRepository
    {
        Task<RequestResponse<SearchResultDataCollection>> GetSearchResultCollection(int subscriptionId, SearchRequest searchRequest);
        Task<RequestResponse<SearchResultDataCollection>> GetLowQuantityItemCollection(int subscriptionId, SearchRequest searchRequest);
        Task<RequestResponse<ItemByAttributeSearchDataCollection>> GetItemsByGroupCollection(int subscriptionId, SearchRequest searchRequest);
        Task<RequestResponse<ItemByAttributeSearchDataCollection>> GetItemsByTagCollection(int subscriptionId, SearchRequest searchRequest);
    }
}
