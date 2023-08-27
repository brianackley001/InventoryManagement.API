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
    [Route("tags")]
    [ApiController, Authorize]
    public class TagController : ControllerBase
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly ITagManager _tagManager;

        public TagController(ITagManager tagManager)
        {
            _tagManager = tagManager;
        }

        [HttpGet("{subscriptionId}")]
        public async Task<IActionResult> GetTags(int subscriptionId)
        {
            try
            {
                var requestResponse = await _tagManager.GetTags(subscriptionId).ConfigureAwait(true);
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
        public async Task<IActionResult> GetTagsWithItemCounts(int subscriptionId, [FromBody] SearchRequest searchRequest)
        {
            try
            {
                var requestResponse = await _tagManager.GetTagCollectionWithItemCounts(subscriptionId, searchRequest).ConfigureAwait(true);
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
        public async Task<IActionResult> GetTagCollection(int subscriptionId, int pageNumber, int pageSize)
        {
            try
            {
                var requestResponse = await _tagManager.GetTagCollection(subscriptionId, pageNumber, pageSize).ConfigureAwait(true);
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
        public async Task<IActionResult> UpsertTag([FromBody] Tag tag)
        {
            try
            {
                var requestResponse = await _tagManager.UpsertTag(tag).ConfigureAwait(true);
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