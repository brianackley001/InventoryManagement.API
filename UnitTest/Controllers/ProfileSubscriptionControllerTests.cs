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
    public class ProfileSubscriptionControllerTests
    {
        private Mock<IProfileSubscriptionManager> _profileSubscriptionManager;
        private Mock<ILogger> _mockLogger;
        private RequestResponse<UserProfile> _userProfileGet;
        private UserProfile _requestedProfile;
        private RequestResponse<Subscription> _userProfileSubscription;
        private NullReferenceException _expectedException;


        [ExcludeFromCodeCoverage, OneTimeSetUp]
        public void TestSetup()
        {
            _expectedException = new NullReferenceException("Test Went Boom!");
            _profileSubscriptionManager = new Mock<IProfileSubscriptionManager>();
            _mockLogger = new Mock<ILogger>();
            _requestedProfile = new UserProfile
            {
                AuthId = "UnitTest01",
                Source = "NUnit",
                IsActive = true,
                Name = "Default Unit Test Profile",
                Id = 667,
                Subscriptions = new List<Subscription>
                {
                    new Subscription {Id=22, Name="Sample", IsActive=true, IsSelectedSubscription=true, ProfileId=1, ProfileSubscriptionId=55}
                }
            };
            _userProfileGet = new RequestResponse<UserProfile>
            {
                Item = _requestedProfile,
                Success = true,
                PagedCollection = new PagedCollection()
            };
            _userProfileSubscription = new RequestResponse<Subscription>
            {
                Item = new Subscription { Id = 1, IsActive=true, IsSelectedSubscription=true, Name="RnadomStuff", ProfileId=23, ProfileSubscriptionId=77},
                Success = true,
                PagedCollection = new PagedCollection()
            };
        }

        [Test()]
        public async Task GetProfileSubscriptionsResultOkTest()
        {
            // Arrange
            _profileSubscriptionManager.Reset();
            _profileSubscriptionManager.Setup(s => s.GetProfileSubscriptions(It.IsAny<string>())).Returns(Task.FromResult(_userProfileGet));
            var sut = new ProfileSubscriptionController(_profileSubscriptionManager.Object);

            // Act
            var response = await sut.GetProfileSubscriptions("Test");
            var okObjectResult = response as OkObjectResult;
            var responseModel = okObjectResult.Value as RequestResponse<UserProfile>;

            // Assert
            Assert.IsNotNull(response, "response != null");
            Assert.IsNotNull(responseModel, "responseModel != null");
            Assert.IsNotNull(responseModel.Item, "responseModel.Item != null");
            _profileSubscriptionManager.Verify(m => m.GetProfileSubscriptions("Test"), Times.Once, "Expected method to be called once");
        }

        [Test()]
        public async Task GetProfileSubscriptions500Test()
        {
            // Arrange
            _profileSubscriptionManager.Reset();
            _profileSubscriptionManager.Setup(s => s.GetProfileSubscriptions(It.IsAny<string>())).Throws(_expectedException);
            var sut = new ProfileSubscriptionController(_profileSubscriptionManager.Object);

            // Act
            var response = await sut.GetProfileSubscriptions("Test");
            var statusCodeResult = response as StatusCodeResult;

            // Assert
            Assert.IsNotNull(statusCodeResult, "statusCodeResult != null");
            Assert.IsTrue(statusCodeResult.StatusCode == 500);
            _profileSubscriptionManager.Verify(m => m.GetProfileSubscriptions("Test"), Times.Once, "Expected method to be called once");
        }

        [Test()]
        public async Task UpsertProfileResultOkTest()
        {
            // Arrange
            _profileSubscriptionManager.Reset();
            _profileSubscriptionManager.Setup(s => s.UpsertProfile(It.IsAny<UserProfile>())).Returns(Task.FromResult(_userProfileGet));
            var sut = new ProfileSubscriptionController(_profileSubscriptionManager.Object);

            // Act
            var response = await sut.UpsertProfile(_userProfileGet.Item);
            var okObjectResult = response as OkObjectResult;
            var responseModel = okObjectResult.Value as RequestResponse<UserProfile>;

            // Assert
            Assert.IsNotNull(response, "response != null");
            Assert.IsNotNull(responseModel, "responseModel != null");
            Assert.IsNotNull(responseModel.Item, "responseModel.Item != null");
            _profileSubscriptionManager.Verify(m => m.UpsertProfile(_userProfileGet.Item), Times.Once, "Expected method to be called once");
        }

        [Test()]
        public async Task UpsertProfile500Test()
        {
            // Arrange
            _profileSubscriptionManager.Reset();
            _profileSubscriptionManager.Setup(s => s.UpsertProfile(It.IsAny<UserProfile>())).Throws(_expectedException);
            var sut = new ProfileSubscriptionController(_profileSubscriptionManager.Object);

            // Act
            var response = await sut.UpsertProfile(_userProfileGet.Item);
            var statusCodeResult = response as StatusCodeResult;

            // Assert
            Assert.IsNotNull(statusCodeResult, "statusCodeResult != null");
            Assert.IsTrue(statusCodeResult.StatusCode == 500);
            _profileSubscriptionManager.Verify(m => m.UpsertProfile(_userProfileGet.Item), Times.Once, "Expected method to be called once");
        }
        [Test()]
        public async Task UpsertProfileSubscriptionResultOkTest()
        {
            // Arrange
            _profileSubscriptionManager.Reset();
            _profileSubscriptionManager.Setup(s => s.UpsertProfileSubscription(It.IsAny<Subscription>())).Returns(Task.FromResult(_userProfileSubscription));
            var sut = new ProfileSubscriptionController(_profileSubscriptionManager.Object);

            // Act
            var response = await sut.UpsertProfileSubscription(_userProfileSubscription.Item);
            var okObjectResult = response as OkObjectResult;
            var responseModel = okObjectResult.Value as RequestResponse<Subscription>;

            // Assert
            Assert.IsNotNull(response, "response != null");
            Assert.IsNotNull(responseModel, "responseModel != null");
            Assert.IsNotNull(responseModel.Item, "responseModel.Item != null");
            _profileSubscriptionManager.Verify(m => m.UpsertProfileSubscription(_userProfileSubscription.Item), Times.Once, "Expected method to be called once");
        }
        [Test()]
        public async Task UpsertProfileSubscription500Test()
        {
            // Arrange
            _profileSubscriptionManager.Reset();
            _profileSubscriptionManager.Setup(s => s.UpsertProfileSubscription(It.IsAny<Subscription>())).Throws(_expectedException);
            var sut = new ProfileSubscriptionController(_profileSubscriptionManager.Object);

            // Act
            var response = await sut.UpsertProfileSubscription(_userProfileSubscription.Item);
            var statusCodeResult = response as StatusCodeResult;

            // Assert
            Assert.IsNotNull(statusCodeResult, "statusCodeResult != null");
            Assert.IsTrue(statusCodeResult.StatusCode == 500);
            _profileSubscriptionManager.Verify(m => m.UpsertProfileSubscription(_userProfileSubscription.Item), Times.Once, "Expected method to be called once");
        }
        [Test()]
        public async Task UpsertSubscriptionResultOkTest()
        {
            // Arrange
            _profileSubscriptionManager.Reset();
            _profileSubscriptionManager.Setup(s => s.UpsertSubscription(It.IsAny<Subscription>())).Returns(Task.FromResult(_userProfileSubscription));
            var sut = new ProfileSubscriptionController(_profileSubscriptionManager.Object);

            // Act
            var response = await sut.UpsertSubscription(_userProfileSubscription.Item);
            var okObjectResult = response as OkObjectResult;
            var responseModel = okObjectResult.Value as RequestResponse<Subscription>;

            // Assert
            Assert.IsNotNull(response, "response != null");
            Assert.IsNotNull(responseModel, "responseModel != null");
            Assert.IsNotNull(responseModel.Item, "responseModel.Item != null");
            _profileSubscriptionManager.Verify(m => m.UpsertSubscription(_userProfileSubscription.Item), Times.Once, "Expected method to be called once");
        }
        [Test()]
        public async Task UpsertSubscription500Test()
        {
            // Arrange
            _profileSubscriptionManager.Reset();
            _profileSubscriptionManager.Setup(s => s.UpsertSubscription(It.IsAny<Subscription>())).Throws(_expectedException);
            var sut = new ProfileSubscriptionController(_profileSubscriptionManager.Object);

            // Act
            var response = await sut.UpsertSubscription(_userProfileSubscription.Item);
            var statusCodeResult = response as StatusCodeResult;

            // Assert
            Assert.IsNotNull(statusCodeResult, "statusCodeResult != null");
            Assert.IsTrue(statusCodeResult.StatusCode == 500);
            _profileSubscriptionManager.Verify(m => m.UpsertSubscription(_userProfileSubscription.Item), Times.Once, "Expected method to be called once");
        }
    }
}