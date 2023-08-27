using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using InventoryManagement.API.Models;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;

namespace InventoryManagement.API.Managers.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture()]
    [Category("ManagerTests")]
    public class ProfileSyncManagerTests
    {
        private Mock<IProfileSubscriptionManager> _mockProfileSubscriptionManager;
        private Mock<ITagManager> _mockTagManager;
        private Mock<IGroupManager> _mockGroupManager;
        private Mock<IShoppingListManager> _mockShoppingListManager;
        private Mock<IConfiguration> _mockConfig;
        private RequestResponse<UserProfile> _userProfileResponse;
        private RequestResponse<List<Tag>> _tagResponse;
        private RequestResponse<List<Group>> _groupResponse;
        private RequestResponse<List<ShoppingList>> _shoppingListResponse;
        private UserProfile _userProfile;


        [OneTimeSetUp]
        public void TestSetup()
        {
            _mockProfileSubscriptionManager = new Mock<IProfileSubscriptionManager>();
            _mockTagManager = new Mock<ITagManager>();
            _mockGroupManager = new Mock<IGroupManager>();
            _mockShoppingListManager = new Mock<IShoppingListManager>();
            _userProfileResponse = new RequestResponse<UserProfile>
            {
                Success = true,
                PagedCollection = new PagedCollection(),
                Item = new UserProfile
                {
                    AuthId = "Test1",
                    Id = 22,
                    IsActive = true,
                    Source = "UnitTest",
                    Name = "TestingManager 001",
                    Subscriptions = new List<Subscription>
                    {
                        new Subscription
                        {
                            Id=6634347,
                            IsSelectedSubscription = false,
                            ProfileId = 22,
                            ProfileSubscriptionId = 987,
                            IsActive = true
                        },
                        new Subscription
                        {
                            Id=667,
                            IsSelectedSubscription = true,
                            ProfileId = 22,
                            ProfileSubscriptionId = 987,
                            IsActive = true
                        }
                    }
                }
            };
            _tagResponse = new RequestResponse<List<Tag>>
            {
                PagedCollection = null,
                Success = true,
                Item = new List<Tag>
                {
                    new Tag{Id=3434, Name="test"}
                }
            };
            _groupResponse = new RequestResponse<List<Group>>
            {
                PagedCollection = null,
                Success = true,
                Item = new List<Group>
                {
                    new Group{Id=3434, Name="test"}
                }
            };
            _shoppingListResponse = new RequestResponse<List<ShoppingList>>
            {
                PagedCollection = null,
                Success = true,
                Item = new List<ShoppingList>
                {
                    new ShoppingList{Id=3434, Name="test"}
                }
            };
            _userProfile = new UserProfile
            {
                AuthId = "xyz123",
                Id = 0,
                Source = "unit-test"
            };

            _mockConfig = new Mock<IConfiguration>();
            //  https://dejanstojanovic.net/aspnet/2018/november/mocking-iconfiguration-getvalue-extension-methods-in-unit-test/
            _mockConfig.Setup(c => c.GetSection(It.IsAny<String>())).Returns(new Mock<IConfigurationSection>().Object);
        }

        [Test()]
        public async Task GetProfileSyncExistingProfileActiveSubscriptionSuccess()
        {
            // Arrange
            _mockProfileSubscriptionManager.Reset();
            _mockGroupManager.Reset();
            _mockShoppingListManager.Reset();
            _mockTagManager.Reset();
            _userProfileResponse.Success = true;
            _tagResponse.Success = true;

            _mockProfileSubscriptionManager.Setup(r => r.GetProfileSubscriptions(It.IsAny<string>()))
                .ReturnsAsync(_userProfileResponse);
            _mockShoppingListManager.Setup(r => r.GetShoppingListCollection(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(_shoppingListResponse);
            _mockGroupManager.Setup(r => r.GetGroups(It.IsAny<int>()))
                .ReturnsAsync(_groupResponse);
            _mockTagManager.Setup(r => r.GetTags(It.IsAny<int>()))
                .ReturnsAsync(_tagResponse);

            var sut = new ProfileSyncManager(_mockProfileSubscriptionManager.Object, _mockTagManager.Object,_mockGroupManager.Object, _mockShoppingListManager.Object);

            // Act
            var response = await sut.GetProfileSync(_userProfile);
            var activeSubscriptionId = response.Item.UserProfile.Subscriptions.Find(a => a.IsSelectedSubscription == true).Id;

            // Assert
            _mockProfileSubscriptionManager.Verify(r => r.GetProfileSubscriptions(_userProfile.AuthId), Times.Once, "Expected method to be called once");
            _mockProfileSubscriptionManager.Verify(r => r.UpsertProfile(_userProfile), Times.Never, "Expected method to be called zero times");

            // Active Subsription ID triggers query for associated Groups/Tags/Shopping Lists...
            _mockGroupManager.Verify(r => r.GetGroups(activeSubscriptionId), Times.Once, "Expected method to be called once");
            _mockShoppingListManager.Verify(r => r.GetShoppingListCollection(activeSubscriptionId, 1, 100), Times.Once, "Expected method to be called once");
            _mockTagManager.Verify(r => r.GetTags(activeSubscriptionId), Times.Once, "Expected method to be called once");
            Assert.IsTrue(response.Success, "Expected response.Success to be true");
        }

        [Test()]
        public async Task GetProfileSyncExistingProfileActiveSubscriptionFailure()
        {
            // Arrange
            _mockProfileSubscriptionManager.Reset();
            _mockGroupManager.Reset();
            _mockShoppingListManager.Reset();
            _mockTagManager.Reset();
            _tagResponse.Success = false;
            _userProfileResponse.Success = true;

            _mockProfileSubscriptionManager.Setup(r => r.GetProfileSubscriptions(It.IsAny<string>()))
                .ReturnsAsync(_userProfileResponse);
            _mockShoppingListManager.Setup(r => r.GetShoppingListCollection(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(_shoppingListResponse);
            _mockGroupManager.Setup(r => r.GetGroups(It.IsAny<int>()))
                .ReturnsAsync(_groupResponse);
            _mockTagManager.Setup(r => r.GetTags(It.IsAny<int>()))
                .ReturnsAsync(_tagResponse);

            var sut = new ProfileSyncManager(_mockProfileSubscriptionManager.Object, _mockTagManager.Object, _mockGroupManager.Object, _mockShoppingListManager.Object);

            // Act
            var response = await sut.GetProfileSync(_userProfile);
            var activeSubscriptionId = response.Item.UserProfile.Subscriptions.Find(a => a.IsSelectedSubscription == true).Id;

            // Assert
            _mockProfileSubscriptionManager.Verify(r => r.GetProfileSubscriptions(_userProfile.AuthId), Times.Once, "Expected method to be called once");
            _mockProfileSubscriptionManager.Verify(r => r.UpsertProfile(_userProfile), Times.Never, "Expected method to be called zero times");

            // Active Subsription ID triggers query for associated Groups/Tags/Shopping Lists...
            _mockGroupManager.Verify(r => r.GetGroups(activeSubscriptionId), Times.Once, "Expected method to be called once");
            _mockShoppingListManager.Verify(r => r.GetShoppingListCollection(activeSubscriptionId, 1, 100), Times.Once, "Expected method to be called once");
            _mockTagManager.Verify(r => r.GetTags(activeSubscriptionId), Times.Once, "Expected method to be called once");
            Assert.IsFalse(response.Success, "Expected response.Success to be true");
        }

        [Test()]
        public async Task GetProfileSyncNewProfileActiveSubscriptionSuccess()
        {
            // Arrange
            _mockProfileSubscriptionManager.Reset();
            _mockGroupManager.Reset();
            _mockShoppingListManager.Reset();
            _mockTagManager.Reset();
            _userProfileResponse.Success = true;
            _tagResponse.Success = true;
            _userProfileResponse.Item.Id = 0;

            _mockProfileSubscriptionManager.Setup(r => r.GetProfileSubscriptions(It.IsAny<string>()))
                .ReturnsAsync(_userProfileResponse);
            _mockProfileSubscriptionManager.Setup(r => r.UpsertProfile(It.IsAny<UserProfile>()))
                .ReturnsAsync(_userProfileResponse);
            _mockShoppingListManager.Setup(r => r.GetShoppingListCollection(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(_shoppingListResponse);
            _mockGroupManager.Setup(r => r.GetGroups(It.IsAny<int>()))
                .ReturnsAsync(_groupResponse);
            _mockTagManager.Setup(r => r.GetTags(It.IsAny<int>()))
                .ReturnsAsync(_tagResponse);

            var sut = new ProfileSyncManager(_mockProfileSubscriptionManager.Object, _mockTagManager.Object, _mockGroupManager.Object, _mockShoppingListManager.Object);

            // Act
            var response = await sut.GetProfileSync(_userProfile);
            var activeSubscriptionId = response.Item.UserProfile.Subscriptions.Find(a => a.IsSelectedSubscription == true).Id;

            // Assert
            _mockProfileSubscriptionManager.Verify(r => r.GetProfileSubscriptions(_userProfile.AuthId), Times.Once, "Expected method to be called once");
            _mockProfileSubscriptionManager.Verify(r => r.UpsertProfile(_userProfile), Times.Once, "Expected method to be called once");

            // Active Subsription ID triggers query for associated Groups/Tags/Shopping Lists...
            _mockGroupManager.Verify(r => r.GetGroups(activeSubscriptionId), Times.Once, "Expected method to be called once");
            _mockShoppingListManager.Verify(r => r.GetShoppingListCollection(activeSubscriptionId, 1, 100), Times.Once, "Expected method to be called once");
            _mockTagManager.Verify(r => r.GetTags(activeSubscriptionId), Times.Once, "Expected method to be called once");
            Assert.IsTrue(response.Success, "Expected response.Success to be true");
        }

        [Test()]
        public async Task GetProfileSyncNewProfileActiveSubscriptionFailure()
        {
            // Arrange
            _mockProfileSubscriptionManager.Reset();
            _mockGroupManager.Reset();
            _mockShoppingListManager.Reset();
            _mockTagManager.Reset();
            _userProfileResponse.Success = true;
            _tagResponse.Success = false;
            _userProfileResponse.Item.Id = 0;

            _mockProfileSubscriptionManager.Setup(r => r.GetProfileSubscriptions(It.IsAny<string>()))
                .ReturnsAsync(_userProfileResponse);
            _mockProfileSubscriptionManager.Setup(r => r.UpsertProfile(It.IsAny<UserProfile>()))
                .ReturnsAsync(_userProfileResponse);
            _mockShoppingListManager.Setup(r => r.GetShoppingListCollection(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(_shoppingListResponse);
            _mockGroupManager.Setup(r => r.GetGroups(It.IsAny<int>()))
                .ReturnsAsync(_groupResponse);
            _mockTagManager.Setup(r => r.GetTags(It.IsAny<int>()))
                .ReturnsAsync(_tagResponse);

            var sut = new ProfileSyncManager(_mockProfileSubscriptionManager.Object, _mockTagManager.Object, _mockGroupManager.Object, _mockShoppingListManager.Object);

            // Act
            var response = await sut.GetProfileSync(_userProfile);
            var activeSubscriptionId = response.Item.UserProfile.Subscriptions.Find(a => a.IsSelectedSubscription == true).Id;

            // Assert
            _mockProfileSubscriptionManager.Verify(r => r.GetProfileSubscriptions(_userProfile.AuthId), Times.Once, "Expected method to be called once");
            _mockProfileSubscriptionManager.Verify(r => r.UpsertProfile(_userProfile), Times.Once, "Expected method to be called once");

            // Active Subsription ID triggers query for associated Groups/Tags/Shopping Lists...
            _mockGroupManager.Verify(r => r.GetGroups(activeSubscriptionId), Times.Once, "Expected method to be called once");
            _mockShoppingListManager.Verify(r => r.GetShoppingListCollection(activeSubscriptionId, 1, 100), Times.Once, "Expected method to be called once");
            _mockTagManager.Verify(r => r.GetTags(activeSubscriptionId), Times.Once, "Expected method to be called once");
            Assert.IsFalse(response.Success, "Expected response.Success to be true");
        }
    }
}
