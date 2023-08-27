using InventoryManagement.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryManagement.API.Managers
{
    public interface ISearchManager
    {
        Task<RequestResponse<List<SearchResult>>> GetSearchResultCollection(int subscriptionId, SearchRequest searchRequest);
        Task<RequestResponse<List<SearchResult>>> GetLowQuantityItemCollection(int subscriptionId, SearchRequest searchRequest);
        Task<RequestResponse<List<SearchResult>>> GetItemsByGroupCollection(int subscriptionId, SearchRequest searchRequest);
        Task<RequestResponse<List<SearchResult>>> GetItemsByTagCollection(int subscriptionId, SearchRequest searchRequest);
    }
}
