using BaylongoApi.DTOs.Dance;
using BaylongoApi.DTOs.Profile;
using BaylongoApi.Services;
using BaylongoApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BaylongoApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ProfileController(IProfileService profileService) : ControllerBase
    {
        [HttpGet("user-profile")]
        public async Task<ActionResult<UserProfileDto>> GetProfile()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var profile = await profileService.GetUserProfileAsync(userId);
            return Ok(profile);
        }
        [HttpPut("update-user-profile")]
        public async Task<ActionResult<UserProfileDto>> UpdateProfile([FromBody] UpdateUserProfileDto dto)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var updated = await profileService.UpdateUserProfileAsync(userId, dto);
            return Ok(updated);
        }

        [HttpPost("profile-picture")]
        public async Task<IActionResult> UploadProfilePicture(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var imageUrl = await profileService.UploadProfilePictureAsync(userId, file);
            return Ok(new { profilePictureUrl = imageUrl });
        }

        [HttpGet("dance-preferences")]
        public async Task<ActionResult<List<UserDancePreferenceDto>>> GetUserDancePreferences()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var preferences = await profileService.GetUserDancePreferencesAsync(userId);
            return Ok(preferences);
        }
        [HttpPut("dance-preferences")]
        public async Task<ActionResult> UpdateUserDancePreferences(
                    [FromBody] UpdateUserDancePreferencesDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            await profileService.UpdateUserDancePreferencesAsync(userId, dto.Preferences);
            return NoContent();
        }
    }
}
