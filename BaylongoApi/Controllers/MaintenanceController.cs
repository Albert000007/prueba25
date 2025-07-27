using BaylongoApi.DTOs.Maintenance;
using BaylongoApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BaylongoApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class MaintenanceController(
        IMaintenanceService maintenanceService,
        ILogger<MaintenanceController> logger) : ControllerBase
    {

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<MaintenanceResponse>> GetCurrentStatus()
        {
            try
            {
                var status = await maintenanceService.GetCurrentStatusAsync();
                return Ok(status);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving maintenance status");
                return StatusCode(500, "Error retrieving maintenance status");
            }
        }
        [HttpPost]
        public async Task<ActionResult<MaintenanceResponse>> SetMaintenanceMode(
      [FromBody] MaintenanceRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var result = await maintenanceService.SetMaintenanceModeAsync(request, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error setting maintenance mode");
                return StatusCode(500, "Error setting maintenance mode");
            }
        }
        [HttpDelete]
        public async Task<IActionResult> DisableMaintenanceMode()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                await maintenanceService.DisableMaintenanceModeAsync(userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error disabling maintenance mode");
                return StatusCode(500, "Error disabling maintenance mode");
            }
        }

    }
}
