using BaylongoApi.DTOs.Dance;
using BaylongoApi.DTOs.Profile;

namespace BaylongoApi.Services.Interfaces
{
    public interface IProfileService
    {
        Task<UserProfileDto> GetUserProfileAsync(int userId);
        Task<UserProfileDto> UpdateUserProfileAsync(int userId, UpdateUserProfileDto dto);
        Task<string> UploadProfilePictureAsync(int userId, IFormFile file);
        Task<List<UserDancePreferenceDto>> GetUserDancePreferencesAsync(int userId);
        Task UpdateUserDancePreferencesAsync(int userId, List<UserDancePreferenceDto> preferences);
    }
}
