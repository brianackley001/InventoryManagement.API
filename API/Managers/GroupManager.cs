using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryManagement.API.Models;
using InventoryManagement.API.DataProvider.SQL;
using NLog;

namespace InventoryManagement.API.Managers
{
    public class GroupManager : IGroupManager
    {
        private readonly IGroupRepository _groupRepository;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public GroupManager(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "<Pending>")]
        public async Task<RequestResponse<List<Group>>> GetGroupCollection(int subscriptionId, int pageNumber, int pageSize)
        {
            var groupCollection = new RequestResponse<List<Group>>();

            try
            {
                groupCollection = await _groupRepository.GetGroupCollection(subscriptionId, pageNumber, pageSize).ConfigureAwait(true); 
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                groupCollection.Success = false; 
            }
            return groupCollection;
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "<Pending>")]
        public async Task<RequestResponse<List<Group>>> GetGroupCollectionWithItemCounts(int subscriptionId, SearchRequest searchRequest)
        {
            var groupCollection = new RequestResponse<List<Group>>();

            try
            {
                groupCollection = await _groupRepository.GetGroupCollectionWithItemCounts(subscriptionId, searchRequest).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                groupCollection.Success = false;
            }
            return groupCollection;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "<Pending>")]
        public async Task<RequestResponse<List<Group>>> GetGroups(int subscriptionId)
        {
            var groups = new RequestResponse<List<Group>>();

            try
            {
                groups = await _groupRepository.GetGroups(subscriptionId).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                groups.Success = false;
            }

            return groups;
        }

        public async Task<RequestResponse<Group>> UpsertGroup(Group group)
        {
            var insertGroupId = -1;

            try
            {
                insertGroupId = await _groupRepository.UpsertGroup(group).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
            }

            if(insertGroupId > -1 && group.Id < 1)
            {
                group.Id = insertGroupId;
                group.CreateDate = DateTime.Now;
            }
            group.UpdateDate = DateTime.Now;
            return new RequestResponse<Group> { Item = group, Success = insertGroupId > -1 };
        }
    }
}
