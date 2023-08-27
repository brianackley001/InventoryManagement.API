using Microsoft.AspNetCore.Mvc;
using InventoryManagement.API.Managers;
using System;
using System.Threading.Tasks;
using InventoryManagement.API.Models;
using NLog;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace InventoryManagement.API.Controllers
{
    [Produces("application/json")]
    [Route("shoppingList")]
    [ApiController, Authorize]
    public class ShoppingListController : ControllerBase
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IShoppingListManager _shoppingListManager;

        public ShoppingListController(IShoppingListManager shoppingListManager)
        {
            _shoppingListManager = shoppingListManager;
        }


        [HttpGet("{subscriptionId}/lists/{pageNumber}/{pageSize}")]
        public async Task<IActionResult> GetShoppingLists(int subscriptionId, int pageNumber, int pageSize)
        {
            try
            {
                var requestResponse = await _shoppingListManager.GetShoppingListCollection(subscriptionId, pageNumber, pageSize).ConfigureAwait(true);
                return Ok(requestResponse);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                return StatusCode(500);
            }
        }

        [HttpPost("{subscriptionId}/lists")]
        public async Task<IActionResult> GetShoppingListsWithItemCounts(int subscriptionId, [FromBody] SearchRequest request)
        {
            try
            {
                var requestResponse = await _shoppingListManager.GetShoppingListCollectionWithItemCounts(subscriptionId, request).ConfigureAwait(true);
                return Ok(requestResponse);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                return StatusCode(500);
            }
        }

        [HttpPost("{subscriptionId}/listItems/{shoppingListId}")]
        public async Task<IActionResult> GetShoppingListItems(int subscriptionId, int shoppingListId, [FromBody] SearchRequest request)
        {
            try
            {
                var requestResponse = await _shoppingListManager.GetShoppingListItems(subscriptionId, shoppingListId, request).ConfigureAwait(true);
                return Ok(requestResponse);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                return StatusCode(500);
            }
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> ShoppingListCheckout([FromBody] List<ShoppingListTableValueParameter> tvp)
        {
            try
            {
                var requestResponse = await _shoppingListManager.ShoppingListCheckout(tvp).ConfigureAwait(true);
                return Ok(requestResponse);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                return StatusCode(500);
            }
        }

        [HttpPost("init-checkout-sync")]
        public async Task<IActionResult> InitShoppingListCheckout([FromBody] List<ShoppingListTableValueParameter> tvp)
        {
            try
            {
                var requestResponse = await _shoppingListManager.InitShoppingListCheckout(tvp).ConfigureAwait(true);
                return Ok(requestResponse);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                return StatusCode(500);
            }
        }

        [HttpPost("checkout-sync-item")]
        public async Task<IActionResult> SyncShoppingListCheckoutItemStatus([FromBody] ShoppingListItem item)
        {
            try
            {
                var requestResponse = await _shoppingListManager.UpdateShoppingListItemCheckoutStatus(item).ConfigureAwait(true);
                return Ok(requestResponse);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                return StatusCode(500);
            }
        }
        [HttpPost("checkout-sync-list/{shoppingListId}")]
        public async Task<IActionResult> SyncShoppingListCheckoutCollection(int shoppingListId)
        {
            try
            {
                var requestResponse = await _shoppingListManager.SyncShoppingListItems(shoppingListId).ConfigureAwait(true);
                return Ok(requestResponse);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                return StatusCode(500);
            }
        }

        [HttpPost()]
        public async Task<IActionResult> UpsertShoppingList([FromBody] ShoppingList shoppingList)
        {
            try
            {
                var requestResponse = await _shoppingListManager.UpsertShoppingList(shoppingList).ConfigureAwait(true);
                return Ok(requestResponse);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                return StatusCode(500);
            }
        }

        [HttpPost("listItems/{shoppingListId}")]
        public async Task<IActionResult> UpsertShoppingListItems(int shoppingListId, [FromBody] List<ShoppingListTableValueParameter> itemTvp)
        {
            try
            {
                var requestResponse = await _shoppingListManager.UpsertShoppingListItems(shoppingListId, itemTvp).ConfigureAwait(true);
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