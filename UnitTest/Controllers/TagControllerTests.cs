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
    [TestFixture]
    [Category("ControllerTests")]
    class TagControllerTests
    {
        private Mock<ITagManager> _tagsManager;
        private Mock<ILogger> _mockLogger;
        private RequestResponse<List<Tag>> _allTags;
        private RequestResponse<List<Tag>> _tagCollection;
        private RequestResponse<Tag> _upsertTag;
        private NullReferenceException _expectedException;

        [ExcludeFromCodeCoverage, OneTimeSetUp]
        public void TestSetup()
        {
            _expectedException = new NullReferenceException("Test Went Boom!");
            _tagsManager = new Mock<ITagManager>();
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
        public async Task GetTagsOkResultTest()
        {
            // Arrange
            _tagsManager.Reset();
            _tagsManager.Setup(t=> t.GetTags(It.IsAny<int>())).Returns(Task.FromResult(_allTags));
            var sut = new TagController(_tagsManager.Object);

            // Act
            var response = await sut.GetTags(123);
            var okObjectResult = response as OkObjectResult;
            var responseModel = okObjectResult.Value as RequestResponse<List<Tag>>;

            // Assert
            Assert.IsNotNull(response, "response != null");
            Assert.IsNotNull(responseModel, "responseModel != null");
            Assert.IsTrue(responseModel.Item.Count > 0, "responseModel.Count > 0");
            _tagsManager.Verify(m => m.GetTags(123), Times.Once, "Expected method to be called once");
        }
        [Test()]
        public async Task GetTags500Test()
        {
            // Arrange
            _tagsManager.Reset();
            _tagsManager.Setup(t=> t.GetTags(It.IsAny<int>())).Throws(_expectedException);
            var sut = new TagController(_tagsManager.Object);

            // Act
            var response = await sut.GetTags(123);
            var statusCodeResult = response as StatusCodeResult;

            // Assert
            Assert.IsNotNull(statusCodeResult, "statusCodeResult != null");
            Assert.IsTrue(statusCodeResult.StatusCode == 500);
            _tagsManager.Verify(m => m.GetTags(123), Times.Once, "Expected method to be called once");
        }

        [Test()]
        public async Task GetGroupCollectionOkResultTest()
        {
            // Arrange
            _tagsManager.Reset();
            _tagsManager.Setup(t=> t.GetTagCollection(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.FromResult(_allTags));
            var sut = new TagController(_tagsManager.Object);

            // Act
            var response = await sut.GetTagCollection(123, 1, 10);
            var okObjectResult = response as OkObjectResult;
            var responseModel = okObjectResult.Value as RequestResponse<List<Tag>>;

            // Assert
            Assert.IsNotNull(response, "response != null");
            Assert.IsNotNull(responseModel, "responseModel != null");
            Assert.IsTrue(responseModel.Item.Count > 0, "responseModel.Count > 0");
            _tagsManager.Verify(m => m.GetTagCollection(123, 1, 10), Times.Once, "Expected method to be called once");
        }
        [Test()]
        public async Task GetGroupCollection500Test()
        {
            // Arrange
            _tagsManager.Reset();
            _tagsManager.Setup(t=> t.GetTagCollection(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Throws(_expectedException);
            var sut = new TagController(_tagsManager.Object);

            // Act
            var response = await sut.GetTagCollection(123, 1, 10);
            var statusCodeResult = response as StatusCodeResult;

            // Assert
            Assert.IsNotNull(statusCodeResult, "statusCodeResult != null");
            Assert.IsTrue(statusCodeResult.StatusCode == 500);
            _tagsManager.Verify(m => m.GetTagCollection(123, 1, 10), Times.Once, "Expected method to be called once");
        }

        [Test()]
        public async Task UpsertTagOkResultTest()
        {
            // Arrange
            _tagsManager.Reset();
            var submitTag = new Tag { AttributeCount = 1, CreateDate = DateTime.Now, Id = 0, IsActive = true, Name = "Unit Test", SubscriptionId = 667, UpdateDate = DateTime.Now };
            _tagsManager.Setup(t=> t.UpsertTag(It.IsAny<Tag>())).Returns(Task.FromResult(_upsertTag));
            var sut = new TagController(_tagsManager.Object);

            // Act
            var response = await sut.UpsertTag(submitTag);
            var okObjectResult = response as OkObjectResult;
            var responseModel = okObjectResult.Value as RequestResponse<Tag>;

            // Assert
            Assert.IsNotNull(response, "response != null");
            Assert.IsNotNull(responseModel, "responseModel != null");
            _tagsManager.Verify(m => m.UpsertTag(submitTag), Times.Once, "Expected method to be called once");
        }

        [Test()]
        public async Task UpsertTag500Test()
        {
            // Arrange
            _tagsManager.Reset();
            var submitTag = new Tag { AttributeCount = 1, CreateDate = DateTime.Now, Id = 0, IsActive = true, Name = "Unit Test", SubscriptionId = 667, UpdateDate = DateTime.Now };
            _tagsManager.Setup(t=> t.UpsertTag(It.IsAny<Tag>())).Throws(_expectedException);
            var sut = new TagController(_tagsManager.Object);

            // Act
            var response = await sut.UpsertTag(submitTag);
            var statusCodeResult = response as StatusCodeResult;

            // Assert
            Assert.IsNotNull(statusCodeResult, "statusCodeResult != null");
            Assert.IsTrue(statusCodeResult.StatusCode == 500);
            _tagsManager.Verify(m => m.UpsertTag(submitTag), Times.Once, "Expected method to be called once");
        }
    }
}
