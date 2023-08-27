using Microsoft.AspNetCore.Mvc;
using InventoryManagement.API.Managers;
using System;
using System.Threading.Tasks;
using InventoryManagement.API.Models;
using NLog;
using Microsoft.AspNetCore.Authorization;

namespace InventoryManagement.API.Controllers
{
    [Produces("application/json")]
    [Route("items")]
    [ApiController, Authorize]
    public class ItemController : ControllerBase
    {

        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IItemManager _itemManager;


        public ItemController(IItemManager itemManager)
        {
            _itemManager = itemManager;
        }


        [HttpGet("{itemId}")]
        public async Task<IActionResult> GetItem(int itemId)
        {
            try
            {
                var requestResponse = await _itemManager.GetItem(itemId).ConfigureAwait(true);
                return Ok(requestResponse);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                return StatusCode(500);
            }
        }


        [HttpGet("stockItems")]
        public async Task<IActionResult> GetStockItems()
        {
            try
            {
                var requestResponse = await _itemManager.GetStockItems().ConfigureAwait(true);
                return Ok(requestResponse);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                return StatusCode(500);
            }
        }

        [HttpPost("{subscriptionId}/items")]
        public async Task<IActionResult> GetItemCollection(int subscriptionId, [FromBody] SearchRequest searchRequest)
        {
            try
            {
                var requestResponse = await _itemManager.GetItemCollection(subscriptionId, searchRequest).ConfigureAwait(true);
                return Ok(requestResponse);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                return StatusCode(500);
            }
        }

        [HttpPost("{subscriptionId}/itemIdCollection")]
        public async Task<IActionResult> GetItemByIdCollection(int subscriptionId, [FromBody] SearchRequest searchRequest)
        {
            try
            {
                var requestResponse = await _itemManager.GetItemsByIdCollection(subscriptionId, searchRequest).ConfigureAwait(true);
                return Ok(requestResponse);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                return StatusCode(500);
            }
        }

        [HttpPost("{subscriptionId}/lowquantity")]
        public async Task<IActionResult> GetLowQuantityItemCollection(int subscriptionId, [FromBody] SearchRequest searchRequest)
        {
            try
            {
                var requestResponse = await _itemManager.GetLowQuantityItemCollection(subscriptionId, searchRequest).ConfigureAwait(true);
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
        public async Task<IActionResult> UpsertItem([FromBody] Item item)
        {
            try
            {
                var requestResponse = await _itemManager.UpsertItem(item).ConfigureAwait(true);
                if (requestResponse.Success)
                {
                    requestResponse = await _itemManager.GetItem(item.Id);
                }
                return Ok(requestResponse);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                return StatusCode(500);
            }
        }

        [HttpPost("stockItems")]
        public async Task<IActionResult> ImportStockItems([FromBody] StockItemImport stockItemImport)
        {
            try
            {
                var requestResponse = await _itemManager.UpsertStockItems(stockItemImport).ConfigureAwait(true);
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