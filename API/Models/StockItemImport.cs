using System.Collections.Generic;

namespace InventoryManagement.API.Models
{
    public class StockItemImport
    {
        public bool ImportAttributeValues { get; set; }
        public List<StockItem> Items { get; set; }
        public int SubscriptionId { get; set; }
    }
}
