using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace InventoryManagement.API.Models
{
    [ExcludeFromCodeCoverage]
    public class ProfileSync
    {
        public UserProfile UserProfile { get; set; }
        public List<Tag> Tags { get; set; }
        public UserPreferences UserPreferences { get; set; }
        public List<Group> Groups { get; set; }
        public List<ShoppingList> ShoppingLists { get; set; }
    }
}
