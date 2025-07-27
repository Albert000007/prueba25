using Baylongo.Data.Data.MsSql.Contexts.SqlServer.BaylongoContext;
using Baylongo.Data.Data.MsSql.Models.DBBaylongo;
using BaylongoApi.DTOs.Organizations;
using BaylongoApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using SendGrid.Helpers.Mail;
using System;

namespace BaylongoApi.Services
{
    public class OrganizationService(BaylongoContext context, ILogger<OrganizationService> _logger, IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor, ILocationUrlService locationUrlService) : IOrganizationService
    {
        public async Task<OrganizationDto> CreateOrganization(CreateOrganizationDto createDto)
        {
            // Validar nombre de organización único (case-insensitive)
            var org = await context.Organizations.ToListAsync();
            var orgs = org
                .FirstOrDefault(o => string.Equals(o.Name, createDto.Name, StringComparison.OrdinalIgnoreCase));

            if (orgs != null)
            {
                throw new ArgumentException("Ya existe una organización con ese nombre. Por favor, elige otro nombre.");
            }

            // Validar que el usuario existe
            var user = await context.Users.FindAsync(createDto.UserId);
            if (user == null)
            {
                throw new KeyNotFoundException("Usuario no encontrado");
            }

            // Validar tipo de organización
            var orgType = await context.OrganizationTypes.FindAsync(createDto.OrganizationTypeId);
            if (orgType == null)
            {
                throw new KeyNotFoundException("Tipo de organización no válido");
            }

            // Determinar si es la primera organización del usuario
            var isFirstOrganization = !await context.Organizations
                .AnyAsync(o => o.UserId == createDto.UserId);

            // Validaciones para organizaciones sucursales
            if (createDto.IsBranch)
            {
                if (!createDto.BaseOrganizationId.HasValue)
                {
                    throw new ArgumentException("Se requiere especificar la organización base para sucursales");
                }

                var baseOrg = await context.Organizations
                    .FirstOrDefaultAsync(o => o.OrganizationId == createDto.BaseOrganizationId &&
                                            o.UserId == createDto.UserId);

                if (baseOrg == null)
                {
                    throw new ArgumentException("La organización base especificada no existe o no pertenece al usuario");
                }

                if (!baseOrg.IsBaseOrganization)
                {
                    throw new ArgumentException("La organización especificada no está configurada como base");
                }
            }

            // Crear la nueva organización
            var organization = new Organization
            {
                UserId = createDto.UserId,
                OrganizationTypeId = createDto.OrganizationTypeId,
                Name = createDto.Name,
                Description = createDto.Description,
                Website = createDto.Website,
                Phone = createDto.Phone,
                CityId = createDto.CityId,
                Latitude = createDto.Latitude,
                Longitude = createDto.Longitude,
                VerificationStatusId = 1, // Pending por defecto
                IsBaseOrganization = !createDto.IsBranch && isFirstOrganization,
                BaseOrganizationId = createDto.IsBranch ? createDto.BaseOrganizationId : null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            if (createDto.Latitude.HasValue && createDto.Longitude.HasValue)
            {
                organization.GoogleMapsUrl = locationUrlService.GenerateGoogleMapsUrl(
                    createDto.Latitude.Value,
                    createDto.Longitude.Value);
            }
            // Actualizar el tipo de usuario a Organizador (2)
            user.UserTypeId = 2;

            // Crear la relación usuario-organización
       

            // Usar transacción para asegurar consistencia
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                await context.Organizations.AddAsync(organization);
                //await context.UserOrganizations.AddAsync(userOrg);
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            _logger.LogInformation(
                "Nueva organización {OrganizationType} creada: {OrganizationName} por el usuario {UserId}",
                organization.IsBaseOrganization ? "base" : "sucursal",
                organization.Name,
                createDto.UserId);

            var userOrg = new UserOrganization
            {
                UserId = createDto.UserId,
                OrganizationId = organization.OrganizationId,
                RoleId = 1, // Rol de administrador
                JoinDate = DateTime.UtcNow,
                IsActive = true
            };
            await context.UserOrganizations.AddAsync(userOrg);
            await context.SaveChangesAsync();

            return await GetOrganizationById(organization.OrganizationId);
        }
        public Task<bool> DeleteOrganization(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedResponse<IEnumerable<OrganizationDetailDto>>> GetUserOrganizationHierarchy(
    int userId,
    int page = 1,
    int pageSize = 10,
    string search = "",
    int? organizationTypeId = null,
    int? verificationStatusId = null)
        {
            // Obtener todas las organizaciones del usuario (bases y sucursales)
            var query = context.Organizations
                .Where(o => o.UserId == userId ||
                           context.Organizations
                               .Where(b => b.UserId == userId && b.IsBaseOrganization)
                               .Any(b => b.OrganizationId == o.BaseOrganizationId))
                .Include(o => o.User)
                .Include(o => o.OrganizationType)
                .Include(o => o.VerificationStatus)
                .Include(o => o.BaseOrganization) // Incluir la organización base para sucursales
                .Where(o => o.VerificationStatusId != 4) // Excluir eliminadas
                .AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(o =>
                o.Name.Contains(search) ||
                o.Description.Contains(search));
            }
            if (organizationTypeId.HasValue)
            {
                query = query.Where(o => o.OrganizationTypeId == organizationTypeId.Value);
            }

            if (verificationStatusId.HasValue)
            {
                query = query.Where(o => o.VerificationStatusId == verificationStatusId.Value);
            }


            // Ordenar: primero las bases, luego las sucursales agrupadas por su base
            var orderedQuery = query
                .OrderByDescending(o => o.IsBaseOrganization)
                .ThenBy(o => o.BaseOrganizationId)
                .ThenByDescending(o => o.CreatedAt);


            // Paginación
            var totalRecords = await query.CountAsync();

            var organizations = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new OrganizationDetailDto
                {
                    OrganizationId = o.OrganizationId,
                    Name = o.Name,
                    Description = o.Description,
                    LogoUrl = o.LogoUrl,
                    Website = o.Website,
                    Phone = o.Phone,
                    CreatedAt = o.CreatedAt,
                    UpdatedAt = o.UpdatedAt,
                    OwnerId = o.UserId,
                    OwnerName = o.User.Username,
                    OwnerEmail = o.User.Email,
                    OrganizationTypeId = o.OrganizationTypeId,
                    OrganizationType = o.OrganizationType.TypeName,
                    VerificationStatusId = o.VerificationStatusId,
                    VerificationStatus = o.VerificationStatus.StatusName,
                    IsBaseOrganization = o.IsBaseOrganization,
                    BaseOrganizationId = o.BaseOrganizationId,
                    BaseOrganizationName = o.BaseOrganization != null ? o.BaseOrganization.Name : null,
                })
                .ToListAsync();

            // Agrupar sucursales bajo sus bases (opcional, para estructura jerárquica)
            var result = organizations.GroupBy(o => o.IsBaseOrganization ? o.OrganizationId : o.BaseOrganizationId)
                .SelectMany(g =>
                {
                    var baseOrg = organizations.FirstOrDefault(o => o.OrganizationId == g.Key && o.IsBaseOrganization);
                    if (baseOrg != null)
                    {
                        return new[] { baseOrg }.Concat(g.Where(o => !o.IsBaseOrganization));
                    }
                    return g.AsEnumerable();
                })
                .ToList();

            var metadata = new PaginationMetadata
            {
                TotalCount = totalRecords,
                PageSize = pageSize,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize)
            };

