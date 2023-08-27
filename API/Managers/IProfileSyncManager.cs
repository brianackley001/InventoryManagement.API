using InventoryManagement.API.Models;
using System.Threading.Tasks;

namespace InventoryManagement.API.Managers
{
    public interface IProfileSyncManager
    {
        Task<RequestResponse<ProfileSync>> GetProfileSync(UserProfile userProfile);
    }
}
