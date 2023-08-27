using System.Diagnostics.CodeAnalysis;


namespace InventoryManagement.API.Models
{
    [ExcludeFromCodeCoverage]
    public class ItemTag
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int TagId { get; set; }
        public System.DateTime CreateDate { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public bool IsActive { get; set; }
        public string TagName { get; set; }
    }
}
