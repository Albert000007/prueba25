using Baylongo.Data.Data.MsSql.Contexts.SqlServer.BaylongoContext;
using Baylongo.Data.Data.MsSql.Models.DBBaylongo;
using BaylongoApi.DTOs.Dance;
using BaylongoApi.DTOs.Profile;
using BaylongoApi.Services.Email.Contracts;
using BaylongoApi.Services.Interfaces;
using BaylongoApi.Templates.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using System.Security.Claims;

namespace BaylongoApi.Services
{
    public class ProfileService(BaylongoContext context, IConfiguration configuration, IPasswordHasher passwordHash, ITemplateService templateService
, IHttpContextAccessor httpContextAccessor) : IProfileService
    {
        public async Task<List<UserDancePreferenceDto>> GetUserDancePreferencesAsync(int userId)
        {
            return await context.UserDancePreferences
                .Include(udp => udp.DanceType)
                 .ThenInclude(dt => dt.Category)
                 .Where(udp => udp.UserId == userId)
                 .Select(udp => new UserDancePreferenceDto
                 {
                     DanceTypeId = udp.DanceTypeId,
                     DanceName = udp.DanceType.Name,
                     DanceIcon = udp.DanceType.IconUrl,
                     PreferenceLevel = udp.PreferenceLevel,
                     LastUpdated = DateTime.UtcNow,
                     CategoryId = udp.DanceType.CategoryId ?? 0,
                     CategoryName = udp.DanceType.Category != null ? udp.DanceType.Category.Name : "N/A"
                 })
                 .ToListAsync();
        }

        public async Task<UserProfileDto> GetUserProfileAsync(int userId)
        {
            var user = await context.Users
           .Include(u => u.UserType)
           .Include(u => u.City)
           .Include(u => u.UserDancePreferences)
               .ThenInclude(udp => udp.DanceType)
                   .ThenInclude(dt => dt.Category)
           .FirstOrDefaultAsync(u => u.UserId == userId)
           ?? throw new Exception("User not found");

            return new UserProfileDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Phone = user.Phone,
                ProfilePictureUrl = user.ProfilePictureUrl,
                IsVerified = user.IsVerified,
                RegistrationDate = user.RegistrationDate,
                UserType = user.UserType?.TypeName ?? "N/A",
                CityId = user.CityId,
                CityName = user.City?.Name ?? "N/A",
                DancePreferences = user.UserDancePreferences.Select(udp => new UserDancePreferenceDto
                {
                    DanceTypeId = udp.DanceTypeId,
                    DanceName = udp.DanceType.Name,
                    DanceIcon = udp.DanceType.IconUrl,
                    PreferenceLevel = udp.PreferenceLevel,
                    LastUpdated = DateTime.UtcNow,
                    CategoryId = udp.DanceType.CategoryId ?? 0,
                    CategoryName = udp.DanceType.Category != null ? udp.DanceType.Category.Name : "N/A"
                }).ToList()
            };
        }

        public async Task UpdateUserDancePreferencesAsync(int userId, List<UserDancePreferenceDto> preferences)
        {
            foreach (var pref in preferences)
            {
                if (pref.PreferenceLevel is < 1 or > 5)
                    throw new ArgumentException($"El nivel de preferencia debe estar entre 1 y 5 para el tipo de baile. {pref.DanceTypeId}");

                var danceType = await context.DanceTypes
                    .Include(dt => dt.Category)
                    .FirstOrDefaultAsync(dt => dt.DanceTypeId == pref.DanceTypeId)
                    ?? throw new Exception($"Tipo de baile con identificación {pref.DanceTypeId} no encontrado");

                if (danceType.CategoryId != pref.CategoryId)
                    throw new Exception($"Tipo de baile {pref.DanceName} no pertenece a la categoría ID {pref.CategoryId}");
            }

            // Eliminar preferencias existentes
            var existingPrefs = await context.UserDancePreferences
                .Where(udp => udp.UserId == userId)
                .ToListAsync();

            context.UserDancePreferences.RemoveRange(existingPrefs);

            // Agregar nuevas preferencias
            var newPrefs = preferences.Select(p => new UserDancePreference
            {
                UserId = userId,
                DanceTypeId = p.DanceTypeId,
                PreferenceLevel = p.PreferenceLevel
            }).ToList();

            await context.UserDancePreferences.AddRangeAsync(newPrefs);
            await context.SaveChangesAsync();
        }

        public async Task<UserProfileDto> UpdateUserProfileAsync(int userId, UpdateUserProfileDto dto)
        {
            var user = await context.Users
          .Include(u => u.UserType)
          .FirstOrDefaultAsync(u => u.UserId == userId)
          ?? throw new Exception("User not found");

            if (!string.IsNullOrWhiteSpace(dto.Username) && dto.Username != user.Username)
            {
                var usernameExists = await context.Users.AnyAsync(u => u.Username == dto.Username && u.UserId != userId);
                if (usernameExists)
                    throw new Exception("Username is already taken");

                user.Username = dto.Username;
            }

            if (dto.CityId.HasValue)
            {
                var cityExists = await context.Cities.AnyAsync(c => c.CityId == dto.CityId.Value);
                if (!cityExists)
                    throw new Exception("City not found");

                user.CityId = dto.CityId.Value;
            }

            user.Phone = dto.Phone ?? user.Phone;

            await context.SaveChangesAsync();
            return await GetUserProfileAsync(userId);
        }

        public async Task<string> UploadProfilePictureAsync(int userId, IFormFile file)
        {

            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            var user = await context.Users.FirstOrDefaultAsync(u => u.UserId == userId)
                        ?? throw new Exception("User not found");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            // Obtener dominio dinámico desde el request actual
            var request = httpContextAccessor.HttpContext?.Request;
            var baseUrl = $"{request?.Scheme}://{request?.Host}";

            var imageUrl = $"{baseUrl}/uploads/{fileName}";

            user.ProfilePictureUrl = imageUrl;
            await context.SaveChangesAsync();

            return imageUrl;
        }
    }
}
