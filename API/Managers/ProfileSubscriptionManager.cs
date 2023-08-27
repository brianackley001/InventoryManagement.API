using System;
using System.Threading.Tasks;
using InventoryManagement.API.Models;
using InventoryManagement.API.DataProvider.SQL;
using NLog;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace InventoryManagement.API.Managers
{
    public class ProfileSubscriptionManager : IProfileSubscriptionManager
    {
        private readonly IProfileSubscriptionRepository _profileSubscriptionRepository;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly string _defaultSubscriptionName;


        public ProfileSubscriptionManager(IProfileSubscriptionRepository profileSubscriptionRepository, IConfiguration configuration)
        {
            _profileSubscriptionRepository = profileSubscriptionRepository;
            _defaultSubscriptionName = configuration.GetValue<string>("Subscription:DefaultName");
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "<Pending>")]
        public async Task<RequestResponse<UserProfile>> GetProfileSubscriptions(string authId)
        {
            var profileSubscriptions = new RequestResponse<UserProfile>();

            try
            {
                profileSubscriptions = await _profileSubscriptionRepository.GetProfileSubscriptions(authId).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                profileSubscriptions.Success = false;
            }
            return profileSubscriptions;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "<Pending>")]
        public async Task<RequestResponse<UserProfile>> UpsertProfile(UserProfile userProfile)
        {
            var upsertProfileResult = new RequestResponse<UserProfile>();
            var isNewProfile = false;
            if(userProfile.Id < 1)
            {
                isNewProfile = true;
            }

            try
            {
                upsertProfileResult = await _profileSubscriptionRepository.UpsertProfile(userProfile).ConfigureAwait(true);

                //  Add default subscription?
                if (isNewProfile)
                {
                    var defaultSubscription = new Subscription
                    {
                        ProfileId = userProfile.Id,
                        Name = (userProfile.Subscriptions == null || userProfile.Subscriptions.Count < 1) ? _defaultSubscriptionName : userProfile.Subscriptions.FirstOrDefault().Name,
                        IsActive = (userProfile.Subscriptions == null || userProfile.Subscriptions.Count < 1) || userProfile.Subscriptions.FirstOrDefault().IsActive,
                        IsSelectedSubscription = (userProfile.Subscriptions == null || userProfile.Subscriptions.Count < 1) || userProfile.Subscriptions.FirstOrDefault().IsSelectedSubscription
                    };

                    var upsertSubscriptionResult = await _profileSubscriptionRepository.UpsertSubscription(defaultSubscription);

                    if(upsertSubscriptionResult.Success)
                    {
                        userProfile.Subscriptions = new System.Collections.Generic.List<Subscription>
                        {
                            upsertSubscriptionResult.Item
                        };
                    }

                    upsertProfileResult.Success = upsertProfileResult.Success && upsertSubscriptionResult.Success;
                }

            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                upsertProfileResult.Success = false;
            }

            return upsertProfileResult;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "<Pending>")]
        public async Task<RequestResponse<Subscription>> UpsertProfileSubscription(Subscription subscription)
        {
            var upsertProfileSubscriptionResult = new RequestResponse<Subscription>();
            try
            {
                upsertProfileSubscriptionResult = await _profileSubscriptionRepository.UpsertProfileSubscription(subscription);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                upsertProfileSubscriptionResult.Success = false;
            }
            return upsertProfileSubscriptionResult;
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "<Pending>")]
        public async Task<RequestResponse<Subscription>> UpsertSubscription(Subscription subscription)
        {
            var upsertSubscriptionResult = new RequestResponse<Subscription>();
            try
            {
                upsertSubscriptionResult = await _profileSubscriptionRepository.UpsertSubscription(subscription);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                upsertSubscriptionResult.Success = false;
            }
            return upsertSubscriptionResult;
        }
    }
}
