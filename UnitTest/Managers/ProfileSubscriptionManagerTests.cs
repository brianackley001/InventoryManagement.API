using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryManagement.API.DataProvider.SQL;
using Moq;
using InventoryManagement.API.Models;
using NLog;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;

namespace InventoryManagement.API.Managers.Tests
{
    [ExcludeFromCodeCoverage]
    [TestFixture()]
    [Category("ManagerTests")]
    public class ProfileSubscriptionManagerTests
    {
        private Mock<IProfileSubscriptionRepository> _mockProfileSubscriptionRepository;
        private Mock<ILogger> _mockLogger;
        private Mock<IConfiguration> _mockConfig;
        private RequestResponse<UserProfile> _userProfileResponse;
        private RequestResponse<Subscription> _subscriptionResponse;
        private string _authIdValue;
        private NullReferenceException _expectedException;

        [OneTimeSetUp]
        public void TestSetup()
        {
            _mockLogger = new Mock<ILogger>();
            _authIdValue = "Test-Auth-ID";
            _mockProfileSubscriptionRepository = new Mock<IProfileSubscriptionRepository>();
            _expectedException = new NullReferenceException("Test Went Boom!");
            _userProfileResponse = new RequestResponse<UserProfile>
            {
                Success = true,
                PagedCollection = new PagedCollection(),
                Item = new UserProfile { AuthId = "Test1", Id = 22, IsActive = true, Source = "UnitTest", Name = "TestingManager 001", Subscriptions = new List<Subscription>() }
            };
            _subscriptionResponse = new RequestResponse<Subscription>
            {
                Success = true,
                PagedCollection = new PagedCollection(),
                Item = new Subscription
                {
                    Id = 887,
                    IsActive = true,
                    IsSelectedSubscription = true,
                    Name = "Testing It Now",
                    ProfileId = 22,
                    ProfileSubscriptionId = 998
                }
            };

            _mockConfig = new Mock<IConfiguration>();
            //  https://dejanstojanovic.net/aspnet/2018/november/mocking-iconfiguration-getvalue-extension-methods-in-unit-test/
            _mockConfig.Setup(c => c.GetSection(It.IsAny<String>())).Returns(new Mock<IConfigurationSection>().Object);
        }



        [Test()]
        public async Task GetProfileSubscriptionsSuccessTest()
        {
            // Arrange
            _mockProfileSubscriptionRepository.Reset();
            _userProfileResponse.Success = true;
            _mockProfileSubscriptionRepository.Setup(r => r.GetProfileSubscriptions(It.IsAny<string>()))
                .Returns(Task.FromResult(_userProfileResponse));
            var sut = new ProfileSubscriptionManager(_mockProfileSubscriptionRepository.Object, _mockConfig.Object);
            _userProfileResponse.Success = true;

            // Act
            var response = await sut.GetProfileSubscriptions(_authIdValue);

            // Assert
            _mockProfileSubscriptionRepository.Verify(r => r.GetProfileSubscriptions(_authIdValue), Times.Once, "Expected method to be called once");
            Assert.IsTrue(response.Success, "Expected response.Success to be true");
        }
        [Test()]
        public async Task GetProfileSubscriptionsFailTest()
        {
            // Arrange
            _mockProfileSubscriptionRepository.Reset();
            _mockProfileSubscriptionRepository.Setup(r => r.GetProfileSubscriptions(It.IsAny<string>()))
                .Throws(_expectedException);
            var sut = new ProfileSubscriptionManager(_mockProfileSubscriptionRepository.Object, _mockConfig.Object);

            // Act
            var response = await sut.GetProfileSubscriptions(_authIdValue);

            // Assert
            _mockProfileSubscriptionRepository.Verify(r => r.GetProfileSubscriptions(_authIdValue), Times.Once, "Expected method to be called once");
            Assert.IsTrue(!response.Success, "Expected response.Success to be false");
        }

