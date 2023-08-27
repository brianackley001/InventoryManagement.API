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
    [Route("itemTags")]
    [ApiController, Authorize]
    public class ItemTagController : ControllerBase
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IItemTagManager _itemTagManager;

        public ItemTagController(IItemTagManager itemTagManager)
        {
            _itemTagManager = itemTagManager;
        }


        [HttpGet("{itemId}")]
        public async Task<IActionResult> GetItemTags(int itemId)
        {
            try
            {
                var requestResponse = await _itemTagManager.GetItemTags(itemId).ConfigureAwait(true);
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
        public async Task<IActionResult> UpsertItemTags([FromBody] List<ItemAttributeTableValueParameter> itemTvp)
        {
            try
            {
                var requestResponse = await _itemTagManager.UpsertItemTags(itemTvp).ConfigureAwait(true);
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