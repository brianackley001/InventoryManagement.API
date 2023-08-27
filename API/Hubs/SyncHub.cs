using InventoryManagement.API.Models;
//using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace InventoryManagement.API.Hubs
{
    //[Authorize]
    public class SyncHub : Hub
    {
        public async Task SyncUserSession(HubMessagePayload payload)
        {
            System.Console.WriteLine(payload.Message);
            await Clients.Others.SendAsync("SyncUserSession", payload.SubscriptionId);
        }
        public async Task SyncShoppingListCheckout(HubMessagePayload payload)
        {
            System.Console.WriteLine(payload.Message);
            await Clients.Others.SendAsync("SyncShoppingListCheckout", payload.ShoppingListId);
        }
        public async Task SyncShoppingListCheckoutCompleted(HubMessagePayload payload)
        {
            System.Console.WriteLine(payload.Message);
            await Clients.Others.SendAsync("SyncShoppingListCheckoutCompleted", payload);
        }
    }
}
