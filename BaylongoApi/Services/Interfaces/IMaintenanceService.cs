using BaylongoApi.DTOs.Maintenance;

namespace BaylongoApi.Services.Interfaces
{
    public interface IMaintenanceService
    {
        Task<MaintenanceResponse> GetCurrentStatusAsync();
        Task<MaintenanceResponse> SetMaintenanceModeAsync(MaintenanceRequest request, int userId);
        Task<MaintenanceResponse> DisableMaintenanceModeAsync(int userId);
        Task<bool> IsMaintenanceActiveAsync();
    }
}
