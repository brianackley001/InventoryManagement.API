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
    public class ItemTagControllerTests
    {
        private Mock<IItemTagManager> _itemTagManager;
        private Mock<ILogger> _mockLogger;
        private RequestResponse<List<ItemTag>> _allItemTags;
        private RequestResponse<List<ItemTag>> _itemTagCollection;
        private RequestResponse<bool> _upsertItemTag;
        private NullReferenceException _expectedException;

        [ExcludeFromCodeCoverage, OneTimeSetUp]
        public void TestSetup()
        {
            _expectedException = new NullReferenceException("Test Went Boom!");
            _itemTagManager = new Mock<IItemTagManager>();
            var testItemTags = new List<ItemTag>
            {
                new ItemTag {CreateDate = DateTime.Now, Id = 123, IsActive=true, UpdateDate=DateTime.Now, ItemId=234, TagId=99998 },
                new ItemTag {CreateDate = DateTime.Now, Id = 333, IsActive=true,  UpdateDate=DateTime.Now, ItemId=787897, TagId=992228 }
            };

            _allItemTags = new RequestResponse<List<ItemTag>>();
            _allItemTags.Item = testItemTags;
            _allItemTags.Success = true;
            _itemTagCollection = new RequestResponse<List<ItemTag>>();
            _itemTagCollection.Success = true;
            _itemTagCollection.PagedCollection = new PagedCollection { CollectionTotal = 55, PageNumber = 1, PageSize = 5 };
            _upsertItemTag = new RequestResponse<bool>();
            _upsertItemTag.Success = true;
            _upsertItemTag.Item = true;
            _mockLogger = new Mock<ILogger>();
        }
        [Test()]
        public async Task GetItemTagsOkResultTest()
        {
            // Arrange
            _itemTagManager.Reset();
            _itemTagManager.Setup(g => g.GetItemTags(It.IsAny<int>())).Returns(Task.FromResult(_allItemTags));
            var sut = new ItemTagController(_itemTagManager.Object);

            // Act
            var response = await sut.GetItemTags(123);
            var okObjectResult = response as OkObjectResult;
            var responseModel = okObjectResult.Value as RequestResponse<List<ItemTag>>;

            // Assert
            Assert.IsNotNull(response, "response != null");
            Assert.IsNotNull(responseModel, "responseModel != null");
            Assert.IsTrue(responseModel.Item.Count > 0, "responseModel.Count > 0");
            _itemTagManager.Verify(m => m.GetItemTags(123), Times.Once, "Expected method to be called once");
        }
        [Test()]
        public async Task GetItemTags500Test()
        {
            // Arrange
            _itemTagManager.Reset();
            _itemTagManager.Setup(g => g.GetItemTags(It.IsAny<int>())).Throws(_expectedException);
            var sut = new ItemTagController(_itemTagManager.Object);

            // Act
            var response = await sut.GetItemTags(123);
            var statusCodeResult = response as StatusCodeResult;

            // Assert
            Assert.IsNotNull(statusCodeResult, "statusCodeResult != null");
            Assert.IsTrue(statusCodeResult.StatusCode == 500);
            _itemTagManager.Verify(m => m.GetItemTags(123), Times.Once, "Expected method to be called once");
        }

        [Test()]
        public async Task UpsertItemTagsOkResultTest()
        {
            // Arrange
            _itemTagManager.Reset();
            var submitItemTag = new List<ItemAttributeTableValueParameter> {
                new ItemAttributeTableValueParameter { IsActive = true, ItemId = 787897, AttributeId = 992228 },
                new ItemAttributeTableValueParameter { IsActive = true, ItemId = 78782, AttributeId = 9922821 } };
            _itemTagManager.Setup(g => g.UpsertItemTags(It.IsAny<List<ItemAttributeTableValueParameter>>())).Returns(Task.FromResult(_upsertItemTag));
            var sut = new ItemTagController(_itemTagManager.Object);

            // Act
            var response = await sut.UpsertItemTags(submitItemTag);
            var okObjectResult = response as OkObjectResult;
            var responseModel = okObjectResult.Value as RequestResponse<bool>;

            // Assert
            Assert.IsNotNull(response, "response != null");
            Assert.IsNotNull(responseModel, "responseModel != null");
            _itemTagManager.Verify(m => m.UpsertItemTags(submitItemTag), Times.Once, "Expected method to be called once");
        }

        [Test()]
        public async Task UpsertItemTags500Test()
        {
            // Arrange
            _itemTagManager.Reset();
            var submitItemTag = new List<ItemAttributeTableValueParameter> {
                new ItemAttributeTableValueParameter { IsActive = true, ItemId = 787897, AttributeId = 992228 },
                new ItemAttributeTableValueParameter { IsActive = true, ItemId = 78782, AttributeId = 9922821 } };
            _itemTagManager.Setup(g => g.UpsertItemTags(It.IsAny<List<ItemAttributeTableValueParameter>>())).Throws(_expectedException);
            var sut = new ItemTagController(_itemTagManager.Object);

            // Act
            var response = await sut.UpsertItemTags(submitItemTag);
            var statusCodeResult = response as StatusCodeResult;

            // Assert
            Assert.IsNotNull(statusCodeResult, "statusCodeResult != null");
            Assert.IsTrue(statusCodeResult.StatusCode == 500);
            _itemTagManager.Verify(m => m.UpsertItemTags(submitItemTag), Times.Once, "Expected method to be called once");
        }
    }
}