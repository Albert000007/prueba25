using BaylongoApi.DTOs.City;
using BaylongoApi.DTOs.Dance;

namespace BaylongoApi.DTOs.Profile
{
    public class UserProfileDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public bool? IsVerified { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string UserType { get; set; } = string.Empty;
        // Nuevas propiedades para preferencias de baile
        public List<UserDancePreferenceDto> DancePreferences { get; set; } = new List<UserDancePreferenceDto>();
        public DateTime? LastDancePreferencesUpdate { get; set; }
        public int? CityId { get; set; } // Nueva propiedad
        public string? CityName { get; set; } // Nueva propiedad
    }
}
