using InventoryManagement.API.Models;
using System.Threading.Tasks;

namespace InventoryManagement.API.DataProvider.SQL
{
    public interface IProfileSubscriptionRepository
    {
        Task<RequestResponse<UserProfile>> GetProfileSubscriptions(string authId);
        Task<RequestResponse<UserProfile>> UpsertProfile(UserProfile userProfile);
        Task<RequestResponse<Subscription>> UpsertProfileSubscription(Subscription subscription);
        Task<RequestResponse<Subscription>> UpsertSubscription(Subscription subscription);
    }
}
