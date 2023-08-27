using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryManagement.API.Models;
using InventoryManagement.API.DataProvider.SQL;
using NLog;


namespace InventoryManagement.API.Managers
{
    public class ItemTagManager : IItemTagManager
    {
        private readonly IItemTagRepository _itemTagRepository;
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();


        public ItemTagManager(IItemTagRepository itemTagRepository)
        {
            _itemTagRepository = itemTagRepository;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "<Pending>")]
        public async Task<RequestResponse<List<ItemTag>>> GetItemTags(int itemId)
        {
            var itemTags = new RequestResponse<List<ItemTag>>();

            try
            {
                itemTags = await _itemTagRepository.GetItemTags(itemId).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                itemTags.Success = false;
            }
            return itemTags;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1854:Unused assignments should be removed", Justification = "<Pending>")]
        public async Task<RequestResponse<bool>> UpsertItemTags(List<ItemAttributeTableValueParameter> itemTvp)
        {
            var gResult = new RequestResponse<bool> { Success = false };

            try
            {
                gResult = await _itemTagRepository.UpsertItemTags(itemTvp);
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
