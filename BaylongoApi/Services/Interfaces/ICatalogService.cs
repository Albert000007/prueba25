using BaylongoApi.DTOs.Catalogs;
using BaylongoApi.DTOs.City;
using BaylongoApi.DTOs.Dance;
using BaylongoApi.DTOs.Organizations;
using BaylongoApi.DTOs.Users;
using System.Threading.Tasks;

namespace BaylongoApi.Services.Interfaces
{
    public interface ICatalogService
    {
        Task<IEnumerable<OrganizationTypeDto>> GetOrganizationTypes();
        Task<IEnumerable<VerificationStatusDto>> GetVerificationStatuses();
        Task<IEnumerable<DTOs.Catalogs.UserTypeDto>> GetUserTypes();
        Task<IEnumerable<CatalogDto>> GetAllCatalogs();
        Task<IEnumerable<DanceTypeDto>> GetDanceTypes(); 
        Task<IEnumerable<CityDto>> GetCities();
        Task<IEnumerable<DanceTypeWithLevelsDto>> GetDanceTypesWithLevels();
    }
}
