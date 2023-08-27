using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryManagement.API.DataProvider.SQL;
using Moq;
using System.Diagnostics.CodeAnalysis;
using InventoryManagement.API.Models;
using NLog;

namespace InventoryManagement.API.Managers.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture()]
    [Category("ManagerTests")]
    public class GroupManagerTests
    {
        private Mock<IGroupRepository> _mockGroupRepository;
        private Mock<ILogger> _mockLogger;
        private RequestResponse<List<Group>> _allGroups;
        private RequestResponse<List<Group>> _groupCollection;
        private RequestResponse<Group> _upsertGroup;
        private NullReferenceException _expectedException;


        [ExcludeFromCodeCoverage, OneTimeSetUp]
        public void TestSetup()
        {
            _expectedException = new NullReferenceException("Test Went Boom!");
            _mockGroupRepository = new Mock<IGroupRepository>();

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
        public async Task GetGroupCollectionSuccessTest()
        {
            // Arrange
            _mockGroupRepository.Reset();
            _mockGroupRepository.Setup(g => g.GetGroupCollection(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(_allGroups));
            var sut = new GroupManager(_mockGroupRepository.Object);

            // Act
            var response = await sut.GetGroupCollection(123, 1, 10);

            // Assert
            _mockGroupRepository.Verify(r => r.GetGroupCollection(123, 1, 10), Times.Once, "Expected method to be called once");
            Assert.IsTrue(response.Success, "Expected response.Success to be true");
        }
        [Test()]
        public async Task GetGroupCollectionFailTest()
        {
            // Arrange
            _mockGroupRepository.Reset();
            _mockGroupRepository.Setup(g => g.GetGroupCollection(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Throws(_expectedException);
            var sut = new GroupManager(_mockGroupRepository.Object);

            // Act
            var response = await sut.GetGroupCollection(123, 1, 10);

            // Assert
            _mockGroupRepository.Verify(r => r.GetGroupCollection(123, 1, 10), Times.Once, "Expected method to be called once");
            Assert.IsTrue(!response.Success, "Expected response.Success to be false");
        }

        [Test()]
        public async Task GetGroupsSuccessTest()
        {
            // Arrange
            _mockGroupRepository.Reset();
            _mockGroupRepository.Setup(g => g.GetGroups(It.IsAny<int>())).Returns(Task.FromResult(_allGroups));
            var sut = new GroupManager(_mockGroupRepository.Object);

            // Act
            var response = await sut.GetGroups(123);

            // Assert
            _mockGroupRepository.Verify(r => r.GetGroups(123), Times.Once, "Expected method to be called once");
            Assert.IsTrue(response.Success, "Expected response.Success to be true");
        }

        [Test()]
        public async Task GetGroupsFailTest()
        {
            // Arrange
            _mockGroupRepository.Reset();
            _mockGroupRepository.Setup(g => g.GetGroups(It.IsAny<int>())).Throws(_expectedException);
            var sut = new GroupManager(_mockGroupRepository.Object);

            // Act
            var response = await sut.GetGroups(123);

            // Assert
            _mockGroupRepository.Verify(r => r.GetGroups(123), Times.Once, "Expected method to be called once");
            Assert.IsTrue(!response.Success, "Expected response.Success to be false");
        }

        [Test()]
        public async Task InsertGroupSuccessTest()
        {
            // Arrange
            _mockGroupRepository.Reset();
            var insertGroupId = 55567;
            var updateDateValue = DateTime.Now;
            var minDateValue = DateTime.Now.AddMilliseconds(-1);
            var submitGroup = new Group { AttributeCount = 1, CreateDate = DateTime.Now, Id = 0, IsActive = true, Name = "Unit Test", SubscriptionId = 667, UpdateDate = updateDateValue };
            _mockGroupRepository.Setup(g => g.UpsertGroup(It.IsAny<Group>())).Returns(Task.FromResult(insertGroupId));
            var sut = new GroupManager(_mockGroupRepository.Object);

            // Act
            var response = await sut.UpsertGroup(submitGroup);

            // Assert
            _mockGroupRepository.Verify(r => r.UpsertGroup(submitGroup), Times.Once, "Expected method to be called once");
            Assert.IsTrue(response.Success, "Expected response.Success to be true");
            Assert.IsTrue(response.Item.Id == insertGroupId, "Expected group ID to be @@IDENTITY returned fromDB Repository to be true");
            Assert.IsTrue(response.Item.UpdateDate > minDateValue, "Expected group UpdateDate > than test value to be true");
            Assert.IsTrue(response.Item.CreateDate > minDateValue, "Expected group CreateDate > than test value to be true");
        }

        [Test()]
        public async Task InsertGroupFailTest()
        {
            // Arrange
            _mockGroupRepository.Reset();
            var submitGroup = new Group { AttributeCount = 1, CreateDate = DateTime.Now, Id = 0, IsActive = true, Name = "Unit Test", SubscriptionId = 667, UpdateDate = DateTime.Now };
            _mockGroupRepository.Setup(g => g.UpsertGroup(It.IsAny<Group>())).Throws(_expectedException);
            var sut = new GroupManager(_mockGroupRepository.Object);

            // Act
            var response = await sut.UpsertGroup(submitGroup);

            // Assert
            _mockGroupRepository.Verify(r => r.UpsertGroup(submitGroup), Times.Once, "Expected method to be called once");
            Assert.IsTrue(!response.Success, "Expected response.Success to be false");
        }

        [Test()]
        public async Task UpdateGroupSuccessTest()
        {
            // Arrange
            _mockGroupRepository.Reset();
            var updateGroupId = 55567;
            var updateDateValue = DateTime.Now;
            var minDateValue = DateTime.Now.AddMilliseconds(-1);
            var createDateValue = DateTime.Now.AddMonths(-1);
            var submitGroup = new Group { AttributeCount = 1, CreateDate = createDateValue, Id = updateGroupId, IsActive = true, Name = "Unit Test", SubscriptionId = 667, UpdateDate = updateDateValue };
            _mockGroupRepository.Setup(g => g.UpsertGroup(It.IsAny<Group>())).Returns(Task.FromResult(updateGroupId));
            var sut = new GroupManager(_mockGroupRepository.Object);

            // Act
            var response = await sut.UpsertGroup(submitGroup);

            // Assert
            _mockGroupRepository.Verify(r => r.UpsertGroup(submitGroup), Times.Once, "Expected method to be called once");
            Assert.IsTrue(response.Success, "Expected response.Success to be true");
            Assert.IsTrue(response.Item.Id == updateGroupId, "Expected group ID equal to object ID passed in to be true");
            Assert.IsTrue(response.Item.UpdateDate > minDateValue, "Expected group UpdateDate > than test value to be true");
            Assert.IsTrue(response.Item.CreateDate == createDateValue, "Expected group CreateDate unchanged to be true");
        }

        [Test()]
        public async Task UpdateGroupFailTest()
        {
            // Arrange
            _mockGroupRepository.Reset();
            var updateGroupId = 55567;
            var submitGroup = new Group { AttributeCount = 1, CreateDate = DateTime.Now, Id = updateGroupId, IsActive = true, Name = "Unit Test", SubscriptionId = 667, UpdateDate = DateTime.Now };
            _mockGroupRepository.Setup(g => g.UpsertGroup(It.IsAny<Group>())).Throws(_expectedException);
            var sut = new GroupManager(_mockGroupRepository.Object);

            // Act
            var response = await sut.UpsertGroup(submitGroup);

            // Assert
            _mockGroupRepository.Verify(r => r.UpsertGroup(submitGroup), Times.Once, "Expected method to be called once");
            Assert.IsTrue(!response.Success, "Expected response.Success to be false");
        }
    }
}