using System.Diagnostics.CodeAnalysis;

namespace InventoryManagement.API.Models
{
    [ExcludeFromCodeCoverage]
    public class Subscription
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public int ProfileSubscriptionId { get; set; }
        public string Name { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsSelectedSubscription { get; set; }
    }
}
