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
    public class GroupControllerTests
    {
        private Mock<IGroupManager> _groupsManager;
        private Mock<ILogger> _mockLogger;
        private RequestResponse<List<Group>> _allGroups;
        private RequestResponse<List<Group>> _groupCollection;
        private RequestResponse<Group> _upsertGroup;
        private NullReferenceException _expectedException;

        [ExcludeFromCodeCoverage, OneTimeSetUp]
        public void TestSetup()
        {
            _expectedException = new NullReferenceException("Test Went Boom!");
            _groupsManager = new Mock<IGroupManager>();
            var testGroups = new List<Group>
            {
                new Group { AttributeCount=1,CreateDate = DateTime.Now, Id = 123, IsActive=true, Name="One", SubscriptionId=333, UpdateDate=DateTime.Now },
                new Group { AttributeCount=1,CreateDate = DateTime.Now, Id = 333, IsActive=true, Name="Two", SubscriptionId=333, UpdateDate=DateTime.Now }
            };

            _allGroups = new RequestResponse<List<Group>>();
            _allGroups.Item = testGroups;
            _allGroups.Success = true;
            _groupCollection = new RequestResponse<List<Group>>();
            _groupCollection.Success = true;
            _groupCollection.PagedCollection = new PagedCollection { CollectionTotal = 55, PageNumber = 1, PageSize = 5 };
            _upsertGroup = new RequestResponse<Group>();
            _upsertGroup.Success = true;
            _upsertGroup.Item = testGroups[0];
            _mockLogger = new Mock<ILogger>();
        }
        [Test()]
        public async Task GetGroupsOkResultTest()
        {
            // Arrange
            _groupsManager.Reset();
            _groupsManager.Setup(g => g.GetGroups(It.IsAny<int>())).Returns(Task.FromResult(_allGroups));
            var sut = new GroupController(_groupsManager.Object);

            // Act
            var response = await sut.GetGroups(123);
            var okObjectResult = response as OkObjectResult;
            var responseModel = okObjectResult.Value as RequestResponse<List<Group>>;

            // Assert
            Assert.IsNotNull(response, "response != null");
            Assert.IsNotNull(responseModel, "responseModel != null");
            Assert.IsTrue(responseModel.Item.Count > 0, "responseModel.Count > 0");
            _groupsManager.Verify(m => m.GetGroups(123), Times.Once, "Expected method to be called once");
        }
        [Test()]
        public async Task GetGroups500Test()
        {
            // Arrange
            _groupsManager.Reset();
            _groupsManager.Setup(g => g.GetGroups(It.IsAny<int>())).Throws(_expectedException);
            var sut = new GroupController(_groupsManager.Object);

            // Act
            var response = await sut.GetGroups(123);
            var statusCodeResult = response as StatusCodeResult;

            // Assert
            Assert.IsNotNull(statusCodeResult, "statusCodeResult != null");
            Assert.IsTrue(statusCodeResult.StatusCode == 500);
            _groupsManager.Verify(m => m.GetGroups(123), Times.Once, "Expected method to be called once");
        }

        [Test()]
        public async Task GetGroupCollectionOkResultTest()
        {
            // Arrange
            _groupsManager.Reset();
            _groupsManager.Setup(g => g.GetGroupCollection(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(_allGroups));
            var sut = new GroupController(_groupsManager.Object);

            // Act
            var response = await sut.GetGroupCollection(123, 1, 10);
            var okObjectResult = response as OkObjectResult;
            var responseModel = okObjectResult.Value as RequestResponse<List<Group>>;

            // Assert
            Assert.IsNotNull(response, "response != null");
            Assert.IsNotNull(responseModel, "responseModel != null");
            Assert.IsTrue(responseModel.Item.Count > 0, "responseModel.Count > 0");
            _groupsManager.Verify(m => m.GetGroupCollection(123, 1, 10), Times.Once, "Expected method to be called once");
        }
        [Test()]
        public async Task GetGroupCollection500Test()
        {
            // Arrange
            _groupsManager.Reset();
            _groupsManager.Setup(g => g.GetGroupCollection(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Throws(_expectedException);
            var sut = new GroupController(_groupsManager.Object);

            // Act
            var response = await sut.GetGroupCollection(123, 1, 10);
            var statusCodeResult = response as StatusCodeResult;

            // Assert
            Assert.IsNotNull(statusCodeResult, "statusCodeResult != null");
            Assert.IsTrue(statusCodeResult.StatusCode == 500);
        }

        [Test()]
        public async Task UpsertGroupOkResultTest()
        {
            // Arrange
            _groupsManager.Reset();
            var submitGroup = new Group { AttributeCount = 1, CreateDate = DateTime.Now, Id = 0, IsActive = true, Name = "Unit Test", SubscriptionId = 667, UpdateDate = DateTime.Now };
            _groupsManager.Setup(g => g.UpsertGroup(It.IsAny<Group>())).Returns(Task.FromResult(_upsertGroup));
            var sut = new GroupController(_groupsManager.Object);

            // Act
            var response = await sut.UpsertGroup(submitGroup);
            var okObjectResult = response as OkObjectResult;
            var responseModel = okObjectResult.Value as RequestResponse<Group>;

            // Assert
            Assert.IsNotNull(response, "response != null");
            Assert.IsNotNull(responseModel, "responseModel != null");
            _groupsManager.Verify(m => m.UpsertGroup(submitGroup), Times.Once, "Expected method to be called once");
        }

        [Test()]
        public async Task UpsertGroup500Test()
        {
            // Arrange
            _groupsManager.Reset();
            var submitGroup = new Group { AttributeCount = 1, CreateDate = DateTime.Now, Id = 0, IsActive = true, Name = "Unit Test", SubscriptionId = 667, UpdateDate = DateTime.Now };
            _groupsManager.Setup(g => g.UpsertGroup(It.IsAny<Group>())).Throws(_expectedException);
            var sut = new GroupController(_groupsManager.Object);

            // Act
            var response = await sut.UpsertGroup(submitGroup);
            var statusCodeResult = response as StatusCodeResult;

            // Assert
            Assert.IsNotNull(statusCodeResult, "statusCodeResult != null");
            Assert.IsTrue(statusCodeResult.StatusCode == 500);
            _groupsManager.Verify(m => m.UpsertGroup(submitGroup), Times.Once, "Expected method to be called once");
        }
    }
}