            return new PagedResponse<IEnumerable<OrganizationDetailDto>>(result, metadata);
        }

        public async Task<OrganizationDto> GetOrganizationById(int id)
        {
            var organization = await context.Organizations
               .Include(o => o.User)
               .Include(o => o.OrganizationType)
               .Include(o => o.City)
               .Include(o => o.VerificationStatus)
               .FirstOrDefaultAsync(o => o.OrganizationId == id);

            if (organization == null)
            {
                throw new KeyNotFoundException("Organización no encontrada");
            }

            return MapToDto(organization);
        }

        public async Task<IEnumerable<OrganizationDto>> GetOrganizationsByUser(int userId)
        {
            return await context.Organizations
                .Where(o => o.UserId == userId)
                .Include(o => o.OrganizationType)
                .Include(o => o.VerificationStatus)
                .Select(o => MapToDto(o))
                .ToListAsync();
        }

        public async Task<Organization> UpdateOrganization(int id, UpdateOrganizationDto updateDto)
        {
            var organization = await context.Organizations.FindAsync(updateDto.OrganizationId);

            if (organization == null)
            {
                throw new KeyNotFoundException("Organization not found");
            }

            organization.OrganizationTypeId = updateDto.OrganizationTypeId;
            organization.Name = updateDto.Name;
            organization.Description = updateDto.Description;
            organization.Website = updateDto.Website;
            organization.Phone = updateDto.Phone;
            organization.CityId = updateDto.CityId;
            organization.Latitude = updateDto.Latitude;
            organization.Longitude = updateDto.Longitude;
            organization.GoogleMapsUrl = updateDto.GoogleMapsUrl;
            organization.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
            return organization;
        }

