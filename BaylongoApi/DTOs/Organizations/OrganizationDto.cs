namespace BaylongoApi.DTOs.Organizations
{
    public class OrganizationDto
    {
        public int OrganizationId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int OrganizationTypeId { get; set; }
        public string OrganizationType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string LogoUrl { get; set; }
        public string Website { get; set; }
        public string Phone { get; set; }
        public int? CityId { get; set; }
        public string? CityName { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? GoogleMapsUrl { get; set; }
        public int VerificationStatusId { get; set; }
        public string VerificationStatus { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
