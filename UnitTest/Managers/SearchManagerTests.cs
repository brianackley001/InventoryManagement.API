using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryManagement.API.DataProvider.SQL;
using Moq;
using System.Diagnostics.CodeAnalysis;
using InventoryManagement.API.Models;
using NLog;
using InventoryManagement.API.UnitTest.Managers.TestSetup;

namespace InventoryManagement.API.Managers.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture()]
    [Category("ManagerTests")]
    public class SearchManagerTests
    {
        private Mock<ISearchRepository> _mockSearchRepository;
        private Mock<ILogger> _mockLogger;
        private RequestResponse<ItemByAttributeSearchDataCollection> _itemsByGroupRequestResponse;
        private RequestResponse<ItemByAttributeSearchDataCollection> _itemsByTagSearchRequestResponse;
        private ItemByAttributeSearchDataCollection _itemsByGroupsCollection;
        private ItemByAttributeSearchDataCollection _itemsByTagsCollection;
        private PagedCollection _pagedCollection;
        private NullReferenceException _expectedException;
        private SearchRequest _searchRequest;


        [OneTimeSetUp]
        public void TestSetup()
        {
            _expectedException = new NullReferenceException("Test Went Boom!");
            _mockSearchRepository = new Mock<ISearchRepository>();
            _itemsByGroupsCollection = SearchManagerSetUp.GetItemsByGroup();
            _itemsByTagsCollection = SearchManagerSetUp.GetItemsByTag();
            _pagedCollection = new PagedCollection { CollectionTotal = 6, PageNumber = 1, PageSize = 3 };

            _itemsByGroupRequestResponse = new RequestResponse<ItemByAttributeSearchDataCollection>
            {
                Item = _itemsByGroupsCollection,
                PagedCollection = _pagedCollection
            };

            _itemsByTagSearchRequestResponse = new RequestResponse<ItemByAttributeSearchDataCollection>
            {
                Item = _itemsByTagsCollection,
                PagedCollection = _pagedCollection
            };
            _searchRequest = new SearchRequest { IdCollection = new List<int>(), SortAscending = true, SearchTerm = "searchTheTerm", SortBy = "name", PagedCollection = new PagedCollection { CollectionTotal = 25, PageNumber = 1, PageSize = 5 } };

            _mockLogger = new Mock<ILogger>();
        }

        [Test()]
        public async Task GetItemsByGroupCollectionResultSuccess()
        {
            // Arrange
            _mockSearchRepository.Reset();
            var idCollection = new List<int> { 20, 30, 40, 50 };
            _searchRequest.IdCollection = idCollection;
            _mockSearchRepository.Setup(r => r.GetItemsByGroupCollection(It.IsAny<int>(), It.IsAny<SearchRequest>()))
                .Returns(Task.FromResult(_itemsByGroupRequestResponse));
            var sut = new SearchManager(_mockSearchRepository.Object);
            _itemsByGroupRequestResponse.Success = true;

            // Act
            var response = await sut.GetItemsByGroupCollection(1, _searchRequest);

            // Assert
            _mockSearchRepository.Verify(r => r.GetItemsByGroupCollection(1, _searchRequest), Times.Once, "Expected method to be called once");
            Assert.IsTrue(response.Success, "Expected response.Success to be true");
        }

        [Test()]
        public async Task GetItemsByGroupCollectionResultFail()
        {
            // Arrange
            _mockSearchRepository.Reset();
            var idCollection = new List<int> { 20, 30, 40, 50 };
            _searchRequest.IdCollection = idCollection;
            _mockSearchRepository.Setup(r => r.GetItemsByGroupCollection(It.IsAny<int>(), It.IsAny<SearchRequest>())).Throws(_expectedException);
            var sut = new SearchManager(_mockSearchRepository.Object);

            // Act
            var response = await sut.GetItemsByGroupCollection(1, _searchRequest);

            // Assert
            _mockSearchRepository.Verify(r => r.GetItemsByGroupCollection(1, _searchRequest), Times.Once, "Expected method to be called once");
            Assert.IsTrue(!response.Success, "Expected response.Success to be false");
        }

        [Test()]
        public async Task GetItemsByTagCollectionResultSuccess()
        {
            // Arrange
            _mockSearchRepository.Reset();
            var idCollection = new List<int> { 110, 130, 140, 150 };
            _searchRequest.IdCollection = idCollection;
            _mockSearchRepository.Setup(r => r.GetItemsByTagCollection(It.IsAny<int>(), It.IsAny<SearchRequest>()))
                .Returns(Task.FromResult(_itemsByTagSearchRequestResponse));
            var sut = new SearchManager(_mockSearchRepository.Object);
            _itemsByTagSearchRequestResponse.Success = true;

            // Act
            var response = await sut.GetItemsByTagCollection(1, _searchRequest);

            // Assert
            _mockSearchRepository.Verify(r => r.GetItemsByTagCollection(1, _searchRequest), Times.Once, "Expected method to be called once");
            Assert.IsTrue(response.Success, "Expected response.Success to be true");
        }

        [Test()]
        public async Task GetItemsByTagCollectionResultFail()
        {
            // Arrange
            _mockSearchRepository.Reset();
            var idCollection = new List<int> { 110, 130, 140, 150 };
            _searchRequest.IdCollection = idCollection;
            _mockSearchRepository.Setup(r => r.GetItemsByTagCollection(It.IsAny<int>(), It.IsAny<SearchRequest>())).Throws(_expectedException);
            var sut = new SearchManager(_mockSearchRepository.Object);

            // Act
            var response = await sut.GetItemsByTagCollection(1, _searchRequest);

            // Assert
            _mockSearchRepository.Verify(r => r.GetItemsByTagCollection(1, _searchRequest), Times.Once, "Expected method to be called once");
            Assert.IsTrue(!response.Success, "Expected response.Success to be false");
        }

        [Test()]
        public async Task ItemsByTagShouldContainExpectedTags()
        {
            //  Arrange
            _mockSearchRepository.Reset();
            var idCollection = new List<int> { 110, 130, 140, 150 };
            _searchRequest.IdCollection = idCollection;

            var pageOneRequestResponse = SearchManagerSetUp.GetItemsPageOneRequestResponse(SearchManagerSetUp.AttributeType.Tag);
            var pageTwoRequestResponse = SearchManagerSetUp.GetItemsPageTwoRequestResponse(SearchManagerSetUp.AttributeType.Tag);


            _mockSearchRepository.SetupSequence(r => r.GetItemsByTagCollection(It.IsAny<int>(), It.IsAny<SearchRequest>()))
                .Returns(Task.FromResult(pageOneRequestResponse))
                .Returns(Task.FromResult(pageTwoRequestResponse));

            var sut = new SearchManager(_mockSearchRepository.Object);

            //  Act
            var responsePageOne = await sut.GetItemsByTagCollection(1, _searchRequest);
            _searchRequest.PagedCollection.PageNumber = 2;
            var responsePageTwo = await sut.GetItemsByTagCollection(1, _searchRequest);

            //  Assert
            _mockSearchRepository.Verify(r => r.GetItemsByTagCollection(1, _searchRequest), Times.Exactly(2), "Expected method to be called once for page(s)");
            Assert.IsTrue(responsePageOne.Item[0].Tags.Count == 0, "Expected responsePageOne.Item[0].Tags.Count == 0");
            Assert.IsTrue(responsePageOne.Item[1].Tags.Count == 1, "Expected responsePageOne.Item[1].Tags.Count == 1");
            Assert.IsTrue(responsePageOne.Item[1].Tags[0].Id == 150, "Expected responsePageOne.Item[1].Tags[0].Id == 150");
            Assert.IsTrue(responsePageOne.Item[2].Tags.Count == 0, "Expected responsePageOne.Item[2].Tags.Count == 0");
            Assert.IsTrue(responsePageTwo.Item[0].Tags.Count == 2, "Expected responsePageTwo.Item[1].Tags.Count == 2");
            Assert.IsTrue(responsePageTwo.Item[0].Tags.Exists(t => t.Id== 110), "Expected responsePageTwo.Item[0].Tags to contain tagId 110");
            Assert.IsTrue(responsePageTwo.Item[0].Tags.Exists(t => t.Id == 130), "Expected responsePageTwo.Item[0].Tags to contain tagId 130");
            Assert.IsTrue(responsePageTwo.Item[1].Tags.Count == 2, "Expected responsePageTwo.Item[1].Tags.Count == 2");
            Assert.IsTrue(responsePageTwo.Item[1].Tags.Exists(t => t.Id==130), "Expected responsePageTwo.Item[1].Tags to contain tagId 130");
            Assert.IsTrue(responsePageTwo.Item[1].Tags.Exists(t => t.Id == 140), "Expected responsePageTwo.Item[1].Tags to contain tagId 140");
            Assert.IsTrue(responsePageTwo.Item[2].Tags.Count == 1, "Expected responsePageTwo.Item[2].Tags.Count == 1");
            Assert.IsTrue(responsePageTwo.Item[2].Tags[0].Id == 110, "Expected responsePageTwo.Item[2].Tags[0].Id == 110");
        }

        [Test()]
        public async Task ItemsByGroupShouldContainExpectedGroups()
        {
            //  Arrange
            _mockSearchRepository.Reset();
            var idCollection = new List<int> { 20, 30, 40, 50 };
            _searchRequest.IdCollection = idCollection;
            var pageOneRequestResponse = SearchManagerSetUp.GetItemsPageOneRequestResponse(SearchManagerSetUp.AttributeType.Group);
            var pageTwoRequestResponse = SearchManagerSetUp.GetItemsPageTwoRequestResponse(SearchManagerSetUp.AttributeType.Group);

            _mockSearchRepository.SetupSequence(r => r.GetItemsByGroupCollection(It.IsAny<int>(), It.IsAny<SearchRequest>()))
                .Returns(Task.FromResult(pageOneRequestResponse))
                .Returns(Task.FromResult(pageTwoRequestResponse));

            var sut = new SearchManager(_mockSearchRepository.Object);

            //  Act
            var responsePageOne = await sut.GetItemsByGroupCollection(1, _searchRequest);
            _searchRequest.PagedCollection.PageNumber = 2;
            var responsePageTwo = await sut.GetItemsByGroupCollection(1, _searchRequest);

            //  Assert
            _mockSearchRepository.Verify(r => r.GetItemsByGroupCollection(1, _searchRequest), Times.Exactly(2), "Expected method to be called once for page(s)");
            Assert.IsTrue(responsePageOne.Item[0].Groups.Count == 4, "Expected responsePageOne.Item[0].Groups.Count == 4");
            Assert.IsTrue(responsePageOne.Item[0].Groups.Exists(g => g.Id == 20), "Expected responsePageOne.Item[0].Groups to contain groupId 20");
            Assert.IsTrue(responsePageOne.Item[0].Groups.Exists(g => g.Id == 30), "Expected responsePageOne.Item[0].Groups to contain groupId 30");
            Assert.IsTrue(responsePageOne.Item[0].Groups.Exists(g => g.Id == 40), "Expected responsePageOne.Item[0].Groups to contain groupId 40");
            Assert.IsTrue(responsePageOne.Item[0].Groups.Exists(g => g.Id == 50), "Expected responsePageOne.Item[0].Groups to contain groupId 50");
            //Assert.IsTrue(responsePageOne.Item[1].Groups.Count == 0, "Expected responsePageOne.Item[1].Groups.Count == 0");
            //Assert.IsTrue(responsePageOne.Item[2].Groups.Count == 0, "Expected responsePageOne.Item[2].Groups.Count == 0");
            //Assert.IsTrue(responsePageTwo.Item[0].Groups.Count == 2, "Expected responsePageTwo.Item[0].Groups.Count == 2");
            Assert.IsTrue(responsePageTwo.Item[0].Groups.Exists(g => g.Id == 20), "Expected responsePageTwo.Item[0].Groups to contain groupId 20");
            Assert.IsTrue(responsePageTwo.Item[0].Groups.Exists(g => g.Id == 30), "Expected responsePageTwo.Item[0].Groups to contain groupId 30");
            //Assert.IsTrue(responsePageTwo.Item[1].Groups.Count == 2, "Expected responsePageTwo.Item[1].Groups.Count == 2");
            Assert.IsTrue(responsePageTwo.Item[1].Groups.Exists(g => g.Id == 20), "Expected responsePageTwo.Item[1].Groups to contain groupId 20");
            Assert.IsTrue(responsePageTwo.Item[1].Groups.Exists(g => g.Id == 40), "Expected responsePageTwo.Item[1].Groups to contain groupId 40");
            //Assert.IsTrue(responsePageTwo.Item[2].Groups.Count == 1, "Expected responsePageTwo.Item[2].Groups.Count == 1");
            Assert.IsTrue(responsePageTwo.Item[2].Groups.Exists(g => g.Id == 20), "Expected responsePageTwo.Item[1].Groups to contain groupId 20");

        }

        [Test(), Ignore("Ignore Test")]
        public async Task SearchCollectionShouldAttributeGroupsAndTags()
        {
            //  Arrange
            _mockSearchRepository.Reset();
            var pageOneRequestResponse = SearchManagerSetUp.GetSearchResultsPageOne();
            var pageTwoRequestResponse = SearchManagerSetUp.GetSearchResultsPageTwo();

            _mockSearchRepository.SetupSequence(r => r.GetSearchResultCollection(It.IsAny<int>(), It.IsAny<SearchRequest>()))
                .Returns(Task.FromResult(pageOneRequestResponse))
                .Returns(Task.FromResult(pageTwoRequestResponse));

            var sut = new SearchManager(_mockSearchRepository.Object);
            _searchRequest.SearchTerm = "ap";

            //  Act
            var responsePageOne = await sut.GetSearchResultCollection(1, _searchRequest);
            _searchRequest.PagedCollection.PageNumber = 2;
            var responsePageTwo = await sut.GetSearchResultCollection(1, _searchRequest);

            //  Assert
            _mockSearchRepository.Verify(r => r.GetSearchResultCollection(1, _searchRequest), Times.Exactly(2), "Expected method to be called once for page(s)");
            //Assert.IsTrue(responsePageOne.Item[0].Groups.Count == 2, "Expected responsePageOne.Item[0].Groups.Count == 2");
            Assert.IsTrue(responsePageOne.Item[0].Groups.Exists(g => g.Id == 900), "Expected responsePageOne.Item[0].Groups to contain groupId 900");
            Assert.IsTrue(responsePageOne.Item[0].Groups.Exists(g => g.Id == 901), "Expected responsePageOne.Item[0].Groups to contain groupId 901");
            //Assert.IsTrue(responsePageOne.Item[0].Tags.Count == 1, "Expected responsePageOne.Item[0].Tags.Count == 1");
            Assert.IsTrue(responsePageOne.Item[0].Tags.Exists(t => t.Id == 110), "Expected responsePageOne.Item[0].Tags to contain groupId 110");
            //Assert.IsTrue(responsePageOne.Item[1].Groups.Count == 0, "Expected responsePageOne.Item[1].Groups.Count == 0");
            //Assert.IsTrue(responsePageOne.Item[1].Tags.Count == 2, "Expected responsePageOne.Item[1].Tags.Count == 2");
            Assert.IsTrue(responsePageOne.Item[1].Tags.Exists(t => t.Id == 130), "Expected responsePageOne.Item[1].Tags to contain groupId 130");
            Assert.IsTrue(responsePageOne.Item[1].Tags.Exists(t => t.Id == 140), "Expected responsePageOne.Item[1].Tags to contain groupId 140");
            //Assert.IsTrue(responsePageOne.Item[2].Tags.Count == 0, "Expected responsePageOne.Item[2].Tags.Count == 0");
            //Assert.IsTrue(responsePageOne.Item[2].Groups.Count == 1, "Expected responsePageOne.Item[2].Groups.Count == 1");
            Assert.IsTrue(responsePageOne.Item[2].Groups.Exists(g => g.Id == 902), "Expected responsePageOne.Item[2].Groups to contain groupId 902");
            //Assert.IsTrue(responsePageTwo.Item[0].Groups== null, "Expected responsePageTwo.Item[0].Groups to be null");
            //Assert.IsTrue(responsePageTwo.Item[0].Tags == null, "Expected responsePageTwo.Item[0].Tags to be null");
            Assert.IsTrue(responsePageTwo.Item[0].Name != string.Empty, "Expected responsePageTwo.Item[0].ResultName to be populated");
            Assert.IsTrue(responsePageTwo.Item[0].ResultType.ToLowerInvariant() != "item", "Expected responsePageTwo.Item[0].ResultType to != 'item'");
            Assert.IsTrue(responsePageTwo.Item[1].Groups == null, "Expected responsePageTwo.Item[1].Groups to be null");
            Assert.IsTrue(responsePageTwo.Item[1].Tags == null, "Expected responsePageTwo.Item[1].Tags to be null");
            Assert.IsTrue(responsePageTwo.Item[1].Name != string.Empty, "Expected responsePageTwo.Item[1].ResultName to be populated");
            Assert.IsTrue(responsePageTwo.Item[1].ResultType.ToLowerInvariant() != "item", "Expected responsePageTwo.Item[1].ResultType to != 'item'");
            Assert.IsTrue(responsePageTwo.Item[2].Groups == null, "Expected responsePageTwo.Item[2].Groups to be null");
            Assert.IsTrue(responsePageTwo.Item[2].Tags == null, "Expected responsePageTwo.Item[2].Tags to be null");
            Assert.IsTrue(responsePageTwo.Item[2].Name != string.Empty, "Expected responsePageTwo.Item[2].ResultName to be populated");
            Assert.IsTrue(responsePageTwo.Item[2].ResultType.ToLowerInvariant() != "item", "Expected responsePageTwo.Item[2].ResultType to != 'item'");
        }

    }
}