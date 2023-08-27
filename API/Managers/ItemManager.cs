using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryManagement.API.Models;
using InventoryManagement.API.DataProvider.SQL;
using NLog;
using System.Linq;

namespace InventoryManagement.API.Managers
{
    public class ItemManager : IItemManager
    {
        private readonly IItemRepository _itemRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IItemTagRepository _itemTagRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IItemGroupRepository _itemGroupRepository;
        private readonly IShoppingListRepository _shoppingListRepository;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();


        public ItemManager(IItemRepository itemRepository, ITagRepository tagRepository, IGroupRepository groupRepository, 
            IShoppingListRepository shoppingListRepository, IItemGroupRepository itemGroupRepository, IItemTagRepository itemTagRepository)
        {
            _itemRepository = itemRepository;
            _tagRepository = tagRepository;
            _groupRepository = groupRepository;
            _shoppingListRepository = shoppingListRepository;
            _itemGroupRepository = itemGroupRepository;
            _itemTagRepository = itemTagRepository;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "<Pending>")]
        public async Task<RequestResponse<Item>> GetItem(int itemId)
        {
            var item = new RequestResponse<Item>();
            var groups = new RequestResponse<List<Group>>();
            var tags = new RequestResponse<List<Tag>>();
            var shoppingLists = new RequestResponse<List<ShoppingList>>();

            try
            {
                item = await _itemRepository.GetItem(itemId).ConfigureAwait(true);

                // retrieve full list of Tag/Group/ShoppingList attribute collections, merge with item's current selected items
                var tagTask = _tagRepository.GetTags(item.Item.SubscriptionId);
                var groupTask = _groupRepository.GetGroups(item.Item.SubscriptionId);
                var shoppingListTask = _shoppingListRepository.GetShoppingListCollection(item.Item.SubscriptionId, 1, 1000);
                await Task.WhenAll(tagTask, groupTask, shoppingListTask);

                foreach (Tag t in tagTask.Result.Item)
                {
                    foreach(Tag selectedTag in item.Item.Tags)
                    {
                        if(t.Id == selectedTag.Id)
                        {
                            t.IsSelected = true;
                        }
                    }
                }
                foreach (Group g in groupTask.Result.Item)
                {
                    foreach (Group selectedGroup in item.Item.Groups)
                    {
                        if (g.Id == selectedGroup.Id)
                        {
                            g.IsSelected = true;
                        }
                    }
                }
                foreach (ShoppingList s in shoppingListTask.Result.Item)
                {
                    foreach (ShoppingList selectedShoppingList in item.Item.ShoppingLists)
                    {
                        if (s.Id == selectedShoppingList.Id)
                        {
                            s.IsSelected = true;
                        }
                    }
                }

                item.Item.Tags = tagTask.Result.Item;
                item.Item.Groups = groupTask.Result.Item;
                item.Item.ShoppingLists = shoppingListTask.Result.Item;
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                item.Success = false;
            }
            return item;
        }

        public async Task<RequestResponse<List<Item>>> GetItemCollection(int subscriptionId, SearchRequest searchRequest)
        {
            var itemCollection = new RequestResponse<List<Item>>();
            itemCollection.Item = new List<Item>();
            var matchedTags = new List<Tag>();
            var matchedGroups = new List<Group>();

            try
            {
                var dataItemCollection = await _itemRepository.GetItemCollection(subscriptionId, searchRequest).ConfigureAwait(true);

                itemCollection.Success = dataItemCollection.Success;
                itemCollection.PagedCollection = dataItemCollection.PagedCollection;

                //  Populate Tags/Groups collection for all items
                foreach (Item currentItem in dataItemCollection.Item.Items)
                {
                    var tagQuery = from itemTag in dataItemCollection.Item.ItemTags
                                   where itemTag.ItemId == currentItem.Id
                                   select new Tag { Id = itemTag.TagId, Name = itemTag.TagName, IsActive = true, SubscriptionId = subscriptionId };

                    foreach (var matchedTagResult in tagQuery)
                    {
                        if (!matchedTags.Any(mt => mt.Id == matchedTagResult.Id))
                        {
                            matchedTags.Add(matchedTagResult);
                        }
                    }
                    if (matchedTags.Count > 0)
                    {
                        currentItem.Tags = new List<Tag>(matchedTags);
                    }

                    var groupQuery = from itemGroup in dataItemCollection.Item.ItemGroups
                                     where itemGroup.ItemId == currentItem.Id
                                     select new Group { Id = itemGroup.GroupId, Name = itemGroup.GroupName, IsActive = true, SubscriptionId = subscriptionId };

                    foreach (var matchedGroupResult in groupQuery)
                    {
                        if (!matchedGroups.Any(mt => mt.Id == matchedGroupResult.Id))
                        {
                            matchedGroups.Add(matchedGroupResult);
                        }
                    }
                    if (matchedGroups.Count > 0)
                    {
                        currentItem.Groups = new List<Group>(matchedGroups);
                    }

                    matchedTags.Clear();
                    matchedGroups.Clear();
                    itemCollection.Item.Add(currentItem);
                }
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                itemCollection.Success = false;
            }

            return itemCollection;
        }

