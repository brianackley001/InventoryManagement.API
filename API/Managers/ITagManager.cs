using InventoryManagement.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryManagement.API.Managers
{
    public interface ITagManager
    {
        Task<RequestResponse<Tag>> UpsertTag(Tag tag);
        Task<RequestResponse<List<Tag>>> GetTags(int subscriptionId);
        Task<RequestResponse<List<Tag>>> GetTagCollectionWithItemCounts(int subscriptionId, SearchRequest searchRequest);
        Task<RequestResponse<List<Tag>>> GetTagCollection(int subscriptionId, int pageNumber, int pageSize);
    }
}