        [Test()]
        public async Task UpsertNewProfileSuccessTest()
        {
            // Arrange
            _mockProfileSubscriptionRepository.Reset();
            _userProfileResponse.Success = true;
            _subscriptionResponse.Success = true;
            _mockProfileSubscriptionRepository.Setup(r => r.UpsertProfile(It.IsAny<UserProfile>()))
                .Returns(Task.FromResult(_userProfileResponse));
            _mockProfileSubscriptionRepository.Setup(r => r.UpsertSubscription(It.IsAny<Subscription>()))
                .Returns(Task.FromResult(_subscriptionResponse));
            var sut = new ProfileSubscriptionManager(_mockProfileSubscriptionRepository.Object, _mockConfig.Object);
            var newProfile = new UserProfile { AuthId = "TestNewProfile", Id = 0, IsActive = true, Source = "UnitTest", Name = "TestingManager 001", Subscriptions = new List<Subscription>() };

            // Act
            var response = await sut.UpsertProfile(newProfile);

            // Assert
            _mockProfileSubscriptionRepository.Verify(r => r.UpsertProfile(newProfile), Times.Once, "Expected method to be called once");
            _mockProfileSubscriptionRepository.Verify(r => r.UpsertSubscription(It.IsAny<Subscription>()), Times.Once, "Expected method to be called once");
            Assert.IsTrue(response.Success, "Expected response.Success to be true");
        }

        [Test()]
        public async Task UpsertNewProfileFailProfileTest()
        {
            // Arrange
            _userProfileResponse.Success = false;
            _mockProfileSubscriptionRepository.Setup(r => r.UpsertProfile(It.IsAny<UserProfile>()))
                .Returns(Task.FromResult(_userProfileResponse));
            _mockProfileSubscriptionRepository.Setup(r => r.UpsertSubscription(It.IsAny<Subscription>()))
                .Returns(Task.FromResult(_subscriptionResponse));
            var sut = new ProfileSubscriptionManager(_mockProfileSubscriptionRepository.Object, _mockConfig.Object);
            var newProfile = new UserProfile { AuthId = "TestNewProfile", Id = 0, IsActive = true, Source = "UnitTest", Name = "TestingManager 001", Subscriptions = new List<Subscription>() };

            // Act
            var response = await sut.UpsertProfile(newProfile);

            // Assert
            _mockProfileSubscriptionRepository.Verify(r => r.UpsertProfile(newProfile), Times.Once, "Expected method to be called once");
            _mockProfileSubscriptionRepository.Verify(r => r.UpsertSubscription(It.IsAny<Subscription>()), Times.Once, "Expected method to be called once");
            Assert.IsTrue(!response.Success, "Expected response.Success to be false");
        }

        [Test()]
        public async Task UpsertNewProfileFailSubscriptionTest()
        {
            // Arrange
            _mockProfileSubscriptionRepository.Reset();
            _userProfileResponse.Success = true;
            _subscriptionResponse.Success = false;
            _mockProfileSubscriptionRepository.Setup(r => r.UpsertProfile(It.IsAny<UserProfile>()))
                .Returns(Task.FromResult(_userProfileResponse));
            _mockProfileSubscriptionRepository.Setup(r => r.UpsertSubscription(It.IsAny<Subscription>()))
                .Returns(Task.FromResult(_subscriptionResponse));
            var sut = new ProfileSubscriptionManager(_mockProfileSubscriptionRepository.Object, _mockConfig.Object);
            var newProfile = new UserProfile { AuthId = "TestNewProfile", Id = 0, IsActive = true, Source = "UnitTest", Name = "TestingManager 001", Subscriptions = new List<Subscription>() };

            // Act
            var response = await sut.UpsertProfile(newProfile);

            // Assert
            _mockProfileSubscriptionRepository.Verify(r => r.UpsertProfile(newProfile), Times.Once, "Expected method to be called once");
            _mockProfileSubscriptionRepository.Verify(r => r.UpsertSubscription(It.IsAny<Subscription>()), Times.Once, "Expected method to be called once");
            Assert.IsTrue(!response.Success, "Expected response.Success to be false");
        }


