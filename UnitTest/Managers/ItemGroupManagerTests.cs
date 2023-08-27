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
    public class ItemGroupManagerTests
    {
        private Mock<IItemGroupRepository> _mockItemGroupRepository;
        private Mock<ILogger> _mockLogger;
        private RequestResponse<List<ItemGroup>> _allItemGroups;
        private RequestResponse<List<ItemGroup>> _itemGroupCollection;
        private RequestResponse<ItemGroup> _upsertItemGroup;
        private NullReferenceException _expectedException;


        [ExcludeFromCodeCoverage, OneTimeSetUp]
        public void TestSetup()
        {
            _expectedException = new NullReferenceException("Test Went Boom!");
            _mockItemGroupRepository = new Mock<IItemGroupRepository>();

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
            _upsertItemGroup = new RequestResponse<ItemGroup>();
            _upsertItemGroup.Success = true;
            _upsertItemGroup.Item = testItemGroups[0];
            _mockLogger = new Mock<ILogger>();
        }

        [Test()]
        public async Task GetItemGroupsSuccessTest()
        {
            // Arrange
            _mockItemGroupRepository.Reset();
            _mockItemGroupRepository.Setup(r=> r.GetItemGroups(It.IsAny<int>())).Returns(Task.FromResult(_allItemGroups));
            var sut = new ItemGroupManager(_mockItemGroupRepository.Object);

            // Act
            var response = await sut.GetItemGroups(123);

            // Assert
            _mockItemGroupRepository.Verify(r => r.GetItemGroups(123), Times.Once, "Expected method to be called once");
            Assert.IsTrue(response.Success, "Expected response.Success to be true");
        }

        [Test()]
        public async Task GetItemGroupsFailExceptionTest()
        {
            // Arrange
            _mockItemGroupRepository.Reset();
            _mockItemGroupRepository.Setup(t => t.GetItemGroups(It.IsAny<int>())).Throws(_expectedException);
            var sut = new ItemGroupManager(_mockItemGroupRepository.Object);

            // Act
            var response = await sut.GetItemGroups(123);

            // Assert
            _mockItemGroupRepository.Verify(r => r.GetItemGroups(123), Times.Once, "Expected method to be called once");
            Assert.IsTrue(!response.Success, "Expected response.Success to be false");
        }

        [Test()]
        public async Task UpsertItemGroupsFailExceptionTest()
        {
            // Arrange
            _mockItemGroupRepository.Reset();
            var submitItemGroup = new List<ItemAttributeTableValueParameter> {
                new ItemAttributeTableValueParameter { IsActive = true, ItemId = 787897, AttributeId = 992228 },
                new ItemAttributeTableValueParameter { IsActive = true, ItemId = 78782, AttributeId = 9922821 } };
            _mockItemGroupRepository.Setup(t => t.UpsertItemGroups(It.IsAny<List<ItemAttributeTableValueParameter>>())).Throws(_expectedException);
            var sut = new ItemGroupManager(_mockItemGroupRepository.Object);

            // Act
            var response = await sut.UpsertItemGroups(submitItemGroup);

            // Assert
            _mockItemGroupRepository.Verify(r => r.UpsertItemGroups(submitItemGroup), Times.Once, "Expected method to be called once");
            Assert.IsFalse(response.Success, "Expected response.Success to be false");
        }

        [Test()]
        public async Task UpsertItemGroupsSuccessTest()
        {
            // Arrange
            _mockItemGroupRepository.Reset();
            var expectedResult = new RequestResponse<bool> { Item=true, Success=true, PagedCollection = new PagedCollection()};
            var submitItemGroup = new List<ItemAttributeTableValueParameter> {
                new ItemAttributeTableValueParameter { IsActive = true, ItemId = 787897, AttributeId = 992228 },
                new ItemAttributeTableValueParameter { IsActive = true, ItemId = 78782, AttributeId = 9922821 } };
            _mockItemGroupRepository.Setup(t => t.UpsertItemGroups(It.IsAny<List<ItemAttributeTableValueParameter>>())).Returns(Task.FromResult(expectedResult));
            var sut = new ItemGroupManager(_mockItemGroupRepository.Object);

            // Act
            var response = await sut.UpsertItemGroups(submitItemGroup);

            // Assert
            _mockItemGroupRepository.Verify(r => r.UpsertItemGroups(submitItemGroup), Times.Once, "Expected method to be called once");
            Assert.IsTrue(response.Success, "Expected response.Success to be true");
            Assert.IsTrue(response.Item, "Expected itemGroup to be true");
        }
    }
}