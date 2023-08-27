using System.ComponentModel;

namespace InventoryManagement.API
{
    public static class Enums
    {
        public enum SortColumn
        {
            [Description("Quantity")]
            Quantity = 0,
            [Description("Name")]
            Name = 1,
            [Description("Date")]
            Date = 2
        }
    }
}
