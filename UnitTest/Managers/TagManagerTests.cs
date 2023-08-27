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
    public class TagManagerTests
    {
        private Mock<ITagRepository> _mockTagRepository;
        private Mock<ILogger> _mockLogger;
        private RequestResponse<List<Tag>> _allTags;
        private RequestResponse<List<Tag>> _tagCollection;
        private RequestResponse<Tag> _upsertTag;
        private NullReferenceException _expectedException;


        [ExcludeFromCodeCoverage, OneTimeSetUp]
        public void TestSetup()
        {
            _expectedException = new NullReferenceException("Test Went Boom!");
            _mockTagRepository = new Mock<ITagRepository>();

            var testTags = new List<Tag>
            {
                new Tag { AttributeCount=1,CreateDate = DateTime.Now, Id = 123, IsActive=true, Name="One", SubscriptionId=333, UpdateDate=DateTime.Now },
                new Tag { AttributeCount=1,CreateDate = DateTime.Now, Id = 333, IsActive=true, Name="Two", SubscriptionId=333, UpdateDate=DateTime.Now }
            };

            _allTags = new RequestResponse<List<Tag>>();
            _allTags.Item = testTags;
            _allTags.Success = true;
            _tagCollection = new RequestResponse<List<Tag>>();
            _tagCollection.Success = true;
            _tagCollection.PagedCollection = new PagedCollection { CollectionTotal = 55, PageNumber = 1, PageSize = 5 };
            _upsertTag = new RequestResponse<Tag>();
            _upsertTag.Success = true;
            _upsertTag.Item = testTags[0];
            _mockLogger = new Mock<ILogger>();
        }

        [Test()]
        public async Task GetTagCollectionSuccessTest()
        {
            // Arrange
            _mockTagRepository.Reset();
            _mockTagRepository.Setup(t=> t.GetTagCollection(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(_allTags));
            var sut = new TagManager(_mockTagRepository.Object);

            // Act
            var response = await sut.GetTagCollection(123, 1, 10);

            // Assert
            _mockTagRepository.Verify(r => r.GetTagCollection(123, 1, 10), Times.Once, "Expected method to be called once");
            Assert.IsTrue(response.Success, "Expected response.Success to be true");
        }
        [Test()]
        public async Task GetTagCollectionFailTest()
        {
            // Arrange
            _mockTagRepository.Reset();
            _mockTagRepository.Setup(t=> t.GetTagCollection(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Throws(_expectedException);
            var sut = new TagManager(_mockTagRepository.Object);

            // Act
            var response = await sut.GetTagCollection(123, 1, 10);

            // Assert
            _mockTagRepository.Verify(r => r.GetTagCollection(123, 1, 10), Times.Once, "Expected method to be called once");
            Assert.IsTrue(!response.Success, "Expected response.Success to be false");
        }

        [Test()]
        public async Task GetTagsSuccessTest()
        {
            // Arrange
            _mockTagRepository.Reset();
            _mockTagRepository.Setup(t=> t.GetTags(It.IsAny<int>())).Returns(Task.FromResult(_allTags));
            var sut = new TagManager(_mockTagRepository.Object);

            // Act
            var response = await sut.GetTags(123);

            // Assert
            _mockTagRepository.Verify(r => r.GetTags(123), Times.Once, "Expected method to be called once");
            Assert.IsTrue(response.Success, "Expected response.Success to be true");
        }

        [Test()]
        public async Task GetTagsFailTest()
        {
            // Arrange
            _mockTagRepository.Reset();
            _mockTagRepository.Setup(t=> t.GetTags(It.IsAny<int>())).Throws(_expectedException);
            var sut = new TagManager(_mockTagRepository.Object);

            // Act
            var response = await sut.GetTags(123);

            // Assert
            _mockTagRepository.Verify(r => r.GetTags(123), Times.Once, "Expected method to be called once");
            Assert.IsTrue(!response.Success, "Expected response.Success to be false");
        }

        [Test()]
        public async Task InsertTagSuccessTest()
        {
            // Arrange
            _mockTagRepository.Reset();
            var insertGroupId = 55567;
            var updateDateValue = DateTime.Now;
            var minDateValue = DateTime.Now.AddMilliseconds(-1);
            var submitTag = new Tag { AttributeCount = 1, CreateDate = DateTime.Now, Id = 0, IsActive = true, Name = "Unit Test", SubscriptionId = 667, UpdateDate = updateDateValue };
            _mockTagRepository.Setup(t=> t.UpsertTag(It.IsAny<Tag>())).Returns(Task.FromResult(insertGroupId));
            var sut = new TagManager(_mockTagRepository.Object);

            // Act
            var response = await sut.UpsertTag(submitTag);

            // Assert
            _mockTagRepository.Verify(r => r.UpsertTag(submitTag), Times.Once, "Expected method to be called once");
            Assert.IsTrue(response.Success, "Expected response.Success to be true");
            Assert.IsTrue(response.Item.Id == insertGroupId, "Expected group ID to be @@IDENTITY returned fromDB Repository to be true");
            Assert.IsTrue(response.Item.UpdateDate > minDateValue, "Expected group UpdateDate > than test value to be true");
            Assert.IsTrue(response.Item.CreateDate > minDateValue, "Expected group CreateDate > than test value to be true");
        }

        [Test()]
        public async Task InsertGroupFailTest()
        {
            // Arrange
            _mockTagRepository.Reset();
            var submitTag = new Tag { AttributeCount = 1, CreateDate = DateTime.Now, Id = 0, IsActive = true, Name = "Unit Test", SubscriptionId = 667, UpdateDate = DateTime.Now };
            _mockTagRepository.Setup(t=> t.UpsertTag(It.IsAny<Tag>())).Throws(_expectedException);
            var sut = new TagManager(_mockTagRepository.Object);

            // Act
            var response = await sut.UpsertTag(submitTag);

            // Assert
            _mockTagRepository.Verify(r => r.UpsertTag(submitTag), Times.Once, "Expected method to be called once");
            Assert.IsTrue(!response.Success, "Expected response.Success to be false");
        }

        [Test()]
        public async Task UpdateTagSuccessTest()
        {
            // Arrange
            _mockTagRepository.Reset();
            var updateGroupId = 55567;
            var updateDateValue = DateTime.Now;
            var minDateValue = DateTime.Now.AddMilliseconds(-1);
            var createDateValue = DateTime.Now.AddMonths(-1);
            var submitTag = new Tag { AttributeCount = 1, CreateDate = createDateValue, Id = updateGroupId, IsActive = true, Name = "Unit Test", SubscriptionId = 667, UpdateDate = updateDateValue };
            _mockTagRepository.Setup(t=> t.UpsertTag(It.IsAny<Tag>())).Returns(Task.FromResult(updateGroupId));
            var sut = new TagManager(_mockTagRepository.Object);

            // Act
            var response = await sut.UpsertTag(submitTag);

            // Assert
            _mockTagRepository.Verify(r => r.UpsertTag(submitTag), Times.Once, "Expected method to be called once");
            Assert.IsTrue(response.Success, "Expected response.Success to be true");
            Assert.IsTrue(response.Item.Id == updateGroupId, "Expected group ID equal to object ID passed in to be true");
            Assert.IsTrue(response.Item.UpdateDate > minDateValue, "Expected group UpdateDate > than test value to be true");
            Assert.IsTrue(response.Item.CreateDate == createDateValue, "Expected group CreateDate unchanged to be true");
        }

        [Test()]
        public async Task UpdateGroupFailTest()
        {
            // Arrange
            _mockTagRepository.Reset();
            var updateGroupId = 55567;
            var submitTag = new Tag { AttributeCount = 1, CreateDate = DateTime.Now, Id = updateGroupId, IsActive = true, Name = "Unit Test", SubscriptionId = 667, UpdateDate = DateTime.Now };
            _mockTagRepository.Setup(t=> t.UpsertTag(It.IsAny<Tag>())).Throws(_expectedException);
            var sut = new TagManager(_mockTagRepository.Object);

            // Act
            var response = await sut.UpsertTag(submitTag);

            // Assert
            _mockTagRepository.Verify(r => r.UpsertTag(submitTag), Times.Once, "Expected method to be called once");
            Assert.IsTrue(!response.Success, "Expected response.Success to be false");
        }
    }
}
