namespace InventoryManagement.API.Models
{
    public class ShoppingListTableValueParameter
    {
        public int ShoppingListId { get; set; }
        public int ItemId { get; set; }
        public int SubscriptionId {get; set;}
        public bool IsActive { get; set; }
        public bool IsSelected { get; set; }
}
}
