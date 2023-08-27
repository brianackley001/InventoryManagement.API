using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using InventoryManagement.API.Models;
using Moq;
using InventoryManagement.API.Managers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NLog;
using NLog.Fluent;

namespace InventoryManagement.API.Controllers.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture()]
    [Category("ControllerTests")]
    public class ShoppingListControllerTests
    {
        private Mock<IShoppingListManager> _shoppingListManager;
        private Mock<ILogger> _mockLogger;
        private RequestResponse<List<ShoppingList>> _shoppingListGet;
        private RequestResponse<List<ShoppingListItem>> _shoppingListItemGet;
        private RequestResponse<ShoppingList> _shoppingListResponse;
        private List<ShoppingListTableValueParameter> _tvpItems;
        private NullReferenceException _expectedException;
        private SearchRequest _searchRequest;

        [ExcludeFromCodeCoverage, OneTimeSetUp]
        public void TestSetup()
        {
            _expectedException = new NullReferenceException("Test Went Boom!");
            _shoppingListManager = new Mock<IShoppingListManager>();
            _mockLogger = new Mock<ILogger>();
            _shoppingListGet = new RequestResponse<List<ShoppingList>>
            {
                Success = true,
                PagedCollection = new PagedCollection { PageNumber = 1, PageSize = 10, CollectionTotal = 5 },
                Item = new List<ShoppingList>
                {
                    new ShoppingList{Id=1, IsActive=true, Name="One", SubscriptionId=1},
                    new ShoppingList{Id=2, IsActive=true, Name="Two", SubscriptionId=1},
                    new ShoppingList{Id=3, IsActive=true, Name="Three", SubscriptionId=1},
                    new ShoppingList{Id=4, IsActive=true, Name="Four", SubscriptionId=1},
                    new ShoppingList{Id=5, IsActive=true, Name="Five", SubscriptionId=1}
                }
            };
            _shoppingListItemGet = new RequestResponse<List<ShoppingListItem>>
            {
                Success = true,
                PagedCollection = new PagedCollection(),
                Item = new List<ShoppingListItem>
                {
                    new ShoppingListItem{AmountValue=1, Id=232, ItemId=987, Description="Test", Name="Name", ItemIsActive=true, SubscriptionId=1, ShoppingListId=3, ShoppingListIsActive=true},
                    new ShoppingListItem{AmountValue=1, Id=3434, ItemId=333, Description="Test", Name="Name", ItemIsActive=true, SubscriptionId=1, ShoppingListId=3, ShoppingListIsActive=true},
                    new ShoppingListItem{AmountValue=1, Id=945945784, ItemId=789789, Description="Test", Name="Name", ItemIsActive=true, SubscriptionId=1, ShoppingListId=3, ShoppingListIsActive=true}
                }
            };
            _shoppingListResponse = new RequestResponse<ShoppingList>
            {
                Success = true,
                PagedCollection = new PagedCollection(),
                Item = new ShoppingList { Id = 98989, IsActive = true, Name = "NewOnetest", SubscriptionId = 1 }
            };
            _tvpItems = new List<ShoppingListTableValueParameter>
            {
                new ShoppingListTableValueParameter{IsActive=true, ItemId=12, ShoppingListId=9, SubscriptionId=1},
                new ShoppingListTableValueParameter{IsActive=true, ItemId=444, ShoppingListId=9, SubscriptionId=1},
                new ShoppingListTableValueParameter{IsActive=true, ItemId=1256565, ShoppingListId=9, SubscriptionId=1}
            };
            _searchRequest = new SearchRequest { PagedCollection = new PagedCollection { CollectionTotal = 20, PageSize = 10, PageNumber = 1 }, SortAscending = true, SortBy = "name", IdCollection = new List<int>() };
        }


        [Test()]
        public async Task GetShoppingListCollectionOKResultTest()
        {
            // Arrange
            _shoppingListManager.Reset();
            _shoppingListManager.Setup(s => s.GetShoppingListCollection(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(_shoppingListGet));
            var sut = new ShoppingListController(_shoppingListManager.Object);

            // Act
            var response = await sut.GetShoppingLists(1,1,10);
            var okObjectResult = response as OkObjectResult;
            var responseModel = okObjectResult.Value as RequestResponse<List<ShoppingList>>;

            // Assert
            Assert.IsNotNull(response, "response != null");
            Assert.IsNotNull(responseModel, "responseModel != null");
            Assert.IsNotNull(responseModel.Item, "responseModel.Item != null");
            _shoppingListManager.Verify(m => m.GetShoppingListCollection(1, 1, 10), Times.Once, "Expected method to be called once");
        }

        [Test()]
        public async Task GetShoppingListCollection500Test()
        {
            // Arrange
            _shoppingListManager.Reset();
            _shoppingListManager.Setup(s => s.GetShoppingListCollection(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).
                Throws(_expectedException);
            var sut = new ShoppingListController(_shoppingListManager.Object);

            // Act
            var response = await sut.GetShoppingLists(1, 1, 10);
            var statusCodeResult = response as StatusCodeResult;

            // Assert
            Assert.IsNotNull(statusCodeResult, "statusCodeResult != null");
            Assert.IsTrue(statusCodeResult.StatusCode == 500);
            _shoppingListManager.Verify(m => m.GetShoppingListCollection(1, 1, 10), Times.Once, "Expected method to be called once");
        }
        [Test()]
        public async Task GetShoppingListItemsOKResultTest()
        {
            // Arrange
            _shoppingListManager.Reset();
            _shoppingListManager.Setup(s => s.GetShoppingListItems(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<SearchRequest>())).Returns(Task.FromResult(_shoppingListItemGet));
            var sut = new ShoppingListController(_shoppingListManager.Object);

            // Act
            var response = await sut.GetShoppingListItems(1, 1, _searchRequest);
            var okObjectResult = response as OkObjectResult;
            var responseModel = okObjectResult.Value as RequestResponse<List<ShoppingListItem>>;

            // Assert
            Assert.IsNotNull(response, "response != null");
            Assert.IsNotNull(responseModel, "responseModel != null");
            Assert.IsNotNull(responseModel.Item, "responseModel.Item != null");
            _shoppingListManager.Verify(m => m.GetShoppingListItems(1, 1, _searchRequest), Times.Once, "Expected method to be called once");
        }
        [Test()]
        public async Task GetShoppingListItems500Test()
        {
            // Arrange
            _shoppingListManager.Reset();
            _shoppingListManager.Setup(s => s.GetShoppingListItems(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<SearchRequest>())).
                Throws(_expectedException);
            var sut = new ShoppingListController(_shoppingListManager.Object);

            // Act
            var response = await sut.GetShoppingListItems(1, 1,_searchRequest);
            var statusCodeResult = response as StatusCodeResult;

            // Assert
            Assert.IsNotNull(statusCodeResult, "statusCodeResult != null");
            Assert.IsTrue(statusCodeResult.StatusCode == 500);
            _shoppingListManager.Verify(m => m.GetShoppingListItems(1, 1, _searchRequest), Times.Once, "Expected method to be called once");
        }
        [Test()]
        public async Task ShoppingListCheckoutOKResultTest()
        {
            // Arrange
            _shoppingListManager.Reset();
            var requestResponse = new RequestResponse<bool> { Item = true, PagedCollection = new PagedCollection(), Success = true };
            _shoppingListManager.Setup(s => s.ShoppingListCheckout(It.IsAny<List<ShoppingListTableValueParameter>>())).Returns(Task.FromResult(requestResponse));
            var sut = new ShoppingListController(_shoppingListManager.Object);

            // Act
            var response = await sut.ShoppingListCheckout(_tvpItems);
            var okObjectResult = response as OkObjectResult;
            var responseModel = okObjectResult.Value as RequestResponse<bool>;

            // Assert
            Assert.IsNotNull(response, "response != null");
            Assert.IsNotNull(responseModel, "responseModel != null");
            Assert.IsNotNull(responseModel.Item, "responseModel.Item != null");
            _shoppingListManager.Verify(m => m.ShoppingListCheckout(_tvpItems), Times.Once, "Expected method to be called once");
        }
        [Test()]
        public async Task ShoppingListCheckout500Test()
        {
            // Arrange
            _shoppingListManager.Reset();
            var requestResponse = new RequestResponse<bool> { Item = true, PagedCollection = new PagedCollection(), Success = true };
            _shoppingListManager.Setup(s => s.ShoppingListCheckout(It.IsAny<List<ShoppingListTableValueParameter>>())).
                Throws(_expectedException);
            var sut = new ShoppingListController(_shoppingListManager.Object);

            // Act
            var response = await sut.ShoppingListCheckout(_tvpItems);
            var statusCodeResult = response as StatusCodeResult;

            // Assert
            Assert.IsNotNull(statusCodeResult, "statusCodeResult != null");
            Assert.IsTrue(statusCodeResult.StatusCode == 500);
            _shoppingListManager.Verify(m => m.ShoppingListCheckout(_tvpItems), Times.Once, "Expected method to be called once");
        }
        [Test()]
        public async Task UpsertShoppingListOKResultTest()
        {
            // Arrange
            _shoppingListManager.Reset();
            var upsertList = new ShoppingList { SubscriptionId = 1, Name = "TestOneItem" };
            _shoppingListManager.Setup(s => s.UpsertShoppingList(It.IsAny<ShoppingList>())).Returns(Task.FromResult(_shoppingListResponse));
            var sut = new ShoppingListController(_shoppingListManager.Object);

            // Act
            var response = await sut.UpsertShoppingList(upsertList);
            var okObjectResult = response as OkObjectResult;
            var responseModel = okObjectResult.Value as RequestResponse<ShoppingList>;

            // Assert
            Assert.IsNotNull(response, "response != null");
            Assert.IsNotNull(responseModel, "responseModel != null");
            Assert.IsNotNull(responseModel.Item, "responseModel.Item != null");
            _shoppingListManager.Verify(m => m.UpsertShoppingList(upsertList), Times.Once, "Expected method to be called once");
        }
        [Test()]
        public async Task UpsertShoppingList500Test()
        {
            // Arrange
            _shoppingListManager.Reset();
            var upsertList = new ShoppingList { SubscriptionId = 1, Name = "TestOneItem" };
            _shoppingListManager.Setup(s => s.UpsertShoppingList(It.IsAny<ShoppingList>()))
                .Throws(_expectedException);
            var sut = new ShoppingListController(_shoppingListManager.Object);

            // Act
            var response = await sut.UpsertShoppingList(upsertList);
            var statusCodeResult = response as StatusCodeResult;

            // Assert
            Assert.IsNotNull(statusCodeResult, "statusCodeResult != null");
            Assert.IsTrue(statusCodeResult.StatusCode == 500);
            _shoppingListManager.Verify(m => m.UpsertShoppingList(upsertList), Times.Once, "Expected method to be called once");
        }
        [Test()]
        public async Task UpsertShoppingListItemsOKResultTest()
        {
            // Arrange
            _shoppingListManager.Reset();
            var requestResponse = new RequestResponse<bool> { Item = true, PagedCollection = new PagedCollection(), Success = true };
            _shoppingListManager.Setup(s => s.UpsertShoppingListItems(It.IsAny<int>(), It.IsAny<List<ShoppingListTableValueParameter>>())).Returns(Task.FromResult(requestResponse));
            var sut = new ShoppingListController(_shoppingListManager.Object);

            // Act
            var response = await sut.UpsertShoppingListItems(1, _tvpItems);
            var okObjectResult = response as OkObjectResult;
            var responseModel = okObjectResult.Value as RequestResponse<bool>;

            // Assert
            Assert.IsNotNull(response, "response != null");
            Assert.IsNotNull(responseModel, "responseModel != null");
            Assert.IsNotNull(responseModel.Item, "responseModel.Item != null");
            _shoppingListManager.Verify(m => m.UpsertShoppingListItems(1, _tvpItems), Times.Once, "Expected method to be called once");
        }
        [Test()]
        public async Task UpsertShoppingListItems500Test()
        {
            // Arrange
            _shoppingListManager.Reset();
            _shoppingListManager.Setup(s => s.UpsertShoppingListItems(It.IsAny<int>(), It.IsAny<List<ShoppingListTableValueParameter>>()))
                .Throws(_expectedException);
            var sut = new ShoppingListController(_shoppingListManager.Object);

            // Act
            var response = await sut.UpsertShoppingListItems(1, _tvpItems);
            var statusCodeResult = response as StatusCodeResult;

            // Assert
            Assert.IsNotNull(statusCodeResult, "statusCodeResult != null");
            Assert.IsTrue(statusCodeResult.StatusCode == 500);
            _shoppingListManager.Verify(m => m.UpsertShoppingListItems(1, _tvpItems), Times.Once, "Expected method to be called once");
        }
        [Test()]
        public async Task SyncShoppingListCheckoutItems500Test()
        {
            // Arrange
            var slItem = new ShoppingListItem();
            _shoppingListManager.Reset();
            _shoppingListManager.Setup(s => s.UpdateShoppingListItemCheckoutStatus(It.IsAny<ShoppingListItem>()))
                .Throws(_expectedException);
            var sut = new ShoppingListController(_shoppingListManager.Object);

            // Act
            var response = await sut.SyncShoppingListCheckoutItemStatus(slItem);
            var statusCodeResult = response as StatusCodeResult;

            // Assert
            Assert.IsNotNull(statusCodeResult, "statusCodeResult != null");
            Assert.IsTrue(statusCodeResult.StatusCode == 500);
            _shoppingListManager.Verify(m => m.UpdateShoppingListItemCheckoutStatus(slItem), Times.Once, "Expected method to be called once");
        }
        [Test()]
        public async Task SyncShoppingListCheckoutItemsOKResultTest()
        {
            // Arrange
            _shoppingListManager.Reset();
            var slItem = new ShoppingListItem();
            var updateResponse = new RequestResponse<List<ShoppingListTableValueParameter>>
            {
                Item = new List<ShoppingListTableValueParameter>(),
                PagedCollection = new PagedCollection(),
                Success = true
            };
            var requestResponse = new RequestResponse<List<ShoppingListTableValueParameter>> { 
                Item = new List<ShoppingListTableValueParameter>(), 
                PagedCollection = new PagedCollection(), 
                Success = true };
            _shoppingListManager.Setup(s => s.UpdateShoppingListItemCheckoutStatus(It.IsAny<ShoppingListItem>()))
                .Returns(Task.FromResult(updateResponse));
            var sut = new ShoppingListController(_shoppingListManager.Object);

            // Act
            var response = await sut.SyncShoppingListCheckoutItemStatus(slItem);
            var okObjectResult = response as OkObjectResult;
            var responseModel = okObjectResult.Value as RequestResponse<List<ShoppingListTableValueParameter>>;

            // Assert
            Assert.IsNotNull(response, "response != null");
            Assert.IsNotNull(responseModel, "responseModel != null");
            Assert.IsNotNull(responseModel.Item, "responseModel.Item != null");
            _shoppingListManager.Verify(m => m.UpdateShoppingListItemCheckoutStatus(slItem), Times.Once, "Expected method to be called once");
        }
        [Test()]
        public async Task InitShoppingListCheckout500Test()
        {
            // Arrange
            _shoppingListManager.Reset();
            _shoppingListManager.Setup(s => s.InitShoppingListCheckout(It.IsAny<List<ShoppingListTableValueParameter>>()))
                .Throws(_expectedException);
            var sut = new ShoppingListController(_shoppingListManager.Object);

            // Act
            var response = await sut.InitShoppingListCheckout(_tvpItems);
            var statusCodeResult = response as StatusCodeResult;

            // Assert
            Assert.IsNotNull(statusCodeResult, "statusCodeResult != null");
            Assert.IsTrue(statusCodeResult.StatusCode == 500);
            _shoppingListManager.Verify(m => m.InitShoppingListCheckout(_tvpItems), Times.Once, "Expected method to be called once");
        }
        [Test()]
        public async Task InitShoppingListCheckoutOKResultTest()
        {
            // Arrange
            _shoppingListManager.Reset();
            var requestResponse = new RequestResponse<List<ShoppingListTableValueParameter>>
            {
                Item = new List<ShoppingListTableValueParameter>(),
                PagedCollection = new PagedCollection(),
                Success = true
            };
            _shoppingListManager.Setup(s => s.InitShoppingListCheckout(It.IsAny<List<ShoppingListTableValueParameter>>()))
                .Returns(Task.FromResult(requestResponse));
            var sut = new ShoppingListController(_shoppingListManager.Object);

            // Act
            var response = await sut.InitShoppingListCheckout(_tvpItems);
            var okObjectResult = response as OkObjectResult;
            var responseModel = okObjectResult.Value as RequestResponse<List<ShoppingListTableValueParameter>>;

            // Assert
            Assert.IsNotNull(response, "response != null");
            Assert.IsNotNull(responseModel, "responseModel != null");
            Assert.IsNotNull(responseModel.Item, "responseModel.Item != null");
            _shoppingListManager.Verify(m => m.InitShoppingListCheckout(_tvpItems), Times.Once, "Expected method to be called once");
        }
    }
}