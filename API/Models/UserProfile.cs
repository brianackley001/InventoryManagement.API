using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace InventoryManagement.API.Models
{
    [ExcludeFromCodeCoverage]
    public class UserProfile
    {
        public int Id { get; set; }
        public string AuthId { get; set; }
        public string Source { get; set; }
        public string Name { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public bool IsActive { get; set; }
        public List<Subscription> Subscriptions { get; set; }
    }
}
 