using InventoryManagement.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace InventoryManagement.API.DataProvider.SQL
{
    public interface IShoppingListRepository
    {
        Task<RequestResponse<List<ShoppingList>>> GetShoppingListCollection(int subscriptionId, int pageNumber, int pageSize);
        Task<RequestResponse<List<ShoppingList>>> GetShoppingListCollectionWithItemCounts(int subscriptionId, SearchRequest searchRequest);
        Task<RequestResponse<bool>> UpsertShoppingListItems(int shoppingListId,  List<ShoppingListTableValueParameter> itemTvp);
        Task<RequestResponse<ShoppingList>> UpsertShoppingList(ShoppingList shoppingList);
        Task<RequestResponse<bool>> ShoppingListCheckout(List<ShoppingListTableValueParameter> itemTvp);
        Task<RequestResponse<ShoppingListItemCollection>> GetShoppingListItems(int subscriptionId, int shoppingListId, SearchRequest searchRequest);
        Task<RequestResponse<List<ShoppingListTableValueParameter>>> InitShoppingListCheckout(List<ShoppingListTableValueParameter> itemTvp);
        Task<RequestResponse<List<ShoppingListTableValueParameter>>> UpdateShoppingListItemCheckoutStatus(ShoppingListItem item);
        Task<RequestResponse<List<ShoppingListTableValueParameter>>> SyncShoppingListItems(int shoppingListId);
    }
}
