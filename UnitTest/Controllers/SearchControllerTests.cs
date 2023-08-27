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
    public class SearchControllerTests
    {
        private Mock<ISearchManager> _searchManager;
        private Mock<ILogger> _mockLogger;
        private RequestResponse<List<SearchResult>> _allSearchResults;
        private RequestResponse<List<SearchResult>> _resultsCollection;
        private List<Group> _itemGroups;
        private List<Tag> _itemTags;
        private SearchRequest _searchRequest;
        private NullReferenceException _expectedException;

        [ExcludeFromCodeCoverage, OneTimeSetUp]
        public void TestSetup()
        {
            _expectedException = new NullReferenceException("Test Went Boom!");
            _searchManager = new Mock<ISearchManager>();
            _itemGroups = new List<Group>
            {
                new Group{ Id=1, SubscriptionId=1, Name="test1", AttributeCount=0, IsActive= true, CreateDate = DateTime.Now.AddDays(-14), UpdateDate = DateTime.Now.AddDays(-14)},
                new Group{ Id=2, SubscriptionId=1, Name="test2", AttributeCount=0, IsActive= true, CreateDate = DateTime.Now.AddDays(-12), UpdateDate = DateTime.Now.AddDays(-11)},
                new Group{ Id=3, SubscriptionId=1, Name="test2", AttributeCount=0, IsActive= true, CreateDate = DateTime.Now.AddDays(-11), UpdateDate = DateTime.Now.AddDays(-10)}
            };
            _itemTags = new List<Tag>
            {
                new Tag{ Id=1, SubscriptionId=1, Name="test11", AttributeCount=0, IsActive= true, CreateDate = DateTime.Now.AddDays(-14), UpdateDate = DateTime.Now.AddDays(-14)},
                new Tag{ Id=2, SubscriptionId=1, Name="test21", AttributeCount=0, IsActive= true, CreateDate = DateTime.Now.AddDays(-12), UpdateDate = DateTime.Now.AddDays(-11)},
                new Tag{ Id=3, SubscriptionId=1, Name="test21", AttributeCount=0, IsActive= true, CreateDate = DateTime.Now.AddDays(-11), UpdateDate = DateTime.Now.AddDays(-10)}
            };

            var searchResultItems = new List<SearchResult>
            {
                new SearchResult { Id=123, Description="Description1", Name="ResultName 1", ResultType="item", ResultId=1, ResultWeight=1, Groups=_itemGroups, Tags=_itemTags },
                new SearchResult { Id=456, Description="Description2", Name="ResultName 2", ResultType="item", ResultId=1, ResultWeight=1, Groups=_itemGroups, Tags=_itemTags }
            };

            _searchRequest = new SearchRequest
            {
                IdCollection = new List<int> { 1, 2, 3 },
                PagedCollection = new PagedCollection { CollectionTotal = 20, PageNumber = 1, PageSize = 2 },
                SearchTerm = "Search X",
                SearchType = "item"
            };

            _allSearchResults = new RequestResponse<List<SearchResult>>
            {
                Item = searchResultItems,
                Success = true
            };
            _resultsCollection = new RequestResponse<List<SearchResult>>
            {
                Success = true,
                PagedCollection = new PagedCollection { CollectionTotal = 55, PageNumber = 1, PageSize = 5 }
            };
            _mockLogger = new Mock<ILogger>();
        }
        [Test()]
        public async Task GetItemsByGroupCollectionResultOkTest()
        {
            // Arrange
            _searchManager.Reset();
            _searchManager.Setup(s => s.GetItemsByGroupCollection(It.IsAny<int>(), It.IsAny<SearchRequest>())).Returns(Task.FromResult(_allSearchResults));
            var sut = new SearchController(_searchManager.Object);

            // Act
            var response = await sut.GetItemsByGroupCollection(1, _searchRequest);
            var okObjectResult = response as OkObjectResult;
            var responseModel = okObjectResult.Value as RequestResponse<List<SearchResult>>;

            // Assert
            Assert.IsNotNull(response, "response != null");
            Assert.IsNotNull(responseModel, "responseModel != null");
            Assert.IsTrue(responseModel.Item.Count > 0, "responseModel.Count > 0");
            _searchManager.Verify(m => m.GetItemsByGroupCollection(It.IsAny<int>(), It.IsAny<SearchRequest>()), Times.Once, "Expected method to be called once");
        }
        [Test()]
        public async Task GetItemsByGroupCollection500Test()
        {
            // Arrange
            _searchManager.Reset();
            _searchManager.Setup(s => s.GetItemsByGroupCollection(It.IsAny<int>(), It.IsAny<SearchRequest>())).Throws(_expectedException);
            var sut = new SearchController(_searchManager.Object);

            // Act
            var response = await sut.GetItemsByGroupCollection(1, _searchRequest);
            var statusCodeResult = response as StatusCodeResult;

            // Assert
            Assert.IsNotNull(statusCodeResult, "statusCodeResult != null");
            Assert.IsTrue(statusCodeResult.StatusCode == 500);
            _searchManager.Verify(m => m.GetItemsByGroupCollection(It.IsAny<int>(), It.IsAny<SearchRequest>()), Times.Once, "Expected method to be called once");
        }
        [Test()]
        public async Task GetItemsByTagCollectionResultOkTest()
        {
            // Arrange
            _searchManager.Reset();
            _searchManager.Setup(s => s.GetItemsByTagCollection(It.IsAny<int>(), It.IsAny<SearchRequest>())).Returns(Task.FromResult(_allSearchResults));
            var sut = new SearchController(_searchManager.Object);

            // Act
            var response = await sut.GetItemsByTagCollection(1, _searchRequest);
            var okObjectResult = response as OkObjectResult;
            var responseModel = okObjectResult.Value as RequestResponse<List<SearchResult>>;

            // Assert
            Assert.IsNotNull(response, "response != null");
            Assert.IsNotNull(responseModel, "responseModel != null");
            Assert.IsTrue(responseModel.Item.Count > 0, "responseModel.Count > 0");
            _searchManager.Verify(m => m.GetItemsByTagCollection(It.IsAny<int>(), It.IsAny<SearchRequest>()), Times.Once, "Expected method to be called once");
        }
        [Test()]
        public async Task GetItemsByTagCollection500Test()
        {
            // Arrange
            _searchManager.Reset();
            _searchManager.Setup(s => s.GetItemsByTagCollection(It.IsAny<int>(), It.IsAny<SearchRequest>())).Throws(_expectedException);
            var sut = new SearchController(_searchManager.Object);

            // Act
            var response = await sut.GetItemsByTagCollection(1, _searchRequest);
            var statusCodeResult = response as StatusCodeResult;

            // Assert
            Assert.IsNotNull(statusCodeResult, "statusCodeResult != null");
            Assert.IsTrue(statusCodeResult.StatusCode == 500);
            _searchManager.Verify(m => m.GetItemsByTagCollection(It.IsAny<int>(), It.IsAny<SearchRequest>()), Times.Once, "Expected method to be called once");
        }
        [Test()]
        public async Task GetSearchResultsOkTest()
        {
            // Arrange
            _searchManager.Reset();
            _searchManager.Setup(s => s.GetSearchResultCollection(It.IsAny<int>(), It.IsAny<SearchRequest>())).Returns(Task.FromResult(_allSearchResults));
            var sut = new SearchController(_searchManager.Object);

            // Act
            var response = await sut.GetSearchResultCollection(1, _searchRequest);
            var okObjectResult = response as OkObjectResult;
            var responseModel = okObjectResult.Value as RequestResponse<List<SearchResult>>;

            // Assert
            Assert.IsNotNull(response, "response != null");
            Assert.IsNotNull(responseModel, "responseModel != null");
            Assert.IsTrue(responseModel.Item.Count > 0, "responseModel.Count > 0");
            _searchManager.Verify(m => m.GetSearchResultCollection(It.IsAny<int>(), It.IsAny<SearchRequest>()), Times.Once, "Expected method to be called once");
        }
        [Test()]
        public async Task GetSearchResults500Test()
        {
            // Arrange
            _searchManager.Reset();
            _searchManager.Setup(s => s.GetSearchResultCollection(It.IsAny<int>(), It.IsAny<SearchRequest>())).Throws(_expectedException);
            var sut = new SearchController(_searchManager.Object);

            // Act
            var response = await sut.GetSearchResultCollection(1, _searchRequest);
            var statusCodeResult = response as StatusCodeResult;

            // Assert
            Assert.IsNotNull(statusCodeResult, "statusCodeResult != null");
            Assert.IsTrue(statusCodeResult.StatusCode == 500);
            _searchManager.Verify(m => m.GetSearchResultCollection(It.IsAny<int>(), It.IsAny<SearchRequest>()), Times.Once, "Expected method to be called once");
        }
    }
}