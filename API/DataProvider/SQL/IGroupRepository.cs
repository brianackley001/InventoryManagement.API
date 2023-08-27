using InventoryManagement.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryManagement.API.DataProvider.SQL
{
    public interface IGroupRepository
    {
        Task<RequestResponse<List<Group>>> GetGroups(int subscriptionId);
        Task<RequestResponse<List<Group>>> GetGroupCollection(int subscriptionId, int pageNumber, int pageSize);
        Task<RequestResponse<List<Group>>> GetGroupCollectionWithItemCounts(int subscriptionId, SearchRequest searchRequest);
        Task<int> UpsertGroup(Group group);
    }
}
