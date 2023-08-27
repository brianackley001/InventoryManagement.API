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
    [Route("profile")]
    [ApiController, Authorize]
    public class ProfileSubscriptionController : ControllerBase
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IProfileSubscriptionManager _profileSubscriptionManager;

        public ProfileSubscriptionController(IProfileSubscriptionManager profileSubscriptionManager)
        {
            _profileSubscriptionManager = profileSubscriptionManager;
        }


        [HttpGet("{authId}")]
        public async Task<IActionResult> GetProfileSubscriptions(string authId)
        {
            try
            {
                var requestResponse = await _profileSubscriptionManager.GetProfileSubscriptions(authId).ConfigureAwait(true);
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
        public async Task<IActionResult> UpsertProfile([FromBody] UserProfile userProfile)
        {
            try
            {
                var requestResponse = await _profileSubscriptionManager.UpsertProfile(userProfile).ConfigureAwait(true);
                return Ok(requestResponse);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                return StatusCode(500);
            }
        }

        [HttpPost("{profileId}/profileSubscription")]
        public async Task<IActionResult> UpsertProfileSubscription([FromBody] Subscription subscription)
        {
            try
            {
                var requestResponse = await _profileSubscriptionManager.UpsertProfileSubscription(subscription).ConfigureAwait(true);
                return Ok(requestResponse);
            }
            catch (Exception ex)
            {
                // Log Exception
                _logger.Log(LogLevel.Error, ex);
                return StatusCode(500);
            }
        }

        [HttpPost("{profileId}")]
        public async Task<IActionResult> UpsertSubscription([FromBody] Subscription subscription)
        {
            try
            {
                var requestResponse = await _profileSubscriptionManager.UpsertSubscription(subscription).ConfigureAwait(true);
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