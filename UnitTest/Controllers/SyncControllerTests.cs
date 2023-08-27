using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using InventoryManagement.API.Models;
using Moq;
using InventoryManagement.API.Managers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture()]
    [Category("ControllerTests")]
    public class SyncControllerTests
    {
        private Mock<IProfileSyncManager> _profileSyncManager;
        private RequestResponse<ProfileSync> _profileSyncResponse;
        private NullReferenceException _expectedException;
        private UserProfile _userProfile;


        [ExcludeFromCodeCoverage, OneTimeSetUp]
        public void TestSetup()
        {
            _expectedException = new NullReferenceException("Test Went Boom!");
            _profileSyncManager = new Mock<IProfileSyncManager>();
            _profileSyncResponse = new RequestResponse<ProfileSync>
            {
                Success = true,
                PagedCollection = null,
                Item = new ProfileSync
                {
                    Groups = new List<Group>
                    {
                    new Group { Id=1, Name="Test1", SubscriptionId=1},
                    new Group { Id=2, Name="Test2", SubscriptionId=1},
                    new Group { Id=3, Name="Test3", SubscriptionId=1}
                    },
                    ShoppingLists = new List<ShoppingList>
                    {
                        new ShoppingList{Id=100, SubscriptionId=1, Name="List-1"},
                        new ShoppingList{Id=101, SubscriptionId=1, Name="List-2"}
                    },
                    Tags = new List<Tag>
                    {
                    new Tag { Id=1, Name="Test1", SubscriptionId=1},
                    new Tag { Id=2, Name="Test2", SubscriptionId=1},
                    new Tag { Id=3, Name="Test3", SubscriptionId=1}
                    },
                    UserProfile = new UserProfile
                    {
                        AuthId="345hgdf",
                        Name="Test 1",
                        Id=98765,
                        Source = "auth0",
                        Subscriptions = new List<Subscription>
                        {
                            new Subscription{Id=98879, Name="Default", IsSelectedSubscription=true, ProfileId=3434, ProfileSubscriptionId=1}
                        }
                    },
                    UserPreferences = new UserPreferences { }
                }
            };
            _userProfile = new UserProfile
            {
                AuthId = "xyz123",
                Id = 0,
                Source = "unit-test"
            };
        }

        [Test()]
        public async Task InitSessionShouldReturnExistingUserOk()
        {
            // Arrange
            _profileSyncManager.Reset();
            _profileSyncManager.Setup(s => s.GetProfileSync(It.IsAny<UserProfile>())).Returns(Task.FromResult(_profileSyncResponse));
            var sut = new SyncController(_profileSyncManager.Object);

            // Act
            var response = await sut.InitSession(_userProfile);
            var okObjectResult = response as OkObjectResult;
            var responseModel = okObjectResult.Value as RequestResponse<ProfileSync>;

            // Assert
            Assert.IsNotNull(response, "response != null");
            Assert.IsNotNull(responseModel, "responseModel != null");
            Assert.IsNotNull(responseModel.Item, "responseModel.Item != null");
            _profileSyncManager.Verify(m => m.GetProfileSync(_userProfile), Times.Once, "Expected method to be called once");
        }

        [Test()]
        public async Task InitSession500Test()
        {
            // Arrange
            _profileSyncManager.Reset();
            _profileSyncManager.Setup(s => s.GetProfileSync(It.IsAny<UserProfile>())).Throws(_expectedException);
            var sut = new SyncController(_profileSyncManager.Object);

            // Act
            var response = await sut.InitSession(_userProfile);
            var statusCodeResult = response as StatusCodeResult;

            // Assert
            Assert.IsNotNull(statusCodeResult, "statusCodeResult != null");
            Assert.IsTrue(statusCodeResult.StatusCode == 500);
            _profileSyncManager.Verify(m => m.GetProfileSync(_userProfile), Times.Once, "Expected method to be called once");
        }
    }
}
