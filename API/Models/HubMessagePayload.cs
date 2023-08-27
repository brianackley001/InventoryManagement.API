namespace InventoryManagement.API.Models
{
    public class HubMessagePayload
    {
        public string Message { get; set; }
        public int ShoppingListId { get; set; }
        public int SubscriptionId { get; set; }
    }
}
