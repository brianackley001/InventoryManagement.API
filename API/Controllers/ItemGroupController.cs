using Microsoft.AspNetCore.Mvc;
using InventoryManagement.API.Managers;
using System;
using System.Threading.Tasks;
using InventoryManagement.API.Models;
using NLog;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace InventoryManagement.API.Controllers
{
    [Produces("application/json")]
    [Route("itemGroups")]
    [ApiController, Authorize]
    public class ItemGroupController : ControllerBase
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IItemGroupManager _itemGroupManager;

        public ItemGroupController(IItemGroupManager itemGroupManager)
        {
            _itemGroupManager = itemGroupManager;
        }


        [HttpGet("{itemId}")]
        public async Task<IActionResult> GetItemGroups(int itemId)
        {
            try
            {
                var requestResponse = await _itemGroupManager.GetItemGroups(itemId).ConfigureAwait(true);
                return Ok(requestResponse);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                return StatusCode(500);
            }
        }


        [HttpPost("{subscriptionId}")]
        public async Task<IActionResult> UpsertItemGroups([FromBody] List<ItemAttributeTableValueParameter> itemTvp)
        {
            try
            {
                var requestResponse = await _itemGroupManager.UpsertItemGroups(itemTvp).ConfigureAwait(true);
                return Ok(requestResponse);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                return StatusCode(500);
            }
        }
    }
}