        public async Task<RequestResponse<List<Item>>> GetLowQuantityItemCollection(int subscriptionId, SearchRequest searchRequest)
        {
            var itemCollection = new RequestResponse<List<Item>>();
            itemCollection.Item = new List<Item>();
            var matchedTags = new List<Tag>();
            var matchedGroups = new List<Group>();


            try
            {
                var dataItemCollection = await _itemRepository.GetLowQuantityItemCollection(subscriptionId, searchRequest).ConfigureAwait(true);

                itemCollection.Success = dataItemCollection.Success;
                itemCollection.PagedCollection = dataItemCollection.PagedCollection;

                //  Populate Tags/Groups collection for all items
                foreach (Item currentItem in dataItemCollection.Item.Items)
                {
                    var tagQuery = from itemTag in dataItemCollection.Item.ItemTags
                                   where itemTag.ItemId == currentItem.Id
                                   select new Tag { Id = itemTag.TagId, Name = itemTag.TagName, IsActive = true, SubscriptionId = subscriptionId };

                    foreach (var matchedTagResult in tagQuery)
                    {
                        if (!matchedTags.Any(mt => mt.Id == matchedTagResult.Id))
                        {
                            matchedTags.Add(matchedTagResult);
                        }
                    }
                    if (matchedTags.Count > 0)
                    {
                        currentItem.Tags = new List<Tag>(matchedTags);
                    }

                    var groupQuery = from itemGroup in dataItemCollection.Item.ItemGroups
                                     where itemGroup.ItemId == currentItem.Id
                                     select new Group { Id = itemGroup.GroupId, Name = itemGroup.GroupName, IsActive = true, SubscriptionId = subscriptionId };

                    foreach (var matchedGroupResult in groupQuery)
                    {
                        if (!matchedGroups.Any(mt => mt.Id == matchedGroupResult.Id))
                        {
                            matchedGroups.Add(matchedGroupResult);
                        }
                    }
                    if (matchedGroups.Count > 0)
                    {
                        currentItem.Groups = new List<Group>(matchedGroups);
                    }

                    matchedTags.Clear();
                    matchedGroups.Clear();
                    itemCollection.Item.Add(currentItem);
                }
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                itemCollection.Success = false;
            }
            return itemCollection;
        }