        [Test()]
        public async Task UpsertExistingProfileSuccessTest()
        {
            // Arrange
            _userProfileResponse.Success = true;
            _mockProfileSubscriptionRepository.Reset();
            _mockProfileSubscriptionRepository.Setup(r => r.UpsertProfile(It.IsAny<UserProfile>()))
                .Returns(Task.FromResult(_userProfileResponse));
            _mockProfileSubscriptionRepository.Setup(r => r.UpsertSubscription(It.IsAny<Subscription>()))
                .Returns(Task.FromResult(_subscriptionResponse));
            var sut = new ProfileSubscriptionManager(_mockProfileSubscriptionRepository.Object, _mockConfig.Object);
            var newProfile = new UserProfile { AuthId = "TestExistingProfile", Id = 255, IsActive = true, Source = "UnitTest", Name = "TestingManager 001", Subscriptions = new List<Subscription>() };

            // Act
            var response = await sut.UpsertProfile(newProfile);

            // Assert
            _mockProfileSubscriptionRepository.Verify(r => r.UpsertProfile(newProfile), Times.Once, "Expected method to be called once");
            _mockProfileSubscriptionRepository.Verify(r => r.UpsertSubscription(It.IsAny<Subscription>()), Times.Never, "Expected method to not be called once");
            Assert.IsTrue(response.Success, "Expected response.Success to be true");
        }
        [Test()]
        public async Task UpsertProfileSubscriptionSuccessTest()
        {
            // Arrange
            _subscriptionResponse.Success = true;
            _mockProfileSubscriptionRepository.Reset();
            _mockProfileSubscriptionRepository.Setup(r => r.UpsertProfileSubscription(It.IsAny<Subscription>()))
                .Returns(Task.FromResult(_subscriptionResponse));
            var sut = new ProfileSubscriptionManager(_mockProfileSubscriptionRepository.Object, _mockConfig.Object);
            var upsertSubscription = new Subscription { Id = 7777, Name = "Unit test Upsert", IsActive = true, IsSelectedSubscription = true, ProfileId = 987, ProfileSubscriptionId = 7878 };

            // Act
            var response = await sut.UpsertProfileSubscription(upsertSubscription);

            // Assert
            _mockProfileSubscriptionRepository.Verify(r => r.UpsertProfileSubscription(upsertSubscription), Times.Once, "Expected method to be called once");
            Assert.IsTrue(response.Success, "Expected response.Success to be true");
        }
        [Test()]
        public async Task UpsertProfileSubscriptionFailTest()
        {
            // Arrange
            _mockProfileSubscriptionRepository.Reset();
            _mockProfileSubscriptionRepository.Setup(r => r.UpsertProfileSubscription(It.IsAny<Subscription>()))
                .Throws(_expectedException);
            var sut = new ProfileSubscriptionManager(_mockProfileSubscriptionRepository.Object, _mockConfig.Object);
            var upsertSubscription = new Subscription { Id = 7777, Name = "Unit test Upsert", IsActive = true, IsSelectedSubscription = true, ProfileId = 987, ProfileSubscriptionId = 7878 };

            // Act
            var response = await sut.UpsertProfileSubscription(upsertSubscription);

            // Assert
            _mockProfileSubscriptionRepository.Verify(r => r.UpsertProfileSubscription(upsertSubscription), Times.Once, "Expected method to be called once");
            Assert.IsTrue(!response.Success, "Expected response.Success to be false");
        }

        [Test()]
        public async Task UpsertSubscriptionSuccesTest()
        {
            // Arrange
            _subscriptionResponse.Success = true;
            _mockProfileSubscriptionRepository.Reset();
            _mockProfileSubscriptionRepository.Setup(r => r.UpsertSubscription(It.IsAny<Subscription>()))
                .Returns(Task.FromResult(_subscriptionResponse));
            var sut = new ProfileSubscriptionManager(_mockProfileSubscriptionRepository.Object, _mockConfig.Object);
            var upsertSubscription = new Subscription { Id = 7777, Name = "Unit test Upsert", IsActive = true, IsSelectedSubscription = true, ProfileId = 987, ProfileSubscriptionId = 7878 };

            // Act
            var response = await sut.UpsertSubscription(upsertSubscription);

            // Assert
            _mockProfileSubscriptionRepository.Verify(r => r.UpsertSubscription(upsertSubscription), Times.Once, "Expected method to be called once");
            Assert.IsTrue(response.Success, "Expected response.Success to be true");
        }
        [Test()]
        public async Task UpsertSubscriptionFailTest()
        {
            // Arrange
            _mockProfileSubscriptionRepository.Reset();
            _mockProfileSubscriptionRepository.Setup(r => r.UpsertSubscription(It.IsAny<Subscription>()))
                .Throws(_expectedException);
            var sut = new ProfileSubscriptionManager(_mockProfileSubscriptionRepository.Object, _mockConfig.Object);
            var upsertSubscription = new Subscription { Id = 7777, Name = "Unit test Upsert", IsActive = true, IsSelectedSubscription = true, ProfileId = 987, ProfileSubscriptionId = 7878 };

            // Act
            var response = await sut.UpsertSubscription(upsertSubscription);

            // Assert
            _mockProfileSubscriptionRepository.Verify(r => r.UpsertSubscription(upsertSubscription), Times.Once, "Expected method to be called once");
            Assert.IsTrue(!response.Success, "Expected response.Success to be false");
        }
    }
}