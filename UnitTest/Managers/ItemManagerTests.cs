using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryManagement.API.DataProvider.SQL;
using Moq;
using System.Diagnostics.CodeAnalysis;
using InventoryManagement.API.Models;
using NLog;
using InventoryManagement.API.UnitTest.Managers.TestSetup;

namespace InventoryManagement.API.Managers.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture()]
    [Category("ManagerTests")]
    public class ItemManagerTests
    {
        private Mock<IItemRepository> _mockItemRepository;
        private Mock<ITagRepository> _mockTagRepository;
        private Mock<IGroupRepository> _mockGroupRepository;
        private Mock<IShoppingListRepository> _mockShoppingListRepository;
        private Mock<IItemGroupRepository> _mockItemGroupRepository;
        private Mock<IItemTagRepository> _mockItemTagRepository;
        private Mock<ILogger> _mockLogger;
        private RequestResponse<ItemCollectionDataCollection> _allItems;
        private RequestResponse<ItemCollectionDataCollection> _itemCollection;
        private RequestResponse<Item> _upsertItem;
        private NullReferenceException _expectedException;
        private List<ShoppingList> _shoppingListCollection;
        private RequestResponse<List<Tag>> _allTags;
        private RequestResponse<List<Group>> _allGroups;
        private RequestResponse<List<ShoppingList>> _shoppingListCollectionResponse;
        private SearchRequest _searchRequest;


        [ExcludeFromCodeCoverage, OneTimeSetUp]
        public void TestSetup()
        {
            _expectedException = new NullReferenceException("Test Went Boom!");
            _mockItemRepository = new Mock<IItemRepository>();
            _mockShoppingListRepository = new Mock<IShoppingListRepository>();
            _mockGroupRepository = new Mock<IGroupRepository>();
            _mockTagRepository = new Mock<ITagRepository>();
            _mockItemGroupRepository = new Mock<IItemGroupRepository>();
            _mockItemTagRepository = new Mock<IItemTagRepository>();



            var testTags = new List<Tag>
            {
                new Tag { AttributeCount=1,CreateDate = DateTime.Now, Id = 123, IsActive=true, Name="One", SubscriptionId=333, UpdateDate=DateTime.Now, IsSelected=false },
                new Tag { AttributeCount=1,CreateDate = DateTime.Now, Id = 333, IsActive=true, Name="Two", SubscriptionId=333, UpdateDate=DateTime.Now , IsSelected=false}
            };
            _allTags = new RequestResponse<List<Tag>>();
            _allTags.Item = testTags;
            _allTags.Success = true;

            var testGroups = new List<Group>
            {
                new Group { AttributeCount=1,CreateDate = DateTime.Now, Id = 123, IsActive=true, Name="One", SubscriptionId=333, UpdateDate=DateTime.Now, IsSelected=false  },
                new Group { AttributeCount=1,CreateDate = DateTime.Now, Id = 333, IsActive=true, Name="Two", SubscriptionId=333, UpdateDate=DateTime.Now, IsSelected=false  }
            };
            _allGroups = new RequestResponse<List<Group>>();
            _allGroups.Item = testGroups;
            _allGroups.Success = true;

            _shoppingListCollection = ShoppingListManagerSetup.GetShoppingLists();
            _shoppingListCollectionResponse = new RequestResponse<List<ShoppingList>>
            {
                Item = _shoppingListCollection,
                PagedCollection = new PagedCollection { CollectionTotal = 10, PageNumber = 1, PageSize = 10 },
                Success = true
            };

            var testItems = new List<Item>
            {
                new Item { CreateDate = DateTime.Now, Id = 123, IsActive=true, Name="One", SubscriptionId=333, UpdateDate=DateTime.Now, Description="testing 1234", AmountValue=2, Groups = _allGroups.Item, ShoppingLists = _shoppingListCollection, Tags = _allTags.Item},
                new Item { CreateDate = DateTime.Now, Id = 102, IsActive=true, Name="Two", SubscriptionId=333, UpdateDate=DateTime.Now, Description="testing YYYYYY", AmountValue=2, Groups = _allGroups.Item, ShoppingLists = _shoppingListCollection, Tags = _allTags.Item }
            };

            var itemGroupCollection = new List<ItemGroup> {
                new ItemGroup{Id=9987, IsActive=true, GroupId=900, ItemId=100,CreateDate=DateTime.Now.AddDays(-12), UpdateDate=DateTime.Now.AddDays(-12), GroupName="First Group"},
                new ItemGroup{Id=22222, IsActive=true, GroupId=901, ItemId=100,CreateDate=DateTime.Now.AddDays(-12), UpdateDate=DateTime.Now.AddDays(-12), GroupName="Second Group"},
                new ItemGroup{Id=23423, IsActive=true, GroupId=902, ItemId=102,CreateDate=DateTime.Now.AddDays(-12), UpdateDate=DateTime.Now.AddDays(-12), GroupName="Third  Group"}
            };

            var itemTagCollection = new List<ItemTag>
            {
                new ItemTag{Id=19987, IsActive=true, TagId=110, ItemId=100,CreateDate=DateTime.Now.AddDays(-12), UpdateDate=DateTime.Now.AddDays(-12), TagName="Tag #1"},
                new ItemTag{Id=18887, IsActive=true, TagId=130, ItemId=101,CreateDate=DateTime.Now.AddDays(-22), UpdateDate=DateTime.Now.AddDays(-23), TagName="Tag #2"},
                new ItemTag{Id=112987, IsActive=true, TagId=140, ItemId=101,CreateDate=DateTime.Now.AddDays(-23), UpdateDate=DateTime.Now.AddDays(-26), TagName="Tag #3"}

            };

            _allItems = new RequestResponse<ItemCollectionDataCollection>();
            _allItems.Item = new ItemCollectionDataCollection { ItemGroups = itemGroupCollection, ItemTags = itemTagCollection, Items = testItems };
            _allItems.Success = true;
            _itemCollection = new RequestResponse<ItemCollectionDataCollection>();
            _itemCollection.Success = true;
            _itemCollection.PagedCollection = new PagedCollection { CollectionTotal = 55, PageNumber = 1, PageSize = 5 };
            _itemCollection.Item = new ItemCollectionDataCollection { ItemGroups = itemGroupCollection, ItemTags = itemTagCollection, Items = testItems };
            _upsertItem = new RequestResponse<Item>();
            _upsertItem.Success = true;
            _upsertItem.Item = testItems[0];
            _mockLogger = new Mock<ILogger>();
            _searchRequest = new SearchRequest { PagedCollection = new PagedCollection { CollectionTotal = 20, PageSize = 10, PageNumber = 1 }, SortAscending = true, SortBy = "name", IdCollection = new List<int>() };
        }

        [Test()]
        public async Task GetItemCollectionSuccessTest()
        {
            // Arrange
            _mockItemRepository.Reset();
            _mockItemRepository.Setup(i => i.GetItemCollection(It.IsAny<int>(), It.IsAny<SearchRequest>())).Returns(Task.FromResult(_allItems));
            var sut = new ItemManager(_mockItemRepository.Object, _mockTagRepository.Object, _mockGroupRepository.Object, _mockShoppingListRepository.Object, _mockItemGroupRepository.Object, _mockItemTagRepository.Object);

            // Act
            var response = await sut.GetItemCollection(123, _searchRequest);

            // Assert
            _mockItemRepository.Verify(r => r.GetItemCollection(123, _searchRequest), Times.Once, "Expected method to be called once");
            Assert.IsTrue(response.Success, "Expected response.Success to be true");
        }
        [Test()]
        public async Task GetItemCollectionFailTest()
        {
            // Arrange
            _mockItemRepository.Reset();
            _mockItemRepository.Setup(i => i.GetItemCollection(It.IsAny<int>(), It.IsAny<SearchRequest>())).Throws(_expectedException);
            var sut = new ItemManager(_mockItemRepository.Object, _mockTagRepository.Object, _mockGroupRepository.Object, _mockShoppingListRepository.Object, _mockItemGroupRepository.Object, _mockItemTagRepository.Object);

            // Act
            var response = await sut.GetItemCollection(123, _searchRequest);

            // Assert
            _mockItemRepository.Verify(r => r.GetItemCollection(123, _searchRequest), Times.Once, "Expected method to be called once");
            Assert.IsTrue(!response.Success, "Expected response.Success to be false");
        }

        [Test()]
        public async Task GetLowQuantityItemCollectionSuccessTest()
        {
            // Arrange
            _mockItemRepository.Reset();
            _mockItemRepository.Setup(r => r.GetLowQuantityItemCollection(It.IsAny<int>(), It.IsAny<SearchRequest>())).Returns(Task.FromResult(_allItems));
            var sut = new ItemManager(_mockItemRepository.Object, _mockTagRepository.Object, _mockGroupRepository.Object, _mockShoppingListRepository.Object, _mockItemGroupRepository.Object, _mockItemTagRepository.Object);

            // Act
            var response = await sut.GetLowQuantityItemCollection(123,_searchRequest);

            // Assert
            _mockItemRepository.Verify(r => r.GetLowQuantityItemCollection(123, _searchRequest), Times.Once, "Expected method to be called once");
            Assert.IsTrue(response.Success, "Expected response.Success to be true");
        }

        [Test()]
        public async Task GetItemLowQuantityCollectionFailTest()
        {
            // Arrange
            _mockItemRepository.Reset();
            _mockItemRepository.Setup(r => r.GetLowQuantityItemCollection(It.IsAny<int>(), It.IsAny<SearchRequest>())).Throws(_expectedException);
            var sut = new ItemManager(_mockItemRepository.Object, _mockTagRepository.Object, _mockGroupRepository.Object, _mockShoppingListRepository.Object, _mockItemGroupRepository.Object, _mockItemTagRepository.Object);

            // Act
            var response = await sut.GetLowQuantityItemCollection(123, _searchRequest);

            // Assert
            _mockItemRepository.Verify(r => r.GetLowQuantityItemCollection(123, _searchRequest), Times.Once, "Expected method to be called once");
            Assert.IsTrue(!response.Success, "Expected response.Success to be false");
        }

        [Test()]
        public async Task GetItemSuccessTest()
        {
            // Arrange
            _mockItemRepository.Reset();
            _mockItemRepository.Setup(i => i.GetItem(It.IsAny<int>())).Returns(Task.FromResult(_upsertItem));
            _mockTagRepository.Setup(t => t.GetTags(It.IsAny<int>())).Returns(Task.FromResult(_allTags));
            _mockGroupRepository.Setup(g => g.GetGroups(It.IsAny<int>())).Returns(Task.FromResult(_allGroups));
            _mockShoppingListRepository.Setup(s => s.GetShoppingListCollection(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(_shoppingListCollectionResponse));
            var sut = new ItemManager(_mockItemRepository.Object, _mockTagRepository.Object, _mockGroupRepository.Object, _mockShoppingListRepository.Object, _mockItemGroupRepository.Object, _mockItemTagRepository.Object);

            // Act
            var response = await sut.GetItem(123);

            // Assert
            _mockItemRepository.Verify(r => r.GetItem(123), Times.Once, "Expected method to be called once");
            Assert.IsTrue(response.Success, "Expected response.Success to be true");
        }

        [Test()]
        public async Task GetItemFailTest()
        {
            // Arrange
            _mockItemRepository.Reset();
            _mockItemRepository.Setup(i => i.GetItem(It.IsAny<int>())).Throws(_expectedException);
            _mockTagRepository.Setup(t => t.GetTags(It.IsAny<int>())).Returns(Task.FromResult(new RequestResponse<List<Tag>>()));
            _mockGroupRepository.Setup(g => g.GetGroups(It.IsAny<int>())).Returns(Task.FromResult(new RequestResponse<List<Group>>()));
            _mockShoppingListRepository.Setup(s => s.GetShoppingListCollection(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(new RequestResponse<List<ShoppingList>>()));
            var sut = new ItemManager(_mockItemRepository.Object, _mockTagRepository.Object, _mockGroupRepository.Object, _mockShoppingListRepository.Object, _mockItemGroupRepository.Object, _mockItemTagRepository.Object);

            // Act
            var response = await sut.GetItem(123);

            // Assert
            _mockItemRepository.Verify(r => r.GetItem(123), Times.Once, "Expected method to be called once");
            Assert.IsTrue(!response.Success, "Expected response.Success to be false");
        }

        [Test()]
        public async Task InsertItemSuccessTest()
        {
            // Arrange
            _mockItemRepository.Reset();
            var insertItemId = 55567;
            var updateDateValue = DateTime.Now;
            var minDateValue = DateTime.Now.AddMilliseconds(-1);
            var submitItem = new Item { CreateDate = DateTime.Now, Id = 0, IsActive = true, Name = "Unit Test", SubscriptionId = 667, UpdateDate = updateDateValue };
            _mockItemRepository.Setup(i => i.UpsertItem(It.IsAny<Item>())).Returns(Task.FromResult(insertItemId));
            var sut = new ItemManager(_mockItemRepository.Object, _mockTagRepository.Object, _mockGroupRepository.Object, _mockShoppingListRepository.Object, _mockItemGroupRepository.Object, _mockItemTagRepository.Object);

            // Act
            var response = await sut.UpsertItem(submitItem);

            // Assert
            _mockItemRepository.Verify(r => r.UpsertItem(submitItem), Times.Once, "Expected method to be called once");
            Assert.IsTrue(response.Success, "Expected response.Success to be true");
            Assert.IsTrue(response.Item.Id == insertItemId, "Expected itemID to be @@IDENTITY returned fromDB Repository to be true");
            Assert.IsTrue(response.Item.UpdateDate > minDateValue, "Expected itemUpdateDate > than test value to be true");
            Assert.IsTrue(response.Item.CreateDate > minDateValue, "Expected itemCreateDate > than test value to be true");
        }

        [Test()]
        public async Task InsertGroupFailTest()
        {
            // Arrange
            _mockItemRepository.Reset();
            var submitItem = new Item { CreateDate = DateTime.Now, Id = 0, IsActive = true, Name = "Unit Test", SubscriptionId = 667, UpdateDate = DateTime.Now };
            _mockItemRepository.Setup(i => i.UpsertItem(It.IsAny<Item>())).Throws(_expectedException);
            var sut = new ItemManager(_mockItemRepository.Object, _mockTagRepository.Object, _mockGroupRepository.Object, _mockShoppingListRepository.Object, _mockItemGroupRepository.Object, _mockItemTagRepository.Object);

            // Act
            var response = await sut.UpsertItem(submitItem);

            // Assert
            _mockItemRepository.Verify(r => r.UpsertItem(submitItem), Times.Once, "Expected method to be called once");
            Assert.IsTrue(!response.Success, "Expected response.Success to be false");
        }

        [Test()]
        public async Task UpdateItemSuccessTest()
        {
            // Arrange
            _mockItemRepository.Reset();
            _mockItemTagRepository.Reset();
            _mockItemGroupRepository.Reset();
            _mockShoppingListRepository.Reset();
            var updateGroupId = 55567;
            var updateDateValue = DateTime.Now;
            var groupResponse = new RequestResponse<bool> { Item = true, Success = true };
            var tagResponse = new RequestResponse<bool> { Item = true, Success = true };
            var shoppingListResponse = new RequestResponse<bool> { Item = true, Success = true };
            var minDateValue = DateTime.Now.AddMilliseconds(-1);
            var createDateValue = DateTime.Now.AddMonths(-1);
            var submitItem = new Item
            {
                CreateDate = createDateValue,
                Id = updateGroupId,
                IsActive = true,
                Name = "Unit Test",
                SubscriptionId = 667,
                UpdateDate = updateDateValue,
                Groups = new List<Group> { new Group { Id = 987 } },
                Tags = new List<Tag> { new Tag { Id = 987 } },
                ShoppingLists = new List<ShoppingList> { new ShoppingList { Id = 999 } }
            };
            _mockItemRepository.Setup(i => i.UpsertItem(It.IsAny<Item>())).Returns(Task.FromResult(updateGroupId));
            _mockItemTagRepository.Setup(t => t.UpsertItemTags(It.IsAny<List<ItemAttributeTableValueParameter>>())).Returns(Task.FromResult(groupResponse));
            _mockItemGroupRepository.Setup(g => g.UpsertItemGroups(It.IsAny<List<ItemAttributeTableValueParameter>>())).Returns(Task.FromResult(tagResponse));
            _mockShoppingListRepository.Setup(s => s.UpsertShoppingListItems(submitItem.ShoppingLists[0].Id, It.IsAny<List<ShoppingListTableValueParameter>>())).Returns(Task.FromResult(shoppingListResponse));
            var sut = new ItemManager(_mockItemRepository.Object, _mockTagRepository.Object, _mockGroupRepository.Object, _mockShoppingListRepository.Object, _mockItemGroupRepository.Object, _mockItemTagRepository.Object);

            // Act
            var response = await sut.UpsertItem(submitItem);

            // Assert
            _mockItemRepository.Verify(r => r.UpsertItem(submitItem), Times.Once, "Expected method to be called once");
            _mockItemTagRepository.Verify(r => r.UpsertItemTags(It.IsAny<List<ItemAttributeTableValueParameter>>()), Times.Once, "Expected method to be called once");
            _mockItemGroupRepository.Verify(r => r.UpsertItemGroups(It.IsAny<List<ItemAttributeTableValueParameter>>()), Times.Once, "Expected method to be called once");
            _mockShoppingListRepository.Verify(r => r.UpsertShoppingListItems(It.IsAny<int>(), It.IsAny<List<ShoppingListTableValueParameter>>()), Times.Once, "Expected method to be called once");
            Assert.IsTrue(response.Success, "Expected response.Success to be true");
            Assert.IsTrue(response.Item.Id == updateGroupId, "Expected itemID equal to object ID passed in to be true");
            Assert.IsTrue(response.Item.UpdateDate > minDateValue, "Expected itemUpdateDate > than test value to be true");
            Assert.IsTrue(response.Item.CreateDate == createDateValue, "Expected itemCreateDate unchanged to be true");
        }

        [Test()]
        public async Task UpdateItemDetailGroupFalse()
        {
            // Arrange
            _mockItemRepository.Reset();
            _mockItemTagRepository.Reset();
            _mockItemGroupRepository.Reset();
            _mockShoppingListRepository.Reset();
            var updateGroupId = 55567;
            var updateDateValue = DateTime.Now;
            var minDateValue = DateTime.Now.AddMilliseconds(-1);
            var createDateValue = DateTime.Now.AddMonths(-1);
            var groupResponse = new RequestResponse<bool> { Item = false, Success = false };
            var tagResponse = new RequestResponse<bool> { Item = true, Success = true };
            var shoppingListResponse = new RequestResponse<bool> { Item = true, Success = true };
            var submitItem = new Item
            {
                CreateDate = createDateValue,
                Id = updateGroupId,
                IsActive = true,
                Name = "Unit Test",
                SubscriptionId = 667,
                UpdateDate = updateDateValue,
                Groups = new List<Group> { new Group { Id = 987 } },
                Tags = new List<Tag> { new Tag { Id = 987 } },
                ShoppingLists = new List<ShoppingList> { new ShoppingList { Id = 999 } }
            };
            _mockItemRepository.Setup(i => i.UpsertItem(It.IsAny<Item>())).Returns(Task.FromResult(updateGroupId));
            _mockItemTagRepository.Setup(t => t.UpsertItemTags(It.IsAny<List<ItemAttributeTableValueParameter>>())).Returns(Task.FromResult(groupResponse));
            _mockItemGroupRepository.Setup(g => g.UpsertItemGroups(It.IsAny<List<ItemAttributeTableValueParameter>>())).Returns(Task.FromResult(tagResponse));
            _mockShoppingListRepository.Setup(s => s.UpsertShoppingListItems(submitItem.ShoppingLists[0].Id, It.IsAny<List<ShoppingListTableValueParameter>>())).Returns(Task.FromResult(shoppingListResponse));
            var sut = new ItemManager(_mockItemRepository.Object, _mockTagRepository.Object, _mockGroupRepository.Object, _mockShoppingListRepository.Object, _mockItemGroupRepository.Object, _mockItemTagRepository.Object);

            // Act
            var response = await sut.UpsertItem(submitItem);

            // Assert
            _mockItemRepository.Verify(r => r.UpsertItem(submitItem), Times.Once, "Expected method to be called once");
            _mockItemTagRepository.Verify(r => r.UpsertItemTags(It.IsAny<List<ItemAttributeTableValueParameter>>()), Times.Once, "Expected method to be called once");
            _mockItemGroupRepository.Verify(r => r.UpsertItemGroups(It.IsAny<List<ItemAttributeTableValueParameter>>()), Times.Once, "Expected method to be called once");
            _mockShoppingListRepository.Verify(r => r.UpsertShoppingListItems(submitItem.ShoppingLists[0].Id, It.IsAny<List<ShoppingListTableValueParameter>>()), Times.Once, "Expected method to be called once");
            Assert.IsFalse(response.Success, "Expected response.Success to be false");
            Assert.IsTrue(response.Item.Id == updateGroupId, "Expected itemID equal to object ID passed in to be true");
            Assert.IsTrue(response.Item.UpdateDate > minDateValue, "Expected itemUpdateDate > than test value to be true");
            Assert.IsTrue(response.Item.CreateDate == createDateValue, "Expected itemCreateDate unchanged to be true");
        }

        [Test()]
        public async Task UpdateItemDetailTagFalse()
        {
            // Arrange
            _mockItemRepository.Reset();
            _mockItemTagRepository.Reset();
            _mockItemGroupRepository.Reset();
            _mockShoppingListRepository.Reset();
            var updateGroupId = 55567;
            var updateDateValue = DateTime.Now;
            var minDateValue = DateTime.Now.AddMilliseconds(-1);
            var createDateValue = DateTime.Now.AddMonths(-1);
            var groupResponse = new RequestResponse<bool> { Item = true, Success = true };
            var tagResponse = new RequestResponse<bool> { Item = false, Success = false };
            var shoppingListResponse = new RequestResponse<bool> { Item = true, Success = true };
            var submitItem = new Item
            {
                CreateDate = createDateValue,
                Id = updateGroupId,
                IsActive = true,
                Name = "Unit Test",
                SubscriptionId = 667,
                UpdateDate = updateDateValue,
                Groups = new List<Group> { new Group { Id = 987 } },
                Tags = new List<Tag> { new Tag { Id = 987 } },
                ShoppingLists = new List<ShoppingList> { new ShoppingList { Id = 999 } }
            };
            _mockItemRepository.Setup(i => i.UpsertItem(It.IsAny<Item>())).Returns(Task.FromResult(updateGroupId));
            _mockItemTagRepository.Setup(t => t.UpsertItemTags(It.IsAny<List<ItemAttributeTableValueParameter>>())).Returns(Task.FromResult(groupResponse));
            _mockItemGroupRepository.Setup(g => g.UpsertItemGroups(It.IsAny<List<ItemAttributeTableValueParameter>>())).Returns(Task.FromResult(tagResponse));
            _mockShoppingListRepository.Setup(s => s.UpsertShoppingListItems(updateGroupId, It.IsAny<List<ShoppingListTableValueParameter>>())).Returns(Task.FromResult(shoppingListResponse));
            var sut = new ItemManager(_mockItemRepository.Object, _mockTagRepository.Object, _mockGroupRepository.Object, _mockShoppingListRepository.Object, _mockItemGroupRepository.Object, _mockItemTagRepository.Object);

            // Act
            var response = await sut.UpsertItem(submitItem);

            // Assert
            _mockItemRepository.Verify(r => r.UpsertItem(submitItem), Times.Once, "Expected method to be called once");
            _mockItemTagRepository.Verify(r => r.UpsertItemTags(It.IsAny<List<ItemAttributeTableValueParameter>>()), Times.Once, "Expected method to be called once");
            _mockItemGroupRepository.Verify(r => r.UpsertItemGroups(It.IsAny<List<ItemAttributeTableValueParameter>>()), Times.Once, "Expected method to be called once");
            _mockShoppingListRepository.Verify(r => r.UpsertShoppingListItems(submitItem.ShoppingLists[0].Id, It.IsAny<List<ShoppingListTableValueParameter>>()), Times.Once, "Expected method to be called once");
            Assert.IsFalse(response.Success, "Expected response.Success to be false");
            Assert.IsTrue(response.Item.Id == updateGroupId, "Expected itemID equal to object ID passed in to be true");
            Assert.IsTrue(response.Item.UpdateDate > minDateValue, "Expected itemUpdateDate > than test value to be true");
            Assert.IsTrue(response.Item.CreateDate == createDateValue, "Expected itemCreateDate unchanged to be true");
        }

        [Test()]
        public async Task UpdateItemDetailShoppingListFalse()
        {
            // Arrange
            _mockItemRepository.Reset();
            _mockItemTagRepository.Reset();
            _mockItemGroupRepository.Reset();
            _mockShoppingListRepository.Reset();
            var updateGroupId = 55567;
            var updateDateValue = DateTime.Now;
            var minDateValue = DateTime.Now.AddMilliseconds(-1);
            var createDateValue = DateTime.Now.AddMonths(-1);
            var groupResponse = new RequestResponse<bool> { Item = true, Success = true };
            var tagResponse = new RequestResponse<bool> { Item = true, Success = true };
            var shoppingListResponse = new RequestResponse<bool> { Item = false, Success = false };
            var submitItem = new Item
            {
                CreateDate = createDateValue,
                Id = updateGroupId,
                IsActive = true,
                Name = "Unit Test",
                SubscriptionId = 667,
                UpdateDate = updateDateValue,
                Groups = new List<Group> { new Group { Id = 987 } },
                Tags = new List<Tag> { new Tag { Id = 987 } },
                ShoppingLists = new List<ShoppingList> { new ShoppingList { Id = 999 } }
            };
            _mockItemRepository.Setup(i => i.UpsertItem(It.IsAny<Item>())).Returns(Task.FromResult(updateGroupId));
            _mockItemTagRepository.Setup(t => t.UpsertItemTags(It.IsAny<List<ItemAttributeTableValueParameter>>())).Returns(Task.FromResult(groupResponse));
            _mockItemGroupRepository.Setup(g => g.UpsertItemGroups(It.IsAny<List<ItemAttributeTableValueParameter>>())).Returns(Task.FromResult(tagResponse));
            _mockShoppingListRepository.Setup(s => s.UpsertShoppingListItems(submitItem.ShoppingLists[0].Id, It.IsAny<List<ShoppingListTableValueParameter>>())).Returns(Task.FromResult(shoppingListResponse));
            var sut = new ItemManager(_mockItemRepository.Object, _mockTagRepository.Object, _mockGroupRepository.Object, _mockShoppingListRepository.Object, _mockItemGroupRepository.Object, _mockItemTagRepository.Object);

            // Act
            var response = await sut.UpsertItem(submitItem);

            // Assert
            _mockItemRepository.Verify(r => r.UpsertItem(submitItem), Times.Once, "Expected method to be called once");
            _mockItemTagRepository.Verify(r => r.UpsertItemTags(It.IsAny<List<ItemAttributeTableValueParameter>>()), Times.Once, "Expected method to be called once");
            _mockItemGroupRepository.Verify(r => r.UpsertItemGroups(It.IsAny<List<ItemAttributeTableValueParameter>>()), Times.Once, "Expected method to be called once");
            _mockShoppingListRepository.Verify(r => r.UpsertShoppingListItems(It.IsAny<int>(), It.IsAny<List<ShoppingListTableValueParameter>>()), Times.Once, "Expected method to be called once");
            Assert.IsFalse(response.Success, "Expected response.Success to be false");
            Assert.IsTrue(response.Item.Id == updateGroupId, "Expected itemID equal to object ID passed in to be true");
            Assert.IsTrue(response.Item.UpdateDate > minDateValue, "Expected itemUpdateDate > than test value to be true");
            Assert.IsTrue(response.Item.CreateDate == createDateValue, "Expected itemCreateDate unchanged to be true");
        }

        [Test()]
        public async Task UpdateItemDetailNoShoppingLists()
        {
            // Arrange
            _mockItemRepository.Reset();
            _mockItemTagRepository.Reset();
            _mockItemGroupRepository.Reset();
            _mockShoppingListRepository.Reset();
            var updateGroupId = 55567;
            var updateDateValue = DateTime.Now;
            var minDateValue = DateTime.Now.AddMilliseconds(-1);
            var createDateValue = DateTime.Now.AddMonths(-1);
            var groupResponse = new RequestResponse<bool> { Item = true, Success = true };
            var tagResponse = new RequestResponse<bool> { Item = true, Success = true };
            var shoppingListResponse = new RequestResponse<bool> { Item = true, Success = true };
            var submitItem = new Item
            {
                CreateDate = createDateValue,
                Id = updateGroupId,
                IsActive = true,
                Name = "Unit Test",
                SubscriptionId = 667,
                UpdateDate = updateDateValue,
                Groups = new List<Group> { new Group { Id = 987 } },
                Tags = new List<Tag> { new Tag { Id = 987 } },
                ShoppingLists = new List<ShoppingList>()
            };
            _mockItemRepository.Setup(i => i.UpsertItem(It.IsAny<Item>())).Returns(Task.FromResult(updateGroupId));
            _mockItemTagRepository.Setup(t => t.UpsertItemTags(It.IsAny<List<ItemAttributeTableValueParameter>>())).Returns(Task.FromResult(groupResponse));
            _mockItemGroupRepository.Setup(g => g.UpsertItemGroups(It.IsAny<List<ItemAttributeTableValueParameter>>())).Returns(Task.FromResult(tagResponse));
            _mockShoppingListRepository.Setup(s => s.UpsertShoppingListItems(It.IsAny<int>(), It.IsAny<List<ShoppingListTableValueParameter>>())).Returns(Task.FromResult(shoppingListResponse));
            var sut = new ItemManager(_mockItemRepository.Object, _mockTagRepository.Object, _mockGroupRepository.Object, _mockShoppingListRepository.Object, _mockItemGroupRepository.Object, _mockItemTagRepository.Object);

            // Act
            var response = await sut.UpsertItem(submitItem);

            // Assert
            _mockItemRepository.Verify(r => r.UpsertItem(submitItem), Times.Once, "Expected method to be called once");
            _mockItemTagRepository.Verify(r => r.UpsertItemTags(It.IsAny<List<ItemAttributeTableValueParameter>>()), Times.Once, "Expected method to be called once");
            _mockItemGroupRepository.Verify(r => r.UpsertItemGroups(It.IsAny<List<ItemAttributeTableValueParameter>>()), Times.Once, "Expected method to be called once");
            _mockShoppingListRepository.Verify(r => r.UpsertShoppingListItems(It.IsAny<int>(), It.IsAny<List<ShoppingListTableValueParameter>>()), Times.Never, "Expected method to not be called");
            Assert.IsTrue(response.Success, "Expected response.Success to be true");
            Assert.IsTrue(response.Item.Id == updateGroupId, "Expected itemID equal to object ID passed in to be true");
            Assert.IsTrue(response.Item.UpdateDate > minDateValue, "Expected itemUpdateDate > than test value to be true");
            Assert.IsTrue(response.Item.CreateDate == createDateValue, "Expected itemCreateDate unchanged to be true");
        }

        [Test()]
        public async Task UpdateItemDetailNoGroups()
        {
            // Arrange
            _mockItemRepository.Reset();
            _mockItemTagRepository.Reset();
            _mockItemGroupRepository.Reset();
            _mockShoppingListRepository.Reset();
            var updateGroupId = 55567;
            var updateDateValue = DateTime.Now;
            var minDateValue = DateTime.Now.AddMilliseconds(-1);
            var createDateValue = DateTime.Now.AddMonths(-1);
            var groupResponse = new RequestResponse<bool> { Item = true, Success = true };
            var tagResponse = new RequestResponse<bool> { Item = true, Success = true };
            var shoppingListResponse = new RequestResponse<bool> { Item = true, Success = true };
            var submitItem = new Item
            {
                CreateDate = createDateValue,
                Id = updateGroupId,
                IsActive = true,
                Name = "Unit Test",
                SubscriptionId = 667,
                UpdateDate = updateDateValue,
                Groups = new List<Group>(),
                Tags = new List<Tag> { new Tag { Id = 987 } },
                ShoppingLists = new List<ShoppingList> { new ShoppingList { Id = 999 } }
            };
            _mockItemRepository.Setup(i => i.UpsertItem(It.IsAny<Item>())).Returns(Task.FromResult(updateGroupId));
            _mockItemTagRepository.Setup(t => t.UpsertItemTags(It.IsAny<List<ItemAttributeTableValueParameter>>())).Returns(Task.FromResult(groupResponse));
            _mockItemGroupRepository.Setup(g => g.UpsertItemGroups(It.IsAny<List<ItemAttributeTableValueParameter>>())).Returns(Task.FromResult(tagResponse));
            _mockShoppingListRepository.Setup(s => s.UpsertShoppingListItems(It.IsAny<int>(), It.IsAny<List<ShoppingListTableValueParameter>>())).Returns(Task.FromResult(shoppingListResponse));
            var sut = new ItemManager(_mockItemRepository.Object, _mockTagRepository.Object, _mockGroupRepository.Object, _mockShoppingListRepository.Object, _mockItemGroupRepository.Object, _mockItemTagRepository.Object);

            // Act
            var response = await sut.UpsertItem(submitItem);

            // Assert
            _mockItemRepository.Verify(r => r.UpsertItem(submitItem), Times.Once, "Expected method to be called once");
            _mockItemTagRepository.Verify(r => r.UpsertItemTags(It.IsAny<List<ItemAttributeTableValueParameter>>()), Times.Once, "Expected method to be called once");
            _mockItemGroupRepository.Verify(r => r.UpsertItemGroups(It.IsAny<List<ItemAttributeTableValueParameter>>()), Times.Never, "Expected method to not be called");
            _mockShoppingListRepository.Verify(r => r.UpsertShoppingListItems(It.IsAny<int>(), It.IsAny<List<ShoppingListTableValueParameter>>()), Times.Once, "Expected method to be called once");
            Assert.IsTrue(response.Success, "Expected response.Success to be true");
            Assert.IsTrue(response.Item.Id == updateGroupId, "Expected itemID equal to object ID passed in to be true");
            Assert.IsTrue(response.Item.UpdateDate > minDateValue, "Expected itemUpdateDate > than test value to be true");
            Assert.IsTrue(response.Item.CreateDate == createDateValue, "Expected itemCreateDate unchanged to be true");
        }

        [Test()]
        public async Task UpdateItemDetailNoTags()
        {
            // Arrange
            _mockItemRepository.Reset();
            _mockItemTagRepository.Reset();
            _mockItemGroupRepository.Reset();
            _mockShoppingListRepository.Reset();
            var updateGroupId = 55567;
            var updateDateValue = DateTime.Now;
            var minDateValue = DateTime.Now.AddMilliseconds(-1);
            var createDateValue = DateTime.Now.AddMonths(-1);
            var groupResponse = new RequestResponse<bool> { Item = true, Success = true };
            var tagResponse = new RequestResponse<bool> { Item = true, Success = true };
            var shoppingListResponse = new RequestResponse<bool> { Item = true, Success = true };
            var submitItem = new Item
            {
                CreateDate = createDateValue,
                Id = updateGroupId,
                IsActive = true,
                Name = "Unit Test",
                SubscriptionId = 667,
                UpdateDate = updateDateValue,
                Groups = new List<Group> { new Group { Id = 987 } },
                Tags = new List<Tag>(),
                ShoppingLists = new List<ShoppingList> { new ShoppingList { Id = 999 } }
            };
            _mockItemRepository.Setup(i => i.UpsertItem(It.IsAny<Item>())).Returns(Task.FromResult(updateGroupId));
            _mockItemTagRepository.Setup(t => t.UpsertItemTags(It.IsAny<List<ItemAttributeTableValueParameter>>())).Returns(Task.FromResult(groupResponse));
            _mockItemGroupRepository.Setup(g => g.UpsertItemGroups(It.IsAny<List<ItemAttributeTableValueParameter>>())).Returns(Task.FromResult(tagResponse));
            _mockShoppingListRepository.Setup(s => s.UpsertShoppingListItems(It.IsAny<int>(), It.IsAny<List<ShoppingListTableValueParameter>>())).Returns(Task.FromResult(shoppingListResponse));
            var sut = new ItemManager(_mockItemRepository.Object, _mockTagRepository.Object, _mockGroupRepository.Object, _mockShoppingListRepository.Object, _mockItemGroupRepository.Object, _mockItemTagRepository.Object);

            // Act
            var response = await sut.UpsertItem(submitItem);

            // Assert
            _mockItemRepository.Verify(r => r.UpsertItem(submitItem), Times.Once, "Expected method to be called once");
            _mockItemTagRepository.Verify(r => r.UpsertItemTags(It.IsAny<List<ItemAttributeTableValueParameter>>()), Times.Never, "Expected method to not be called");
            _mockItemGroupRepository.Verify(r => r.UpsertItemGroups(It.IsAny<List<ItemAttributeTableValueParameter>>()), Times.Once, "Expected method to be called once");
            _mockShoppingListRepository.Verify(r => r.UpsertShoppingListItems(It.IsAny<int>(), It.IsAny<List<ShoppingListTableValueParameter>>()), Times.Once, "Expected method to be called once");
            Assert.IsTrue(response.Success, "Expected response.Success to be true");
            Assert.IsTrue(response.Item.Id == updateGroupId, "Expected itemID equal to object ID passed in to be true");
            Assert.IsTrue(response.Item.UpdateDate > minDateValue, "Expected itemUpdateDate > than test value to be true");
            Assert.IsTrue(response.Item.CreateDate == createDateValue, "Expected itemCreateDate unchanged to be true");
        }

        [Test()]
        public async Task UpdateGroupFailTest()
        {
            // Arrange
            _mockItemRepository.Reset();
            var updateGroupId = 55567;
            var submitItem = new Item { CreateDate = DateTime.Now, Id = updateGroupId, IsActive = true, Name = "Unit Test", SubscriptionId = 667, UpdateDate = DateTime.Now };
            _mockItemRepository.Setup(i => i.UpsertItem(It.IsAny<Item>())).Throws(_expectedException);
            _mockItemRepository.Setup(i => i.GetItemCollection(It.IsAny<int>(), It.IsAny<SearchRequest>())).Returns(Task.FromResult(_allItems));
            var sut = new ItemManager(_mockItemRepository.Object, _mockTagRepository.Object, _mockGroupRepository.Object, _mockShoppingListRepository.Object, _mockItemGroupRepository.Object, _mockItemTagRepository.Object);

            // Act
            var response = await sut.UpsertItem(submitItem);

            // Assert
            _mockItemRepository.Verify(r => r.UpsertItem(submitItem), Times.Once, "Expected method to be called once");
            Assert.IsTrue(!response.Success, "Expected response.Success to be false");
        }
    }
}