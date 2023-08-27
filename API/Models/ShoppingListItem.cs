using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace InventoryManagement.API.Models
{
    [ExcludeFromCodeCoverage]
    public class ShoppingListItem
    {
        public int Id { get; set; }
        public int SubscriptionId { get; set; }
        public int ShoppingListId { get; set; }
        public System.DateTime ShoppingListCreateDate { get; set; }
        public System.DateTime ShoppingListUpdateDate { get; set; }
        public bool ShoppingListIsActive { get; set; }
        public int ItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int AmountValue { get; set; }
        public System.DateTime ItemCreateDate { get; set; }
        public System.DateTime ItemUpdateDate { get; set; }
        public bool ItemIsActive { get; set; }
        public bool IsSelected { get; set; }
        public bool IsRetrieved { get; set; }
        public List<Tag> Tags { get; set; }
        public List<Group> Groups { get; set; }
    }
}
