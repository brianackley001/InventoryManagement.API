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
    public class ItemGroupControllerTests
    {
        private Mock<IItemGroupManager> _itemGroupManager;
        private Mock<ILogger> _mockLogger;
        private RequestResponse<List<ItemGroup>> _allItemGroups;
        private RequestResponse<List<ItemGroup>> _itemGroupCollection;
        private RequestResponse<bool> _UpsertItemGroup;
        private NullReferenceException _expectedException;

        [ExcludeFromCodeCoverage, OneTimeSetUp]
        public void TestSetup()
        {
            _expectedException = new NullReferenceException("Test Went Boom!");
            _itemGroupManager = new Mock<IItemGroupManager>();
            var testItemGroups = new List<ItemGroup>
            {
                new ItemGroup {CreateDate = DateTime.Now, Id = 123, IsActive=true, UpdateDate=DateTime.Now, ItemId=234, GroupId=99998 },
                new ItemGroup {CreateDate = DateTime.Now, Id = 333, IsActive=true,  UpdateDate=DateTime.Now, ItemId=787897, GroupId=992228 }
            };

            _allItemGroups = new RequestResponse<List<ItemGroup>>();
            _allItemGroups.Item = testItemGroups;
            _allItemGroups.Success = true;
            _itemGroupCollection = new RequestResponse<List<ItemGroup>>();
            _itemGroupCollection.Success = true;
            _itemGroupCollection.PagedCollection = new PagedCollection { CollectionTotal = 55, PageNumber = 1, PageSize = 5 };
            _UpsertItemGroup = new RequestResponse<bool>();
            _UpsertItemGroup.Success = true;
            _UpsertItemGroup.Item = true;
            _mockLogger = new Mock<ILogger>();
        }
        [Test()]
        public async Task GetGroupsOkResultTest()
        {
            // Arrange
            _itemGroupManager.Reset();
            _itemGroupManager.Setup(g => g.GetItemGroups(It.IsAny<int>())).Returns(Task.FromResult(_allItemGroups));
            var sut = new ItemGroupController(_itemGroupManager.Object);

            // Act
            var response = await sut.GetItemGroups(123);
            var okObjectResult = response as OkObjectResult;
            var responseModel = okObjectResult.Value as RequestResponse<List<ItemGroup>>;

            // Assert
            Assert.IsNotNull(response, "response != null");
            Assert.IsNotNull(responseModel, "responseModel != null");
            Assert.IsTrue(responseModel.Item.Count > 0, "responseModel.Count > 0");
            _itemGroupManager.Verify(m => m.GetItemGroups(123), Times.Once, "Expected method to be called once");
        }
        [Test()]
        public async Task GetGroups500Test()
        {
            // Arrange
            _itemGroupManager.Reset();
            _itemGroupManager.Setup(g => g.GetItemGroups(It.IsAny<int>())).Throws(_expectedException);
            var sut = new ItemGroupController(_itemGroupManager.Object);

            // Act
            var response = await sut.GetItemGroups(123);
            var statusCodeResult = response as StatusCodeResult;

            // Assert
            Assert.IsNotNull(statusCodeResult, "statusCodeResult != null");
            Assert.IsTrue(statusCodeResult.StatusCode == 500);
            _itemGroupManager.Verify(m => m.GetItemGroups(123), Times.Once, "Expected method to be called once");
        }

        [Test()]
        public async Task UpsertItemGroupsOkResultTest()
        {
            // Arrange
            _itemGroupManager.Reset();
            var submitItemGroup = new List<ItemAttributeTableValueParameter> { 
                new ItemAttributeTableValueParameter { IsActive = true, ItemId = 787897, AttributeId = 992228 }, 
                new ItemAttributeTableValueParameter { IsActive = true, ItemId = 78782, AttributeId = 9922821 } };
            _itemGroupManager.Setup(g => g.UpsertItemGroups(It.IsAny<List<ItemAttributeTableValueParameter>>())).Returns(Task.FromResult(_UpsertItemGroup));
            var sut = new ItemGroupController(_itemGroupManager.Object);

            // Act
            var response = await sut.UpsertItemGroups(submitItemGroup);
            var okObjectResult = response as OkObjectResult;
            var responseModel = okObjectResult.Value as RequestResponse<bool>;

            // Assert
            Assert.IsNotNull(response, "response != null");
            Assert.IsNotNull(responseModel, "responseModel != null");
            _itemGroupManager.Verify(m => m.UpsertItemGroups(submitItemGroup), Times.Once, "Expected method to be called once");
        }

        [Test()]
        public async Task UpsertItemGroup500Test()
        {
            // Arrange
            _itemGroupManager.Reset();
            var submitItemGroup = new List<ItemAttributeTableValueParameter> {
                new ItemAttributeTableValueParameter { IsActive = true, ItemId = 787897, AttributeId = 992228 },
                new ItemAttributeTableValueParameter { IsActive = true, ItemId = 78782, AttributeId = 9922821 } };
            _itemGroupManager.Setup(g => g.UpsertItemGroups(It.IsAny<List<ItemAttributeTableValueParameter>>())).Throws(_expectedException);
            var sut = new ItemGroupController(_itemGroupManager.Object);

            // Act
            var response = await sut.UpsertItemGroups(submitItemGroup);
            var statusCodeResult = response as StatusCodeResult;

            // Assert
            Assert.IsNotNull(statusCodeResult, "statusCodeResult != null");
            Assert.IsTrue(statusCodeResult.StatusCode == 500);
            _itemGroupManager.Verify(m => m.UpsertItemGroups(submitItemGroup), Times.Once, "Expected method to be called once");
        }
    }
}