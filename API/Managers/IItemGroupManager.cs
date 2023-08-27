using InventoryManagement.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryManagement.API.Managers
{
   public  interface IItemGroupManager
    {
        Task<RequestResponse<bool>> UpsertItemGroups(List<ItemAttributeTableValueParameter> itemTvp);
        Task<RequestResponse<List<ItemGroup>>> GetItemGroups(int itemId);
    }
}
