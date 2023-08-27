using System.Diagnostics.CodeAnalysis;
namespace InventoryManagement.API.Models
{
    [ExcludeFromCodeCoverage]


    public class Tag
    {
        public int Id { get; set; }
        public int SubscriptionId { get; set; }
        public string Name { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public bool IsActive { get; set; }
        public int AttributeCount { get; set; }
        public bool IsSelected { get; set; }
        public int ItemCount { get; set; }
    }
}
