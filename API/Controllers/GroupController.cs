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
    [Route("groups")]
    [ApiController, Authorize]
    public class GroupController : ControllerBase
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IGroupManager _groupManager;

        public GroupController(IGroupManager groupManager)
        {
            _groupManager = groupManager;
        }

        [HttpGet("{subscriptionId}")]
        public async Task<IActionResult> GetGroups(int subscriptionId)
        {
            try
            {
                var requestResponse = await _groupManager.GetGroups(subscriptionId).ConfigureAwait(true);
                return Ok(requestResponse);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                return StatusCode(500);
            }
        }


        [HttpPost("{subscriptionId}/manage")]
        public async Task<IActionResult> GetGroupsWithItemCounts(int subscriptionId, [FromBody] SearchRequest searchRequest)
        {
            try
            {
                var requestResponse = await _groupManager.GetGroupCollectionWithItemCounts(subscriptionId, searchRequest).ConfigureAwait(true);
                return Ok(requestResponse);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                return StatusCode(500);
            }
        }

        [HttpGet("{subscriptionId}/{pageNumber}/{pageSize}")]
        public async Task<IActionResult> GetGroupCollection(int subscriptionId, int pageNumber, int pageSize)
        {
            try
            {
                var requestResponse = await _groupManager.GetGroupCollection(subscriptionId, pageNumber, pageSize).ConfigureAwait(true);
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
        public async Task<IActionResult> UpsertGroup([FromBody] Group group)
        {
            try
            {
                var requestResponse = await _groupManager.UpsertGroup(group).ConfigureAwait(true);
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