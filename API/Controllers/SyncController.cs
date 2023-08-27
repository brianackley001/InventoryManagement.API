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
    [Route("sync")]
    [ApiController, Authorize]
    public class SyncController : ControllerBase
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();
        private readonly IProfileSyncManager _profileSyncManager;


        public SyncController(IProfileSyncManager profileSyncManager)
        {
            _profileSyncManager = profileSyncManager;
        }

        [HttpPost()]
        public async Task<IActionResult> InitSession([FromBody] UserProfile userProfile)
        {
            try
            {
                var requestResponse = await _profileSyncManager.GetProfileSync(userProfile).ConfigureAwait(true);
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