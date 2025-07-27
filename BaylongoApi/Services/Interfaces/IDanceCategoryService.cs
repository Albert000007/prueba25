using BaylongoApi.DTOs.Dance;

namespace BaylongoApi.Services.Interfaces
{
    public interface IDanceCategoryService
    {
        Task<IEnumerable<DanceCategoryDto>> GetAllCategoriesAsync(bool includeDanceTypes = false);
        Task<DanceCategoryDto> GetCategoryByIdAsync(int id, bool includeDanceTypes = false);
        Task<DanceCategoryDto> CreateCategoryAsync(CreateDanceCategoryDto createDto);
        Task<DanceCategoryDto> UpdateCategoryAsync(int id, UpdateDanceCategoryDto updateDto);
        Task<bool> DeleteCategoryAsync(int id);
    }
}
