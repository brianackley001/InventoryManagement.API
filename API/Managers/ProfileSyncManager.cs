using InventoryManagement.API.Models;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagement.API.Managers
{
    public class ProfileSyncManager : IProfileSyncManager
    {
        private readonly IProfileSubscriptionManager _profileSubscriptionManager;
        private readonly ITagManager _tagManager;
        private readonly IGroupManager _groupManager;
        private readonly IShoppingListManager _shoppingListManager;

        public ProfileSyncManager(IProfileSubscriptionManager profileSubscriptionManager, ITagManager tagManager, IGroupManager groupManager, IShoppingListManager shoppingListManager)
        {
            _profileSubscriptionManager = profileSubscriptionManager;
            _tagManager = tagManager;
            _groupManager = groupManager;
            _shoppingListManager = shoppingListManager;
        }

        public async Task<RequestResponse<ProfileSync>> GetProfileSync(UserProfile userProfile)
        {
            var profileSyncResponse = new RequestResponse<ProfileSync>();
            var profileSyncItem = new ProfileSync();
            var activeSubscriptionId = -1;

            //  GET profile based on authID

            var requestResponse = await _profileSubscriptionManager.GetProfileSubscriptions(userProfile.AuthId).ConfigureAwait(true);
            profileSyncResponse.Success = requestResponse.Success;
            profileSyncItem.UserProfile = requestResponse.Item;

            if (requestResponse.Item.Id <= 0)
            {
                //  check for new user
                var upsertProfileResponse = await _profileSubscriptionManager.UpsertProfile(userProfile).ConfigureAwait(true);
                profileSyncResponse.Success = upsertProfileResponse.Success;
                profileSyncItem.UserProfile = upsertProfileResponse.Item;
            }

            activeSubscriptionId = profileSyncItem.UserProfile.Subscriptions.FirstOrDefault(a => a.IsSelectedSubscription).Id;

            if(activeSubscriptionId > 0)
            {
                var tagTask = _tagManager.GetTags(activeSubscriptionId);
                var groupTask = _groupManager.GetGroups(activeSubscriptionId);
                var shoppingListTask = _shoppingListManager.GetShoppingListCollection(activeSubscriptionId, 1, 100);

                await Task.WhenAll(tagTask, groupTask, shoppingListTask);

                profileSyncItem.Tags = tagTask.Result.Item;
                profileSyncItem.Groups = groupTask.Result.Item;
                profileSyncItem.ShoppingLists = shoppingListTask.Result.Item;

                profileSyncResponse.Success &= tagTask.Result.Success && groupTask.Result.Success && shoppingListTask.Result.Success;
            }

            profileSyncResponse.Item = profileSyncItem;

            return profileSyncResponse;
        }
    }
}
