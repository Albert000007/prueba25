using BaylongoApi.DTOs.Dance;

namespace BaylongoApi.Services.Interfaces
{
    public interface IDanceService
    {
        // Obtener todos los tipos de baile con sus niveles disponibles
        Task<IEnumerable<DanceTypeWithLevelsDto>> GetDanceTypesWithLevels();

        // Obtener niveles para un tipo de baile específico
        Task<IEnumerable<DanceLevelDto>> GetLevelsForDanceType(int danceTypeId);

        // Obtener todos los niveles de baile disponibles
        Task<IEnumerable<DanceLevelDto>> GetAllDanceLevels();

        // Obtener detalles completos de un tipo de baile
        Task<DanceTypeDto> GetDanceTypeDetails(int danceTypeId);

        // Obtener tipos de baile por categoría
        Task<IEnumerable<DanceTypeDto>> GetDanceTypesByCategory(int categoryId);
    }
}
