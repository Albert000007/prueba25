using System.ComponentModel.DataAnnotations;

namespace BaylongoApi.DTOs.Organizations
{
    public class UpdateOrganizationDto
    {
        [Required]
        public int OrganizationId { get; set; }
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Website { get; set; }

        [Phone]
        public string Phone { get; set; }
        public int? CityId { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? GoogleMapsUrl { get; set; }
        public int OrganizationTypeId { get; set; }
    }
}
