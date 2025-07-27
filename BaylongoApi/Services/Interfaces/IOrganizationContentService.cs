using BaylongoApi.DTOs.Organizations;

namespace BaylongoApi.Services.Interfaces
{
    public interface IOrganizationContentService
    {
        Task<List<ContentTypeDto>> GetAllContentTypes();
        Task<List<OrganizationContentPermissionDto>> GetOrganizationPermissions(int organizationId);
        Task<bool> UpdatePermission(int organizationId, int contentTypeId, bool isEnabled);
        Task<bool> CanPublishContent(int organizationId, int contentTypeId);
        Task<bool> DeletePermission(int organizationId, int contentTypeId);
        Task<List<OrganizationWithPermissionsDto>> GetOrganizationsByContentTypeAsync(int contentTypeId);
        Task<List<OrganizationWithContentTypesDto>> GetOrganizationsWithContentTypesAsync(List<int> contentTypeIds);
    }
}
