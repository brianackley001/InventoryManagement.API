using System;
using System.Collections.Generic;

namespace InventoryManagement.API.Models
{
    public class StockItem
    {
        public int TreeNodeId { get; set; }
        public int ItemId { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public string ItemName { get; set; }
        public string NodeType { get; set; }
        public List<StockItem> Children { get; set; }
    }
}
