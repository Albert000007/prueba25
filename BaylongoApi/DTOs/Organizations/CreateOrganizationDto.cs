using System.ComponentModel.DataAnnotations;

namespace BaylongoApi.DTOs.Organizations
{
    public class CreateOrganizationDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int OrganizationTypeId { get; set; }

        [Required]
        public bool IsBranch { get; set; }

        [Required]
        public int? BaseOrganizationId { get; set; }

        [Required]
        [StringLength(100)]
        public string? Name { get; set; }

        public string? Description { get; set; }


        public string? Website { get; set; }

        [Required]
        [Phone]
        public string? Phone { get; set; }

        public int? CityId { get; set; }

        [Range(-90, 90, ErrorMessage = "La latitud debe estar entre -90 y 90")]
        public decimal? Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "La longitud debe estar entre -180 y 180")]
        public decimal? Longitude { get; set; }


    }
}
