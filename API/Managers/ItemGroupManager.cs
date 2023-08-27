using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryManagement.API.Models;
using InventoryManagement.API.DataProvider.SQL;
using NLog;

namespace InventoryManagement.API.Managers
{
    public class ItemGroupManager : IItemGroupManager
    {
        private readonly IItemGroupRepository _itemGroupRepository;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public ItemGroupManager(IItemGroupRepository itemGroupRepository)
        {
            _itemGroupRepository = itemGroupRepository;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "<Pending>")]
        public async Task<RequestResponse<List<ItemGroup>>> GetItemGroups(int itemId)
        {
            var itemGroups = new RequestResponse<List<ItemGroup>>();

            try
            {
                itemGroups = await _itemGroupRepository.GetItemGroups(itemId).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                itemGroups.Success = false;
            }
            return itemGroups;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "<Pending>")]
        public async Task<RequestResponse<bool>> UpsertItemGroups(List<ItemAttributeTableValueParameter> itemTvp)
        {
            var gResult = new RequestResponse<bool> { Success = false };

            try
            {
                gResult = await _itemGroupRepository.UpsertItemGroups(itemTvp);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
            }
            return gResult;
        }
    }
}
