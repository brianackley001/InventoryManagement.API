using System.Diagnostics.CodeAnalysis;


namespace InventoryManagement.API.Models
{
    [ExcludeFromCodeCoverage]
    public class ItemShoppingList
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int ShoppingListId { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public bool IsActive { get; set; }
        public string ShoppingListName { get; set; }
        public bool IsSelected { get; set; }
    }
}
