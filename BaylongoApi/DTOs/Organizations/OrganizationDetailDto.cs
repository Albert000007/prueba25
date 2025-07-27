namespace BaylongoApi.DTOs.Organizations
{
    public class OrganizationDetailDto
    {
        public int OrganizationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string LogoUrl { get; set; }
        public string Website { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public bool IsBaseOrganization { get; set; }
        public int? BaseOrganizationId { get; set; }
        public string? BaseOrganizationName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Información del dueño
        public int OwnerId { get; set; }
        public string OwnerName { get; set; }
        public string OwnerEmail { get; set; }

        // Tipo de organización
        public int OrganizationTypeId { get; set; }
        public string OrganizationType { get; set; }

        // Estado de verificación
        public int VerificationStatusId { get; set; }
        public string VerificationStatus { get; set; }
    }
}
