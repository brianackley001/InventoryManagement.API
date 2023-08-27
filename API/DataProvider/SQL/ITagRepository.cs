using InventoryManagement.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace InventoryManagement.API.DataProvider.SQL
{
    public interface ITagRepository
    {
        Task<RequestResponse<List<Tag>>> GetTags(int subscriptionId);
        Task<RequestResponse<List<Tag>>> GetTagCollection(int subscriptionId, int pageNumber, int pageSize);
        Task<RequestResponse<List<Tag>>> GetTagCollectionWithItemCounts(int subscriptionId, SearchRequest searchRequest);
        Task<int> UpsertTag(Tag tag);
    }
}
