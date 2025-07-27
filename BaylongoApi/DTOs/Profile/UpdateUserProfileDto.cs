namespace BaylongoApi.DTOs.Profile
{
    public class UpdateUserProfileDto
    {
        public string? Username { get; set; }
        public string? Phone { get; set; }
        public int? CityId { get; set; } // Nueva propiedad
    }
}