        public async Task<RequestResponse<List<Item>>> GetItemsByIdCollection(int subscriptionId, SearchRequest searchRequest)
        {
            var itemCollection = new RequestResponse<List<Item>>();
            itemCollection.Item = new List<Item>();
            var matchedTags = new List<Tag>();
            var matchedGroups = new List<Group>();


            try
            {
                var dataItemCollection = await _itemRepository.GetItemsByIdCollection(subscriptionId, searchRequest).ConfigureAwait(true);

                itemCollection.Success = dataItemCollection.Success;
                itemCollection.PagedCollection = dataItemCollection.PagedCollection;

                //  Populate Tags/Groups collection for all items
                foreach (Item currentItem in dataItemCollection.Item.Items)
                {
                    var tagQuery = from itemTag in dataItemCollection.Item.ItemTags
                                   where itemTag.ItemId == currentItem.Id
                                   select new Tag { Id = itemTag.TagId, Name = itemTag.TagName, IsActive = true, SubscriptionId = subscriptionId };

                    foreach (var matchedTagResult in tagQuery)
                    {
                        if (!matchedTags.Any(mt => mt.Id == matchedTagResult.Id))
                        {
                            matchedTags.Add(matchedTagResult);
                        }
                    }
                    if (matchedTags.Count > 0)
                    {
                        currentItem.Tags = new List<Tag>(matchedTags);
                    }

                    var groupQuery = from itemGroup in dataItemCollection.Item.ItemGroups
                                     where itemGroup.ItemId == currentItem.Id
                                     select new Group { Id = itemGroup.GroupId, Name = itemGroup.GroupName, IsActive = true, SubscriptionId = subscriptionId };

                    foreach (var matchedGroupResult in groupQuery)
                    {
                        if (!matchedGroups.Any(mt => mt.Id == matchedGroupResult.Id))
                        {
                            matchedGroups.Add(matchedGroupResult);
                        }
                    }
                    if (matchedGroups.Count > 0)
                    {
                        currentItem.Groups = new List<Group>(matchedGroups);
                    }

                    matchedTags.Clear();
                    matchedGroups.Clear();
                    itemCollection.Item.Add(currentItem);
                }
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                itemCollection.Success = false;
            }
            return itemCollection;
        }

#pragma warning disable S927 // parameter names should match base declaration and other partial definitions
        public async Task<RequestResponse<Item>> UpsertItem(Item item)
#pragma warning restore S927 // parameter names should match base declaration and other partial definitions
        {
            var insertItemId = -1;
            var collectionSuccess = true;

            try
            {
                insertItemId = await _itemRepository.UpsertItem(item).ConfigureAwait(true);

                // handle Groups/Tags
                var itemGroups = new List<ItemAttributeTableValueParameter>();
                var itemTags = new List<ItemAttributeTableValueParameter>();
                var itemShoppingLists = new List<ShoppingListTableValueParameter>();

                foreach (Group g in item.Groups)
                {
                    itemGroups.Add(new ItemAttributeTableValueParameter
                    {
                        IsActive = g.IsActive,
                        IsSelected = g.IsSelected,
                        AttributeId = g.Id,
                        ItemId = item.Id > 0 ? item.Id : insertItemId
                    });
                }

                var gResult = new RequestResponse<bool> { Success = true };
                if (item.Groups.Count > 0)
                {
                    gResult = await _itemGroupRepository.UpsertItemGroups(itemGroups);
                }
                collectionSuccess &= gResult.Success;

                foreach (Tag t in item.Tags)
                {
                    itemTags.Add(new ItemAttributeTableValueParameter
                    {
                        IsActive = t.IsActive,
                        IsSelected = t.IsSelected,
                        AttributeId =  t.Id,
                        ItemId = item.Id > 0 ? item.Id : insertItemId
                    });
                }

                var tResult = new RequestResponse<bool> { Success = true };
                if (item.Tags.Count > 0)
                {
                    tResult = await _itemTagRepository.UpsertItemTags(itemTags);
                }
                collectionSuccess &= tResult.Success;

                foreach (ShoppingList sl in item.ShoppingLists)
                {
                    itemShoppingLists.Add(new ShoppingListTableValueParameter
                    {
                        IsActive = sl.IsActive,
                        IsSelected = sl.IsSelected,
                        SubscriptionId = item.SubscriptionId,
                        ItemId = item.Id > 0 ? item.Id : insertItemId,
                        ShoppingListId = sl.Id
                    });
                }

                var slResult = new RequestResponse<bool> { Success = true };
                if (item.ShoppingLists.Count > 0)
                {
                    slResult = await _shoppingListRepository.UpsertShoppingListItems(item.ShoppingLists[0].Id, itemShoppingLists);
                }

                collectionSuccess &= slResult.Success;
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
            }

            if (insertItemId > -1 && item.Id < 1)
            {
                item.Id = insertItemId;
                item.CreateDate = DateTime.Now;
            }
            item.UpdateDate = DateTime.Now;
            return new RequestResponse<Item> { Item = item, Success = insertItemId > -1 && collectionSuccess };
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "<Pending>")]
        public async Task<RequestResponse<List<StockItem>>> GetStockItems()
        {
            var stockItemCollection = new RequestResponse<List<StockItem>>();

            try
            {
                stockItemCollection = await _itemRepository.GetStockItems().ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                stockItemCollection.Success = false;
            }
            return stockItemCollection;
        }

        public async Task<RequestResponse<bool>> UpsertStockItems(StockItemImport stockItemImport)
        {
            var success = true;

            try
            {
                await _itemRepository.UpsertStockItems(stockItemImport).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                success = false;
            }
            return new RequestResponse<bool> { Item = success, Success = success };
        }
    }
}
