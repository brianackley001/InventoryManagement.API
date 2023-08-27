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
    public class ItemTagManagerTests
    {
        private Mock<IItemTagRepository> mockItemTagRepository;
        private Mock<ILogger> _mockLogger;
        private RequestResponse<List<ItemTag>> _allItemTags;
        private RequestResponse<List<ItemTag>> _itemTagCollection;
        private RequestResponse<ItemTag> _upsertItemTag;
        private NullReferenceException _expectedException;


        [ExcludeFromCodeCoverage, OneTimeSetUp]
        public void TestSetup()
        {
            _expectedException = new NullReferenceException("Test Went Boom!");
            mockItemTagRepository = new Mock<IItemTagRepository>();

            var testItemGroups = new List<ItemTag>
            {
                new ItemTag {CreateDate = DateTime.Now, Id = 123, IsActive=true, UpdateDate=DateTime.Now, ItemId=234, TagId=99998 },
                new ItemTag {CreateDate = DateTime.Now, Id = 333, IsActive=true,  UpdateDate=DateTime.Now, ItemId=787897, TagId=992228 }
            };

            _allItemTags = new RequestResponse<List<ItemTag>>();
            _allItemTags.Item = testItemGroups;
            _allItemTags.Success = true;
            _itemTagCollection = new RequestResponse<List<ItemTag>>();
            _itemTagCollection.Success = true;
            _itemTagCollection.PagedCollection = new PagedCollection { CollectionTotal = 55, PageNumber = 1, PageSize = 5 };
            _upsertItemTag = new RequestResponse<ItemTag>();
            _upsertItemTag.Success = true;
            _upsertItemTag.Item = testItemGroups[0];
            _mockLogger = new Mock<ILogger>();
        }

        [Test()]
        public async Task GetItemTagsSuccessTest()
        {
            // Arrange
            mockItemTagRepository.Reset();
            mockItemTagRepository.Setup(r => r.GetItemTags(It.IsAny<int>())).Returns(Task.FromResult(_allItemTags));
            var sut = new ItemTagManager(mockItemTagRepository.Object);

            // Act
            var response = await sut.GetItemTags(123);

            // Assert
            mockItemTagRepository.Verify(r => r.GetItemTags(123), Times.Once, "Expected method to be called once");
            Assert.IsTrue(response.Success, "Expected response.Success to be true");
        }

        [Test()]
        public async Task GetItemTagsFailTest()
        {
            // Arrange
            mockItemTagRepository.Reset();
            mockItemTagRepository.Setup(t => t.GetItemTags(It.IsAny<int>())).Throws(_expectedException);
            var sut = new ItemTagManager(mockItemTagRepository.Object);

            // Act
            var response = await sut.GetItemTags(123);

            // Assert
            mockItemTagRepository.Verify(r => r.GetItemTags(123), Times.Once, "Expected method to be called once");
            Assert.IsTrue(!response.Success, "Expected response.Success to be false");
        }

        [Test()]
        public async Task InsertItemTagspFailExceptionTest()
        {
            // Arrange
            mockItemTagRepository.Reset();
            var submitItemTag = new List<ItemAttributeTableValueParameter> {
                new ItemAttributeTableValueParameter { IsActive = true, ItemId = 787897, AttributeId = 992228 },
                new ItemAttributeTableValueParameter { IsActive = true, ItemId = 78782, AttributeId = 9922821 } };
            mockItemTagRepository.Setup(t => t.UpsertItemTags(It.IsAny<List<ItemAttributeTableValueParameter>>())).Throws(_expectedException);
            var sut = new ItemTagManager(mockItemTagRepository.Object);

            // Act
            var response = await sut.UpsertItemTags(submitItemTag);

            // Assert
            mockItemTagRepository.Verify(r => r.UpsertItemTags(submitItemTag), Times.Once, "Expected method to be called once");
            Assert.IsFalse(response.Success, "Expected response.Success to be false");
        }

        [Test()]
        public async Task UpdateItemTagsSuccessTest()
        {
            // Arrange
            mockItemTagRepository.Reset();
            var expectedResult = new RequestResponse<bool> { Item = true, Success = true, PagedCollection = new PagedCollection() };
            var submitItemTag = new List<ItemAttributeTableValueParameter> {
                new ItemAttributeTableValueParameter { IsActive = true, ItemId = 787897, AttributeId = 992228 },
                new ItemAttributeTableValueParameter { IsActive = true, ItemId = 78782, AttributeId = 9922821 } };
            mockItemTagRepository.Setup(t => t.UpsertItemTags(It.IsAny<List<ItemAttributeTableValueParameter>>())).Returns(Task.FromResult(expectedResult));
            var sut = new ItemTagManager(mockItemTagRepository.Object);

            // Act
            var response = await sut.UpsertItemTags(submitItemTag);

            // Assert
            mockItemTagRepository.Verify(r => r.UpsertItemTags(submitItemTag), Times.Once, "Expected method to be called once");
            Assert.IsTrue(response.Success, "Expected response.Success to be true");
            Assert.IsTrue(response.Item, "Expected item to be true");
        }
    }
}