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
    public class ShoppingListManagerTests
    {
        private Mock<IShoppingListRepository> _mockShoppingListRepository;
        private Mock<ILogger> _mockLogger;
        private RequestResponse<ShoppingListItemCollection> _shoppingListItemCollectionResponse;
        private RequestResponse<List<ShoppingList>> _shoppingListCollectionResponse;
        private RequestResponse<List<ShoppingListItem>> _shoppingListItemsResponse;
        private RequestResponse<ShoppingList> _upsertShoppingListResponse;
        private RequestResponse<bool> _boolResponse;
        private List<ShoppingList> _shoppingListCollection;
        private List<ShoppingListItem> _shoppingListItems;
        private List<ShoppingListTableValueParameter> _tvpItems;
        private PagedCollection _pagedCollection;
        private NullReferenceException _expectedException;
        private SearchRequest _searchRequest;


        [OneTimeSetUp]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task TestSetup()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            _expectedException = new NullReferenceException("Test Went Boom!");
            _mockShoppingListRepository = new Mock<IShoppingListRepository>();
            _pagedCollection = new PagedCollection { CollectionTotal = 6, PageNumber = 1, PageSize = 3 };
            _shoppingListCollection = ShoppingListManagerSetup.GetShoppingLists();
            _shoppingListItems = ShoppingListManagerSetup.GetShoppingListItems();
            _tvpItems = ShoppingListManagerSetup.GetItemTvp();

            _shoppingListItemCollectionResponse = new RequestResponse<ShoppingListItemCollection>
            {
                Item = new ShoppingListItemCollection { ShoppingListItems = _shoppingListItems, ItemGroups = new List<ItemGroup>(), ItemTags = new List<ItemTag>()},
                PagedCollection = _pagedCollection
            };
            _shoppingListItemsResponse = new RequestResponse<List<ShoppingListItem>>
            {
                Item = _shoppingListItems,
                PagedCollection = _pagedCollection
            };
            _shoppingListCollectionResponse = new RequestResponse<List<ShoppingList>>
            {
                Item = _shoppingListCollection,
                PagedCollection = _pagedCollection
            };

            _boolResponse = new RequestResponse<bool>
            {
                Item = true,
                PagedCollection = null,
                Success = true
            };
            _upsertShoppingListResponse = new RequestResponse<ShoppingList>
            {
                PagedCollection = null,
                Item = new ShoppingList { Id = 2098, IsActive = true, Name = "New Unit test List", SubscriptionId = 1 },
                Success = true
            };

            _searchRequest = new SearchRequest { PagedCollection = new PagedCollection { CollectionTotal = 20, PageSize = 10, PageNumber = 1 }, SortAscending = true, SortBy = "name", IdCollection = new List<int>() };
            _mockLogger = new Mock<ILogger>();
        }

        [Test()]
        public async Task GetShoppingListCollectionResultSuccess()
        {
            // Arrange
            _mockShoppingListRepository.Reset();
            _mockShoppingListRepository.Setup(r => r.GetShoppingListCollection(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns(Task.FromResult(_shoppingListCollectionResponse));
            var sut = new ShoppingListManager(_mockShoppingListRepository.Object);
            _shoppingListCollectionResponse.Success = true;

            // Act
            var response = await sut.GetShoppingListCollection(1, 1, 3);

            // Assert
            _mockShoppingListRepository.Verify(r => r.GetShoppingListCollection(1, 1, 3), Times.Once, "Expected method to be called once");
            Assert.IsTrue(response.Success, "Expected response.Success to be true");
        }

        [Test()]
        public async Task GetShoppingListCollectionResultFail()
        {
            // Arrange
            _mockShoppingListRepository.Reset();
            _mockShoppingListRepository.Setup(r => r.GetShoppingListCollection(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Throws(_expectedException);
            var sut = new ShoppingListManager(_mockShoppingListRepository.Object);

            // Act
            var response = await sut.GetShoppingListCollection(1, 1, 3);

            // Assert
            _mockShoppingListRepository.Verify(r => r.GetShoppingListCollection(1, 1, 3), Times.Once, "Expected method to be called once");
            Assert.IsTrue(!response.Success, "Expected response.Success to be false");
        }

        [Test()]
        public async Task GetShoppingListItemsResultSuccess()
        {
            // Arrange
            _mockShoppingListRepository.Reset();
            _mockShoppingListRepository.Setup(r => r.GetShoppingListItems(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<SearchRequest>()))
                .Returns(Task.FromResult(_shoppingListItemCollectionResponse));
            var sut = new ShoppingListManager(_mockShoppingListRepository.Object);
            _shoppingListItemsResponse.Success = true;

            // Act
            var response = await sut.GetShoppingListItems(1, 1, _searchRequest);

            // Assert
            _mockShoppingListRepository.Verify(r => r.GetShoppingListItems(1, 1, _searchRequest), Times.Once, "Expected method to be called once");
            Assert.IsTrue(response.Success, "Expected response.Success to be true");
        }

        [Test()]
        public async Task GetShoppingListItemsResultFail()
        {
            // Arrange
            _mockShoppingListRepository.Reset();
            _mockShoppingListRepository.Setup(r => r.GetShoppingListItems(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<SearchRequest>())).Throws(_expectedException);
            var sut = new ShoppingListManager(_mockShoppingListRepository.Object);

            // Act
            var response = await sut.GetShoppingListItems(1, 1, _searchRequest);

            // Assert
            _mockShoppingListRepository.Verify(r => r.GetShoppingListItems(1, 1, _searchRequest), Times.Once, "Expected method to be called once");
            Assert.IsTrue(!response.Success, "Expected response.Success to be false");
        }

        [Test()]
        public async Task ShoppingListCheckoutResultSuccess()
        {
            // Arrange
            _mockShoppingListRepository.Reset();
            _mockShoppingListRepository.Setup(r => r.ShoppingListCheckout(It.IsAny<List<ShoppingListTableValueParameter>>()))
                .Returns(Task.FromResult(_boolResponse));
            var sut = new ShoppingListManager(_mockShoppingListRepository.Object);
            _boolResponse.Success = true;

            // Act
            var response = await sut.ShoppingListCheckout(_tvpItems);

            // Assert
            _mockShoppingListRepository.Verify(r => r.ShoppingListCheckout(_tvpItems), Times.Once, "Expected method to be called once");
            Assert.IsTrue(response.Success, "Expected response.Success to be true");
        }

        [Test()]
        public async Task ShoppingListCheckoutResultFail()
        {
            // Arrange
            _mockShoppingListRepository.Reset();
            _mockShoppingListRepository.Setup(r => r.ShoppingListCheckout(It.IsAny<List<ShoppingListTableValueParameter>>())).Throws(_expectedException);
            var sut = new ShoppingListManager(_mockShoppingListRepository.Object);

            // Act
            var response = await sut.ShoppingListCheckout(_tvpItems);

            // Assert
            _mockShoppingListRepository.Verify(r => r.ShoppingListCheckout(_tvpItems), Times.Once, "Expected method to be called once");
            Assert.IsTrue(!response.Success, "Expected response.Success to be false");
        }

        [Test()]
        public async Task UpsertShoppingListResultSuccess()
        {
            // Arrange
            _mockShoppingListRepository.Reset();
            _mockShoppingListRepository.Setup(r => r.UpsertShoppingList(It.IsAny<ShoppingList>()))
                .Returns(Task.FromResult(_upsertShoppingListResponse));
            var sut = new ShoppingListManager(_mockShoppingListRepository.Object);
            _upsertShoppingListResponse.Success = true;
            var upsertListValue = new ShoppingList
            {
                Id = 0,
                SubscriptionId = 1,
                Name = "New Unit test List"
            };

            // Act
            var response = await sut.UpsertShoppingList(upsertListValue);

            // Assert
            _mockShoppingListRepository.Verify(r => r.UpsertShoppingList(upsertListValue), Times.Once, "Expected method to be called once");
            Assert.IsTrue(response.Success, "Expected response.Success to be true");
        }

        [Test()]
        public async Task UpsertShoppingListResultFail()
        {
            // Arrange
            _mockShoppingListRepository.Reset();
            _mockShoppingListRepository.Setup(r => r.UpsertShoppingList(It.IsAny<ShoppingList>())).Throws(_expectedException);
            var sut = new ShoppingListManager(_mockShoppingListRepository.Object);
            var upsertListValue = new ShoppingList
            {
                Id = 0,
                SubscriptionId = 1,
                Name = "New Unit test List"
            };

            // Act
            var response = await sut.UpsertShoppingList(upsertListValue);

            // Assert
            _mockShoppingListRepository.Verify(r => r.UpsertShoppingList(upsertListValue), Times.Once, "Expected method to be called once");
            Assert.IsTrue(!response.Success, "Expected response.Success to be false");
        }

        [Test()]
        public async Task UpsertShoppingListItemsResultSuccess()
        {
            // Arrange
            _mockShoppingListRepository.Reset();
            _mockShoppingListRepository.Setup(r => r.UpsertShoppingListItems(It.IsAny <int>(), It.IsAny<List<ShoppingListTableValueParameter>>()))
                .Returns(Task.FromResult(_boolResponse));
            var sut = new ShoppingListManager(_mockShoppingListRepository.Object);
            _boolResponse.Success = true;

            // Act
            var response = await sut.UpsertShoppingListItems(1, _tvpItems);

            // Assert
            _mockShoppingListRepository.Verify(r => r.UpsertShoppingListItems(1, _tvpItems), Times.Once, "Expected method to be called once");
            Assert.IsTrue(response.Success, "Expected response.Success to be true");
        }

        [Test()]
        public async Task UpsertShoppingListItemsResultFail()
        {
            // Arrange
            _mockShoppingListRepository.Reset();
            _mockShoppingListRepository.Setup(r => r.UpsertShoppingListItems(It.IsAny<int>(), It.IsAny<List<ShoppingListTableValueParameter>>())).Throws(_expectedException);
            var sut = new ShoppingListManager(_mockShoppingListRepository.Object);

            // Act
            var response = await sut.UpsertShoppingListItems(1, _tvpItems);

            // Assert
            _mockShoppingListRepository.Verify(r => r.UpsertShoppingListItems(1, _tvpItems), Times.Once, "Expected method to be called once");
            Assert.IsTrue(!response.Success, "Expected response.Success to be false");
        }

        [Test()]
        public async Task InitShoppingListCheckoutFailure()
        {
            // Arrange
            _mockShoppingListRepository.Reset();
            _mockShoppingListRepository.Setup(r => r.InitShoppingListCheckout( It.IsAny<List<ShoppingListTableValueParameter>>())).Throws(_expectedException);
            var sut = new ShoppingListManager(_mockShoppingListRepository.Object);

            // Act
            var response = await sut.InitShoppingListCheckout(_tvpItems);

            // Assert
            _mockShoppingListRepository.Verify(r => r.InitShoppingListCheckout(_tvpItems), Times.Once, "Expected method to be called once");
            Assert.IsTrue(!response.Success, "Expected response.Success to be false");
        }

        [Test()]
        public async Task InitShoppingListCheckoutSuccess()
        {
            // Arrange
            var initResponse = new RequestResponse<List<ShoppingListTableValueParameter>>();
            initResponse.Success = true;
            initResponse.Item = _tvpItems;
            _mockShoppingListRepository.Reset();
            _mockShoppingListRepository.Setup(r => r.InitShoppingListCheckout(It.IsAny<List<ShoppingListTableValueParameter>>()))
                .Returns(Task.FromResult(initResponse));
            var sut = new ShoppingListManager(_mockShoppingListRepository.Object);
            _boolResponse.Success = true;

            // Act
            var response = await sut.InitShoppingListCheckout(_tvpItems);

            // Assert
            _mockShoppingListRepository.Verify(r => r.InitShoppingListCheckout(_tvpItems), Times.Once, "Expected method to be called once");
            Assert.IsTrue(response.Success, "Expected response.Success to be true");
        }

        [Test()]
        public async Task UpdateShoppingListItemCheckoutStatusFailure()
        {
            // Arrange
            var slItem = new ShoppingListItem();
            _mockShoppingListRepository.Reset();
            _mockShoppingListRepository.Setup(r => r.UpdateShoppingListItemCheckoutStatus(It.IsAny<ShoppingListItem>())).Throws(_expectedException);
            var sut = new ShoppingListManager(_mockShoppingListRepository.Object);

            // Act
            var response = await sut.UpdateShoppingListItemCheckoutStatus(slItem);

            // Assert
            _mockShoppingListRepository.Verify(r => r.UpdateShoppingListItemCheckoutStatus(slItem), Times.Once, "Expected method to be called once");
            Assert.IsTrue(!response.Success, "Expected response.Success to be false");
        }

        [Test()]
        public async Task UpdateShoppingListItemCheckoutStatusSuccess()
        {
            // Arrange
            var slItem = new ShoppingListItem();
            var updateResponse = new RequestResponse<List<ShoppingListTableValueParameter>>();
            _mockShoppingListRepository.Reset();
            _mockShoppingListRepository.Setup(r => r.UpdateShoppingListItemCheckoutStatus(It.IsAny<ShoppingListItem>()))
                .Returns(Task.FromResult(updateResponse));
            var sut = new ShoppingListManager(_mockShoppingListRepository.Object);

            // Act
            var response = await sut.UpdateShoppingListItemCheckoutStatus(slItem);

            // Assert
            _mockShoppingListRepository.Verify(r => r.UpdateShoppingListItemCheckoutStatus(slItem), Times.Once, "Expected method to be called once");
            Assert.IsTrue(!response.Success, "Expected response.Success to be false");
        }

        [Test()]
        public async Task SyncShoppingListItemsSuccess()
        {
            // Arrange
            var syncResponse = new RequestResponse<List<ShoppingListTableValueParameter>>();
            _mockShoppingListRepository.Reset();
            _mockShoppingListRepository.Setup(r => r.SyncShoppingListItems(It.IsAny<int>()))
                .Returns(Task.FromResult(syncResponse));
            var sut = new ShoppingListManager(_mockShoppingListRepository.Object);

            // Act
            var response = await sut.SyncShoppingListItems(667);

            // Assert
            _mockShoppingListRepository.Verify(r => r.SyncShoppingListItems(667), Times.Once, "Expected method to be called once");
            Assert.IsTrue(!response.Success, "Expected response.Success to be false");
        }
        [Test()]
        public async Task SyncShoppingListItemsFailure()
        {
            // Arrange
            var slItem = new ShoppingListItem();
            _mockShoppingListRepository.Reset();
            _mockShoppingListRepository.Setup(r => r.SyncShoppingListItems(It.IsAny<int>())).Throws(_expectedException);
            var sut = new ShoppingListManager(_mockShoppingListRepository.Object);

            // Act
            var response = await sut.SyncShoppingListItems(665);

            // Assert
            _mockShoppingListRepository.Verify(r => r.SyncShoppingListItems(665), Times.Once, "Expected method to be called once");
            Assert.IsTrue(!response.Success, "Expected response.Success to be false");
        }

    }
}