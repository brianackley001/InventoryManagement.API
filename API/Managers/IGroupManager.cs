using InventoryManagement.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryManagement.API.Managers
{
    public interface IGroupManager
    {
        Task<RequestResponse<Group>> UpsertGroup(Group group);
        Task<RequestResponse<List<Group>>> GetGroups(int subscriptionId);
        Task<RequestResponse<List<Group>>> GetGroupCollection(int subscriptionId, int pageNumber, int pageSize);
        Task<RequestResponse<List<Group>>> GetGroupCollectionWithItemCounts(int subscriptionId, SearchRequest searchRequest);
    }
}
