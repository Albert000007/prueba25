using Baylongo.Data.Data.MsSql.Contexts.SqlServer.BaylongoContext;
using Baylongo.Data.Data.MsSql.Models.DBBaylongo;
using BaylongoApi.DTOs.Maintenance;
using BaylongoApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;

namespace BaylongoApi.Services
{
    // Services/MaintenanceService.cs
    public class MaintenanceService(BaylongoContext context, ILogger<MaintenanceService> logger) : IMaintenanceService
    {
        public async Task<MaintenanceResponse> GetCurrentStatusAsync()
        {
            var maintenance = await context.SystemMaintenances
               .Include(m => m.CreatedByNavigation)
               .OrderByDescending(m => m.MaintenanceId)
               .AsNoTracking()
               .FirstOrDefaultAsync();

            if (maintenance == null)
            {
                return new MaintenanceResponse { IsActive = false };
            }

            return new MaintenanceResponse
            {
                IsActive = maintenance.IsActive,
                Message = maintenance.Message,
                StartTime = maintenance.StartTime,
                EndTime = maintenance.EndTime,
            };
        }

        public async Task<MaintenanceResponse> SetMaintenanceModeAsync(MaintenanceRequest request, int userId)
        {
            // Validación de parámetros
            if (request == null)
                throw new ArgumentNullException(nameof(request), "La solicitud de mantenimiento no puede ser nula");

            if (userId <= 0)
                throw new ArgumentException("ID de usuario inválido", nameof(userId));

            // Verificar si el usuario existe
            var userExists = await context.Users
                .AsNoTracking()
                .AnyAsync(u => u.UserId == userId);

            if (!userExists)
                throw new KeyNotFoundException($"No se encontró el usuario con ID {userId}");

            // Crear el registro de mantenimiento
            var maintenance = new SystemMaintenance
            {
                IsActive = request.IsActive,
                Message = !string.IsNullOrWhiteSpace(request.Message) ? request.Message.Trim() : null,
                EndTime = request.EndTime.HasValue ?
                    DateTime.SpecifyKind(request.EndTime.Value, DateTimeKind.Utc) :
                    null,
                CreatedBy = userId,
                StartTime = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                // Usar el DbSet correcto según tu modelo
                context.SystemMaintenances.Add(maintenance);
                await context.SaveChangesAsync();

                logger.LogInformation("Modo mantenimiento {Estado} configurado por el usuario {UserId}. Mensaje: {Mensaje}",
                    request.IsActive ? "activado" : "desactivado",
                    userId,
                    request.Message ?? "Sin mensaje adicional");
            }
            catch (DbUpdateException dbEx)
            {
                logger.LogError(dbEx, "Error de base de datos al configurar el modo mantenimiento");
                throw new ApplicationException("Error al guardar el registro de mantenimiento en la base de datos", dbEx);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error inesperado al configurar el modo mantenimiento");
                throw;
            }

            return await GetCurrentStatusAsync();
        }
        public async Task<MaintenanceResponse> DisableMaintenanceModeAsync(int userId)
        {
            var request = new MaintenanceRequest
            {
                IsActive = false,
                Message = "Maintenance manually disabled"
            };

            return await SetMaintenanceModeAsync(request, userId);
        }

        public async Task<bool> IsMaintenanceActiveAsync()
        {
            var maintenance = await context.SystemMaintenances
                .OrderByDescending(m => m.MaintenanceId)
                .FirstOrDefaultAsync();

            if (maintenance == null || !maintenance.IsActive)
            {
                return false;
            }

            // Verificar si el mantenimiento ha expirado
            if (maintenance.EndTime.HasValue && maintenance.EndTime.Value < DateTime.UtcNow)
            {
                // Auto-desactivar
                await DisableMaintenanceModeAsync(maintenance.CreatedBy ?? 0);
                return false;
            }

            return true;
        }
    }
}
