using Baylongo.Data.Data.MsSql.Contexts.SqlServer.BaylongoContext;
using Baylongo.Data.Data.MsSql.Models.DBBaylongo;
using BaylongoApi.DTOs.Dance;
using BaylongoApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BaylongoApi.Services
{
    public class DanceCategoryService(BaylongoContext context) : IDanceCategoryService
    {
        public async Task<DanceCategoryDto> CreateCategoryAsync(CreateDanceCategoryDto createDto)
        {
            var category = new DanceCategory
            {
                Name = createDto.Name,
                Description = createDto.Description,
                Order = createDto.Order,
                IsActive = createDto.IsActive
            };

            context.DanceCategories.Add(category);
            await context.SaveChangesAsync();

            return new DanceCategoryDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Description = category.Description,
                Order = category.Order,
                IsActive = category.IsActive
            };
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await context.DanceCategories.FindAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with id {id} not found");
            }

            // Verificar si hay tipos de baile asociados
            var hasDanceTypes = await context.DanceTypes
                .AnyAsync(dt => dt.CategoryId == id);

            if (hasDanceTypes)
            {
                throw new InvalidOperationException("Cannot delete category with associated dance types");
            }

            context.DanceCategories.Remove(category);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<DanceCategoryDto>> GetAllCategoriesAsync(bool includeDanceTypes = false)
        {
            var query = context.DanceCategories.AsQueryable();

            if (includeDanceTypes)
            {
                query = query.Include(c => c.DanceTypes)
                             .ThenInclude(dt => dt.DanceTypeLevels);
            }

            return await query
                .OrderBy(c => c.Order)
                .Select(c => new DanceCategoryDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    Description = c.Description,
                    Order = c.Order,
                    IsActive = c.IsActive,
                    DanceTypes = includeDanceTypes ?
                        c.DanceTypes
                         .Where(dt => dt.IsActive)
                         .OrderBy(dt => dt.Order)
                         .Select(dt => new DanceTypeDto
                         {
                             DanceTypeId = dt.DanceTypeId,
                             Name = dt.Name
                         })
                         .ToList() : new List<DanceTypeDto>()
                })
                .ToListAsync();
        }

        public async Task<DanceCategoryDto> GetCategoryByIdAsync(int id, bool includeDanceTypes = false)
        {
            var query = context.DanceCategories
            .Where(c => c.CategoryId == id);

            if (includeDanceTypes)
            {
                query = query.Include(c => c.DanceTypes)
                             .ThenInclude(dt => dt.DanceTypeLevels);
            }

            var category = await query.FirstOrDefaultAsync();

            if (category == null)
            {
                throw new KeyNotFoundException($"Category with id {id} not found");
            }

            return new DanceCategoryDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Description = category.Description,
                Order = category.Order,
                IsActive = category.IsActive,
                DanceTypes = includeDanceTypes ?
                    category.DanceTypes
                             .Where(dt => dt.IsActive)
                             .OrderBy(dt => dt.Order)
                             .Select(dt => new DanceTypeDto
                             {
                                 DanceTypeId = dt.DanceTypeId,
                                 Name = dt.Name,

                             })
                             .ToList() : new List<DanceTypeDto>()
            };
        }

        public async Task<DanceCategoryDto> UpdateCategoryAsync(int id, UpdateDanceCategoryDto updateDto)
        {
            var category = await context.DanceCategories.FindAsync(id) ?? throw new KeyNotFoundException($"Category with id {id} not found");
            category.Name = updateDto.Name;
            category.Description = updateDto.Description;
            category.Order = updateDto.Order;
            category.IsActive = updateDto.IsActive;

            await context.SaveChangesAsync();

            return new DanceCategoryDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                Description = category.Description,
                Order = category.Order,
                IsActive = category.IsActive
            };
        }
    }
}
