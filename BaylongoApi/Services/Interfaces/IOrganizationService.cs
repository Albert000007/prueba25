using Baylongo.Data.Data.MsSql.Models.DBBaylongo;
using BaylongoApi.DTOs.Organizations;

namespace BaylongoApi.Services.Interfaces
{
    public interface IOrganizationService
    {
        Task<OrganizationDto> CreateOrganization(CreateOrganizationDto createDto);
        Task<OrganizationDto> GetOrganizationById(int id);
        Task<IEnumerable<OrganizationDto>> GetOrganizationsByUser(int userId);
        Task<PagedResponse<IEnumerable<OrganizationDetailDto>>> GetUserOrganizationHierarchy(
            int userId,
            int page = 1,
            int pageSize = 10,
            string search = "",
            int? organizationTypeId = null,
            int? verificationStatusId = null);
        Task<Organization> UpdateOrganization(int id, UpdateOrganizationDto updateDto);
        Task<bool> DeleteOrganization(int id);
        Task<OrganizationDto> UpdateVerificationStatus(int id, int statusId);
        Task<string> OrganizationLogoAsync(int organizationId, IFormFile logoFile);
        Task<bool> DeleteOrganization(int organizationId, int requestingUserId);
       
    }
}
