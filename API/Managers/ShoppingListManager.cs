using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryManagement.API.Models;
using InventoryManagement.API.DataProvider.SQL;
using System.Linq;
using NLog;

namespace InventoryManagement.API.Managers
{
    public class ShoppingListManager : IShoppingListManager
    {
        private readonly IShoppingListRepository _shoppingListRepository;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();


        public ShoppingListManager(IShoppingListRepository shoppingListRepository)
        {
            _shoppingListRepository = shoppingListRepository;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "<Pending>")]
        public async Task<RequestResponse<List<ShoppingList>>> GetShoppingListCollection(int subscriptionId, int pageNumber, int pageSize)
        {
            var requestResponse = new RequestResponse<List<ShoppingList>>();

            try
            {
                requestResponse = await _shoppingListRepository.GetShoppingListCollection(subscriptionId,pageNumber, pageSize).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                requestResponse.Success = false;
            }
            return requestResponse;
        }

        public async Task<RequestResponse<List<ShoppingListItem>>> GetShoppingListItems(int subscriptionId, int shoppingListId, SearchRequest searchRequest)
        {
            var requestResponse = new RequestResponse<List<ShoppingListItem>>();
            var shoppingListItems = new List<ShoppingListItem>();
            var matchedTags = new List<Tag>();
            var matchedGroups = new List<Group>();

            try
            {
                var dataCollection = await _shoppingListRepository.GetShoppingListItems(subscriptionId, shoppingListId, searchRequest).ConfigureAwait(true);
                requestResponse.Success = true;

                //  Populate Tags/Groups collection for all items
                foreach (ShoppingListItem currentItem in dataCollection.Item.ShoppingListItems)
                {
                    var tagQuery = from itemTag in dataCollection.Item.ItemTags
                                   where itemTag.ItemId == currentItem.ItemId
                                   select new Tag { Id = itemTag.TagId, Name = itemTag.TagName, IsActive = true, SubscriptionId = subscriptionId };

                    foreach (var matchedTagResult in tagQuery)
                    {
                        if (!matchedTags.Any(mt => mt.Id == matchedTagResult.Id))
                        {
                            matchedTags.Add(matchedTagResult);
                        }
                    }
                    currentItem.Tags = new List<Tag>(matchedTags);

                    var groupQuery = from itemGroup in dataCollection.Item.ItemGroups
                                     where itemGroup.ItemId == currentItem.ItemId
                                     select new Group { Id = itemGroup.GroupId, Name = itemGroup.GroupName, IsActive = true, SubscriptionId = subscriptionId };

                    foreach (var matchedGroupResult in groupQuery)
                    {
                        if (!matchedGroups.Any(mt => mt.Id == matchedGroupResult.Id))
                        {
                            matchedGroups.Add(matchedGroupResult);
                        }
                    }
                    currentItem.Groups = new List<Group>(matchedGroups);

                    shoppingListItems.Add(currentItem);
                    matchedTags.Clear();
                    matchedGroups.Clear();
                }
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                requestResponse.Success = false;
            }
            requestResponse.Item = shoppingListItems;
            return requestResponse;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "<Pending>")]
        public async Task<RequestResponse<bool>> ShoppingListCheckout(List<ShoppingListTableValueParameter> itemTvp)
        {
            var requestResponse = new RequestResponse<bool>();

            try
            {
                requestResponse = await _shoppingListRepository.ShoppingListCheckout(itemTvp).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                requestResponse.Success = false;
            }
            return requestResponse;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "<Pending>")]
        public async Task<RequestResponse<ShoppingList>> UpsertShoppingList(ShoppingList shoppingList)
        {
            var requestResponse = new RequestResponse<ShoppingList>();

            try
            {
                requestResponse = await _shoppingListRepository.UpsertShoppingList(shoppingList).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                requestResponse.Success = false;
            }
            return requestResponse;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "<Pending>")]
        public async Task<RequestResponse<bool>> UpsertShoppingListItems(int shoppingListId, List<ShoppingListTableValueParameter> itemTvp)
        {
            var requestResponse = new RequestResponse<bool>();

            try
            {
                requestResponse = await _shoppingListRepository.UpsertShoppingListItems(shoppingListId, itemTvp).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                requestResponse.Success = false;
            }
            return requestResponse;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "<Pending>")]
        public async Task<RequestResponse<List<ShoppingListTableValueParameter>>> InitShoppingListCheckout(List<ShoppingListTableValueParameter> itemTvp)
        {
            var requestResponse = new RequestResponse<List<ShoppingListTableValueParameter>>();

            try
            {
                requestResponse = await _shoppingListRepository.InitShoppingListCheckout(itemTvp).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                requestResponse.Success = false;
            }
            return requestResponse;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "<Pending>")]
        public async Task<RequestResponse<List<ShoppingListTableValueParameter>>> UpdateShoppingListItemCheckoutStatus(ShoppingListItem item)
        {
            var requestResponse = new RequestResponse<List<ShoppingListTableValueParameter>>();

            try
            {
                requestResponse = await _shoppingListRepository.UpdateShoppingListItemCheckoutStatus(item).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                requestResponse.Success = false;
            }
            return requestResponse;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "<Pending>")]
        public async Task<RequestResponse<List<ShoppingListTableValueParameter>>> SyncShoppingListItems(int shoppingListId)
        {
            var requestResponse = new RequestResponse<List<ShoppingListTableValueParameter>>();

            try
            {
                requestResponse = await _shoppingListRepository.SyncShoppingListItems(shoppingListId).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                requestResponse.Success = false;
            }
            return requestResponse;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "<Pending>")]
        public async Task<RequestResponse<List<ShoppingList>>> GetShoppingListCollectionWithItemCounts(int subscriptionId, SearchRequest searchRequest)
        {
            var requestResponse = new RequestResponse<List<ShoppingList>>();

            try
            {
                requestResponse = await _shoppingListRepository.GetShoppingListCollectionWithItemCounts(subscriptionId, searchRequest).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                requestResponse.Success = false;
            }
            return requestResponse;
        }
    }
}
