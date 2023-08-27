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


namespace InventoryManagement.API.Controllers.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture()]
    [Category("ControllerTests")]
    public class ItemControllerTests
    {
        private Mock<IItemManager> _itemManager;
        private Mock<ILogger> _mockLogger;
        private RequestResponse<List<Item>> _allItems;
        private RequestResponse<List<Item>> _itemCollection;
        private RequestResponse<Item> _upsertItem;
        private NullReferenceException _expectedException;
        private SearchRequest _searchRequest;

        [ExcludeFromCodeCoverage, OneTimeSetUp]
        public void TestSetup()
        {
            _expectedException = new NullReferenceException("Test Went Boom!");
            _itemManager = new Mock<IItemManager>();
            var testItems = new List<Item>
            {
                new Item { CreateDate = DateTime.Now, Id = 123, IsActive=true, Name="One", SubscriptionId=333, UpdateDate=DateTime.Now, Description="Blaaaaaaaaaaaaaaaaaaaaaaaah:1" },
                new Item { CreateDate = DateTime.Now, Id = 333, IsActive=true, Name="Two", SubscriptionId=333, UpdateDate=DateTime.Now , Description="Blaaaaaaaaaaaaaaaaaaaaaaaah:2" }
            };

            _allItems = new RequestResponse<List<Item>>();
            _allItems.Item = testItems;
            _allItems.Success = true;
            _itemCollection = new RequestResponse<List<Item>>();
            _itemCollection.Success = true;
            _itemCollection.PagedCollection = new PagedCollection { CollectionTotal = 55, PageNumber = 1, PageSize = 5 };
            _upsertItem = new RequestResponse<Item>();
            _upsertItem.Success = true;
            _upsertItem.Item = testItems[0];
            _mockLogger = new Mock<ILogger>();
            _searchRequest = new SearchRequest{ PagedCollection = new PagedCollection { CollectionTotal = 20, PageSize = 10, PageNumber = 1 }, SortAscending = true, SortBy = "name", IdCollection = new List<int>() };
        }
        [Test()]
        public async Task GetItemOkResultTest()
        {
            // Arrange
            _itemManager.Reset();
            _itemManager.Setup(i => i.GetItem(It.IsAny<int>())).Returns(Task.FromResult(_upsertItem));
            var sut = new ItemController(_itemManager.Object);

            // Act
            var response = await sut.GetItem(123);
            var okObjectResult = response as OkObjectResult;
            var responseModel = okObjectResult.Value as RequestResponse<Item>;

            // Assert
            Assert.IsNotNull(response, "response != null");
            Assert.IsNotNull(responseModel, "responseModel != null");
            Assert.IsTrue(responseModel.Item != null, "responseModel.Item  != null");
            _itemManager.Verify(m => m.GetItem(123), Times.Once, "Expected method to be called once");
        }
        [Test()]
        public async Task GetItem500Test()
        {
            // Arrange
            _itemManager.Reset();
            _itemManager.Setup(i => i.GetItem(It.IsAny<int>())).Throws(_expectedException);
            var sut = new ItemController(_itemManager.Object);

            // Act
            var response = await sut.GetItem(123);
            var statusCodeResult = response as StatusCodeResult;

            // Assert
            Assert.IsNotNull(statusCodeResult, "statusCodeResult != null");
            Assert.IsTrue(statusCodeResult.StatusCode == 500);
            _itemManager.Verify(m => m.GetItem(123), Times.Once, "Expected method to be called once");
        }

        [Test()]
        public async Task GetItemCollectionOkResultTest()
        {
            // Arrange
            _itemManager.Reset();
            _itemManager.Setup(i => i.GetItemCollection(It.IsAny<int>(), It.IsAny<SearchRequest>())).Returns(Task.FromResult(_allItems));
            var sut = new ItemController(_itemManager.Object);

            // Act
            var response = await sut.GetItemCollection(123, _searchRequest);
            var okObjectResult = response as OkObjectResult;
            var responseModel = okObjectResult.Value as RequestResponse<List<Item>>;

            // Assert
            Assert.IsNotNull(response, "response != null");
            Assert.IsNotNull(responseModel, "responseModel != null");
            Assert.IsTrue(responseModel.Item.Count > 0, "responseModel.Count > 0");
            _itemManager.Verify(m => m.GetItemCollection(123, _searchRequest), Times.Once, "Expected method to be called once");
        }
        [Test()]
        public async Task GetItemCollection500Test()
        {
            // Arrange
            _itemManager.Reset();
            _itemManager.Setup(i => i.GetItemCollection(It.IsAny<int>(), It.IsAny<SearchRequest>())).Throws(_expectedException);
            var sut = new ItemController(_itemManager.Object);

            // Act
            var response = await sut.GetItemCollection(123, _searchRequest);
            var statusCodeResult = response as StatusCodeResult;

            // Assert
            Assert.IsNotNull(statusCodeResult, "statusCodeResult != null");
            Assert.IsTrue(statusCodeResult.StatusCode == 500);
            _itemManager.Verify(m => m.GetItemCollection(123, _searchRequest), Times.Once, "Expected method to be called once");
        }

        [Test()]
        public async Task UpsertItemOkResultTest()
        {
            // Arrange
            _itemManager.Reset();
            var submitItem = new Item { CreateDate = DateTime.Now, Id = 0, IsActive = true, Name = "Unit Test", SubscriptionId = 667, UpdateDate = DateTime.Now };
            _itemManager.Setup(i => i.UpsertItem(It.IsAny<Item>())).Returns(Task.FromResult(_upsertItem));
            _itemManager.Setup(i => i.GetItem(It.IsAny<int>())).Returns(Task.FromResult(_upsertItem));
            var sut = new ItemController(_itemManager.Object);

            // Act
            var response = await sut.UpsertItem(submitItem);
            var okObjectResult = response as OkObjectResult;
            var responseModel = okObjectResult.Value as RequestResponse<Item>;

            // Assert
            Assert.IsNotNull(response, "response != null");
            Assert.IsNotNull(responseModel, "responseModel != null");
            _itemManager.Verify(m => m.UpsertItem(submitItem), Times.Once, "Expected method to be called once");
        }

        [Test()]
        public async Task UpsertItem500Test()
        {
            // Arrange
            _itemManager.Reset();
            var submitItem = new Item { CreateDate = DateTime.Now, Id = 0, IsActive = true, Name = "Unit Test", SubscriptionId = 667, UpdateDate = DateTime.Now };
            _itemManager.Setup(i => i.UpsertItem(It.IsAny<Item>())).Throws(_expectedException);
            var sut = new ItemController(_itemManager.Object);

            // Act
            var response = await sut.UpsertItem(submitItem);
            var statusCodeResult = response as StatusCodeResult;

            // Assert
            Assert.IsNotNull(statusCodeResult, "statusCodeResult != null");
            Assert.IsTrue(statusCodeResult.StatusCode == 500);
            _itemManager.Verify(m => m.UpsertItem(submitItem), Times.Once, "Expected method to be called once");
        }


        [Test()]
        public async Task GetItemLowQuantityCollectionOkResultTest()
        {
            // Arrange
            _itemManager.Reset();
            _itemManager.Setup(i => i.GetLowQuantityItemCollection(It.IsAny<int>(), It.IsAny<SearchRequest>())).Returns(Task.FromResult(_allItems));
            var sut = new ItemController(_itemManager.Object);

            // Act
            var response = await sut.GetLowQuantityItemCollection(123, _searchRequest);
            var okObjectResult = response as OkObjectResult;
            var responseModel = okObjectResult.Value as RequestResponse<List<Item>>;

            // Assert
            Assert.IsNotNull(response, "response != null");
            Assert.IsNotNull(responseModel, "responseModel != null");
            Assert.IsTrue(responseModel.Item.Count > 0, "responseModel.Count > 0");
            _itemManager.Verify(m => m.GetLowQuantityItemCollection(123,  _searchRequest), Times.Once, "Expected method to be called once");
        }
        [Test()]
        public async Task GetItemLowQuantityCollection500Test()
        {
            // Arrange
            _itemManager.Reset();
            _itemManager.Setup(i => i.GetLowQuantityItemCollection(It.IsAny<int>(), It.IsAny<SearchRequest>())).Throws(_expectedException);
            var sut = new ItemController(_itemManager.Object);

            // Act
            var response = await sut.GetLowQuantityItemCollection(123, _searchRequest);
            var statusCodeResult = response as StatusCodeResult;

            // Assert
            Assert.IsNotNull(statusCodeResult, "statusCodeResult != null");
            Assert.IsTrue(statusCodeResult.StatusCode == 500);
            _itemManager.Verify(m => m.GetLowQuantityItemCollection(123, _searchRequest), Times.Once, "Expected method to be called once");
        }
    }
}