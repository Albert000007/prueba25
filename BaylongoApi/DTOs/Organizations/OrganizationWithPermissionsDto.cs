namespace BaylongoApi.DTOs.Organizations
{
    public class OrganizationWithPermissionsDto
    {
        public int OrganizationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string LogoUrl { get; set; }
        public int OwnerId { get; set; }
        public string OwnerName { get; set; }
        public ContentTypeDto ContentType { get; set; }
        public DateTime? PermissionGrantedAt { get; set; }
    }
}
