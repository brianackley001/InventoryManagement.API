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
    [Route("search")]
    [ApiController, Authorize]
    public class SearchController : ControllerBase
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly ISearchManager _searchManager;

        public SearchController(ISearchManager searchManager)
        {
            _searchManager = searchManager;
        }


        [HttpPost("{subscriptionId}")]
        public async Task<IActionResult> GetSearchResultCollection(int subscriptionId, [FromBody] SearchRequest request)
        {
            try
            {
                var requestResponse = await _searchManager.GetSearchResultCollection(subscriptionId, request).ConfigureAwait(true);
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
        public async Task<IActionResult> GetLowQuanityItems(int subscriptionId, [FromBody] SearchRequest request)
        {
            try
            {
                var requestResponse = await _searchManager.GetLowQuantityItemCollection(subscriptionId, request).ConfigureAwait(true);
                return Ok(requestResponse);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                return StatusCode(500);
            }
        }

        [HttpPost("{subscriptionId}/groups")]
        public async Task<IActionResult> GetItemsByGroupCollection(int subscriptionId, [FromBody] SearchRequest request)
        {
            try
            {
                var requestResponse = await _searchManager.GetItemsByGroupCollection(subscriptionId, request).ConfigureAwait(true);
                return Ok(requestResponse);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                return StatusCode(500);
            }
        }

        [HttpPost("{subscriptionId}/tags")]
        public async Task<IActionResult> GetItemsByTagCollection(int subscriptionId, [FromBody] SearchRequest request)
        {
            try
            {
                var requestResponse = await _searchManager.GetItemsByTagCollection(subscriptionId, request).ConfigureAwait(true);
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