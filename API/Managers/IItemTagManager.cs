using InventoryManagement.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryManagement.API.Managers
{
   public  interface IItemTagManager
    {
        Task<RequestResponse<bool>> UpsertItemTags(List<ItemAttributeTableValueParameter> itemTvp);
        Task<RequestResponse<List<ItemTag>>> GetItemTags(int itemId);
    }
}