        public async Task<OrganizationDto> UpdateVerificationStatus(int id, int statusId)
        {
            var organization = await context.Organizations.FindAsync(id);
            if (organization == null)
            {
                throw new KeyNotFoundException("Organización no encontrada");
            }

            var status = await context.VerificationStatuses.FindAsync(statusId);
            if (status == null)
            {
                throw new KeyNotFoundException("Estado de verificación no válido");
            }

            organization.VerificationStatusId = statusId;
            organization.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return await GetOrganizationById(id);
        }
        private OrganizationDto MapToDto(Organization organization)
        {
            return new OrganizationDto
            {
                OrganizationId = organization.OrganizationId,
                UserId = organization.UserId,
                UserName = organization.User?.Username,
                OrganizationTypeId = organization.OrganizationTypeId,
                OrganizationType = organization.OrganizationType?.TypeName,
                Name = organization.Name,
                Description = organization.Description,
                LogoUrl = organization.LogoUrl,
                Website = organization.Website,
                Phone = organization.Phone,
                CityId = organization.CityId,
                CityName = organization.City?.Name,
                Latitude = organization.Latitude,
                Longitude = organization.Longitude,
                GoogleMapsUrl = organization.GoogleMapsUrl,
                VerificationStatus = organization.VerificationStatus?.StatusName,
                CreatedAt = organization.CreatedAt,
                UpdatedAt = organization.UpdatedAt,
                
                
            };
        }

        public async Task<string> OrganizationLogoAsync(int organizationId, IFormFile logoFile)
        {
            var organization = await context.Organizations.FindAsync(organizationId) ?? throw new KeyNotFoundException("Organization not found");

            // Validar el archivo
            if (logoFile == null || logoFile.Length == 0)
            {
                throw new ArgumentException("No file uploaded");
            }

            if (logoFile.Length > 5 * 1024 * 1024) // 5MB máximo
            {
                throw new ArgumentException("File size exceeds the limit (5MB)");
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(logoFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new ArgumentException("Invalid file type. Only JPG, JPEG, PNG, and GIF are allowed.");
            }

            // Crear directorio si no existe
            var uploadsFolder = Path.Combine(environment.WebRootPath, "uploads", "logos");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Generar nombre único para el archivo
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Guardar el archivo
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await logoFile.CopyToAsync(stream);
            }

            // Construir la URL
            var request = httpContextAccessor?.HttpContext?.Request;
            var logoUrl = $"{request?.Scheme}://{request?.Host}/uploads/logos/{uniqueFileName}";

            // Actualizar la organización
            organization.LogoUrl = logoUrl;
            organization.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();

            return logoUrl;
        }

        public async Task<bool> DeleteOrganization(int organizationId, int requestingUserId)
        {
            // Obtener la organización con sus relaciones
            var organization = await context.Organizations
                .Include(o => o.UserOrganizations)
                .FirstOrDefaultAsync(o => o.OrganizationId == organizationId) ?? throw new KeyNotFoundException("Organización no encontrada");

            // Verificar permisos: solo el dueño o admin puede eliminar
            var isOwner = organization.UserId == requestingUserId;
            var isAdmin = await context.Users
                .Where(u => u.UserId == requestingUserId)
                .Select(u => u.UserType.TypeName == "Administrador")
                .FirstOrDefaultAsync();

            if (!isOwner && !isAdmin)
            {
                throw new UnauthorizedAccessException("No tienes permisos para eliminar esta organización");
            }

            // Cambiar el estado a "deleted" (ID = 4) en lugar de borrado físico
            organization.VerificationStatusId = 4; // deleted
            organization.UpdatedAt = DateTime.UtcNow;

            // Desactivar todas las relaciones de usuarios con esta organización
            foreach (var userOrg in organization.UserOrganizations)
            {
                userOrg.IsActive = false;
            }

            await context.SaveChangesAsync();
            return true;
        }
    }
}
