namespace InventoryManagement.API.Models
{
    public class ItemAttributeTableValueParameter
    {
        public int AttributeId { get; set; }
        public int ItemId { get; set; }
        public bool IsActive { get; set; }
        public bool IsSelected { get; set; }
    }
}
