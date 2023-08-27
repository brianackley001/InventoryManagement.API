using InventoryManagement.API.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace InventoryManagement.API.UnitTest.Managers.TestSetup
{
    [ExcludeFromCodeCoverage]
    public static class ShoppingListManagerSetup
    {
        public static List<ShoppingList> GetShoppingLists()
        {
            return new List<ShoppingList>
            {
                new ShoppingList{ Id=100, IsActive=true, Name="List one", SubscriptionId=1, IsSelected=false},
                new ShoppingList{ Id=200, IsActive=true, Name="List 2", SubscriptionId=1, IsSelected=false},
                new ShoppingList{ Id=300, IsActive=true, Name="List 3", SubscriptionId=1, IsSelected=false},
                new ShoppingList{ Id=400, IsActive=true, Name="List 4", SubscriptionId=1, IsSelected=false},
                new ShoppingList{ Id=500, IsActive=true, Name="List 5", SubscriptionId=1, IsSelected=false},
                new ShoppingList{ Id=600, IsActive=true, Name="List 6", SubscriptionId=1, IsSelected=false},
                new ShoppingList{ Id=700, IsActive=true, Name="List 7", SubscriptionId=1, IsSelected=false},
                new ShoppingList{ Id=800, IsActive=true, Name="List 8", SubscriptionId=1, IsSelected=false},
                new ShoppingList{ Id=900, IsActive=true, Name="List 9", SubscriptionId=1, IsSelected=false}
            };
        }

        public static List<ShoppingListItem> GetShoppingListItems()
        {
            return new List<ShoppingListItem>
            {
                new ShoppingListItem{ SubscriptionId=1, AmountValue=2, Id=1001, Description="test a", ItemIsActive=true, ItemId=1001, Name="test name a", ShoppingListId=100, ShoppingListIsActive=true},
                new ShoppingListItem{ SubscriptionId=1, AmountValue=0, Id=1002, Description="test b", ItemIsActive=true, ItemId=1001, Name="test name 2", ShoppingListId=100, ShoppingListIsActive=true},
                new ShoppingListItem{ SubscriptionId=1, AmountValue=1, Id=1003, Description="test c", ItemIsActive=true, ItemId=1001, Name="test name 4", ShoppingListId=100, ShoppingListIsActive=true},
                new ShoppingListItem{ SubscriptionId=1, AmountValue=0, Id=1004, Description="test d", ItemIsActive=true, ItemId=1001, Name="test name df", ShoppingListId=100, ShoppingListIsActive=true},
                new ShoppingListItem{ SubscriptionId=1, AmountValue=1, Id=1005, Description="test aaaaa", ItemIsActive=true, ItemId=1001, Name="test name 788", ShoppingListId=100, ShoppingListIsActive=true},
                new ShoppingListItem{ SubscriptionId=1, AmountValue=2, Id=1006, Description="test dfdsf", ItemIsActive=true, ItemId=1001, Name="test name 999", ShoppingListId=100, ShoppingListIsActive=true}
            };
        }

        public static List<ShoppingListTableValueParameter> GetItemTvp()
        {
            return new List<ShoppingListTableValueParameter>
            {
                new ShoppingListTableValueParameter{ IsActive=true, ItemId=1001, ShoppingListId=100, SubscriptionId=1},
                new ShoppingListTableValueParameter{ IsActive=true, ItemId=1002, ShoppingListId=100, SubscriptionId=1},
                new ShoppingListTableValueParameter{ IsActive=true, ItemId=1003, ShoppingListId=100, SubscriptionId=1}
            };
        }
    }
}
