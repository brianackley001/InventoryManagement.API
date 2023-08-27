using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace InventoryManagement.API.Models
{
    [ExcludeFromCodeCoverage]
    public class Item
    {
        public int Id { get; set; }
        public int SubscriptionId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int AmountValue { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public bool IsActive { get; set; }
        public List<Tag> Tags { get; set; }
        public List<Group> Groups { get; set; }
        public List<ShoppingList> ShoppingLists { get; set; }
    }
}
