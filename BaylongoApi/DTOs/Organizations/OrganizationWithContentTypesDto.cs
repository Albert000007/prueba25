namespace BaylongoApi.DTOs.Organizations
{
    public class OrganizationWithContentTypesDto
    {
        public OrganizationDto Organization { get; set; }
        public List<ContentTypePermissionDto> AllowedContentTypes { get; set; }
    }
}
