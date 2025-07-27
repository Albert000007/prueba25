using Baylongo.Data.Data.MsSql.Contexts.SqlServer.BaylongoContext;
using BaylongoApi.DTOs.Dance;
using BaylongoApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;

namespace BaylongoApi.Services
{
    public class DanceService(BaylongoContext context) : IDanceService
    {
        public async Task<IEnumerable<DanceLevelDto>> GetAllDanceLevels()
        {
            return await context.DanceLevels
               .Where(dl => dl.IsActive)
               .OrderBy(dl => dl.Order)
               .Select(dl => new DanceLevelDto
               {
                   LevelId = dl.LevelId,
                   Name = dl.Name,
                   Description = dl.Description,
                   Order = dl.Order,
                   IsBeginnerFriendly = false // Este valor se debe obtener de DanceTypeLevels
               })
               .ToListAsync();
        }

        public async Task<DanceTypeDto> GetDanceTypeDetails(int danceTypeId)
        {

            var danceType = await context.DanceTypes
                .Include(dt => dt.DanceTypeLevels)
                    .ThenInclude(dtl => dtl.Level)
                .Include(dt => dt.Category)
                .FirstOrDefaultAsync(dt => dt.DanceTypeId == danceTypeId && dt.IsActive);

            if (danceType == null)
            {
                throw new KeyNotFoundException($"Dance type with id {danceTypeId} not found");
            }

            return new DanceTypeDto
            {
                DanceTypeId = danceType.DanceTypeId,
                Name = danceType.Name,
                Description = danceType.Description,
                IsActive = danceType.IsActive,
                Order = danceType.Order,
                Category = danceType.Category != null ? new DanceCategoryDto
                {
                    CategoryId = danceType.Category.CategoryId,
                    Name = danceType.Category.Name
                } : null,
                DanceTypeLevels = (ICollection<DanceTypeLevel>)danceType.DanceTypeLevels
                    .Where(dtl => dtl.Level.IsActive)
                    .OrderBy(dtl => dtl.Level.Order)
                    .Select(dtl => new DanceLevelDto
                    {
                        LevelId = dtl.LevelId,
                        Name = dtl.Level.Name,
                        Description = dtl.Level.Description,
                        Order = dtl.Level.Order,
                        IsBeginnerFriendly = dtl.IsBeginnerFriendly
                    })
                    .ToList()
            };
        }

        public async Task<IEnumerable<DanceTypeDto>> GetDanceTypesByCategory(int categoryId)
        {
            return await context.DanceTypes
                .Include(dt => dt.Category)
                .Where(dt => dt.CategoryId == categoryId && dt.IsActive)
                .OrderBy(dt => dt.Order)
                .Select(dt => new DanceTypeDto
                {
                    DanceTypeId = dt.DanceTypeId,
                    Name = dt.Name,
                    Description = dt.Description,
                    IsActive = dt.IsActive,
                    Order = dt.Order,
                    CategoryId = dt.CategoryId,
                    CategoryName = dt.Category != null ? dt.Category.Name : null
                })
                .ToListAsync();

        }

        public async Task<IEnumerable<DanceTypeWithLevelsDto>> GetDanceTypesWithLevels()
        {
            return await context.DanceTypes
                .Include(dt => dt.DanceTypeLevels)
                    .ThenInclude(dtl => dtl.Level)
                .Include(dt => dt.Category)
                .Where(dt => dt.IsActive)
                .OrderBy(dt => dt.Order)
                .Select(dt => new DanceTypeWithLevelsDto
                {
                    DanceTypeId = dt.DanceTypeId,
                    Name = dt.Name,
                    Description = dt.Description,
                    CategoryId = dt.CategoryId,
                    CategoryName = dt.Category != null ? dt.Category.Name : null,
                    AvailableLevels = dt.DanceTypeLevels
                        .Where(dtl => dtl.Level.IsActive)
                        .OrderBy(dtl => dtl.Level.Order)
                        .Select(dtl => new DanceLevelDto
                        {
                            LevelId = dtl.LevelId,
                            Name = dtl.Level.Name,
                            Description = dtl.Level.Description,
                            Order = dtl.Level.Order,
                            IsBeginnerFriendly = dtl.IsBeginnerFriendly
                        })
                        .ToList()
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<DanceLevelDto>> GetLevelsForDanceType(int danceTypeId)
        {
            return await context.DanceTypeLevels
                .Include(dtl => dtl.Level)
                .Where(dtl => dtl.DanceTypeId == danceTypeId && dtl.Level.IsActive)
                .OrderBy(dtl => dtl.Level.Order)
                .Select(dtl => new DanceLevelDto
                {
                    LevelId = dtl.LevelId,
                    Name = dtl.Level.Name,
                    Description = dtl.Level.Description,
                    Order = dtl.Level.Order,
                    IsBeginnerFriendly = dtl.IsBeginnerFriendly
                })
                .ToListAsync();
        }
    }
}
