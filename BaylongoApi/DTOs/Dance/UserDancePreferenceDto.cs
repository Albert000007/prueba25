namespace BaylongoApi.DTOs.Dance
{
    public class UserDancePreferenceDto
    {
        public int DanceTypeId { get; set; }
        public string DanceName { get; set; } = string.Empty;
        public string? DanceIcon { get; set; }
        public int PreferenceLevel { get; set; } // 1-5
        public DateTime LastUpdated { get; set; }
        public int CategoryId { get; set; } // Nueva propiedad para la categoría
        public string CategoryName { get; set; } = string.Empty; // Nueva propiedad para el nombre de la categoría
    }
}
