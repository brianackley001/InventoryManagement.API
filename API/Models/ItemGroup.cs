using System.Diagnostics.CodeAnalysis;


namespace InventoryManagement.API.Models
{
    [ExcludeFromCodeCoverage]
    public class ItemGroup
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int GroupId { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public bool IsActive { get; set; }
        public string GroupName { get; set; }
    }
}
