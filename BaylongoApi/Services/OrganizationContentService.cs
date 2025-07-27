using Baylongo.Data.Data.MsSql.Contexts.SqlServer.BaylongoContext;
using Baylongo.Data.Data.MsSql.Models.DBBaylongo;
using BaylongoApi.DTOs.Organizations;
using BaylongoApi.DTOs.Users;
using BaylongoApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;

namespace BaylongoApi.Services
{
    public class OrganizationContentService(BaylongoContext context) : IOrganizationContentService
    {
        public async Task<bool> CanPublishContent(int organizationId, int contentTypeId)
        {
            // Verificar si el tipo de contenido existe
            var contentTypeExists = await context.ContentTypes
                .AnyAsync(ct => ct.ContentTypeId == contentTypeId);

            if (!contentTypeExists)
            {
                return false;
            }

            // Buscar permiso explícito en OrganizationContentPermissions
            var permission = await context.OrganizationContentPermissions
                .FirstOrDefaultAsync(p => p.OrganizationId == organizationId
                                      && p.ContentTypeId == contentTypeId);

            // Si no existe permiso explícito, se deniega por defecto
            return permission?.IsEnabled ?? false;
        }

        public async Task<bool> DeletePermission(int organizationId, int contentTypeId)
        {
            var permission = await context.OrganizationContentPermissions
        .FirstOrDefaultAsync(p => p.OrganizationId == organizationId
                              && p.ContentTypeId == contentTypeId);

            if (permission == null)
            {
                return false;
            }

            context.OrganizationContentPermissions.Remove(permission);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ContentTypeDto>> GetAllContentTypes()
        {
            return await context.ContentTypes
            .Where(ct => ct.IsActive)
            .Select(ct => new ContentTypeDto
            {
                ContentTypeId = ct.ContentTypeId,
                Name = ct.Name,
                Description = ct.Description
            })
            .ToListAsync();
        }

        public async Task<List<OrganizationContentPermissionDto>> GetOrganizationPermissions(int organizationId)
        {
            return await context.OrganizationContentPermissions
            .Include(p => p.ContentType)
            .Where(p => p.OrganizationId == organizationId)
            .Select(p => new OrganizationContentPermissionDto
            {
                ContentTypeId = p.ContentTypeId,
                ContentTypeName = p.ContentType.Name,
                IsEnabled = p.IsEnabled,
                UpdatedAt = p.UpdatedAt
            })
            .ToListAsync();
        }

        public async Task<List<OrganizationWithPermissionsDto>> GetOrganizationsByContentTypeAsync(int contentTypeId)
        {
            return await context.OrganizationContentPermissions
                .Where(p => p.ContentTypeId == contentTypeId && p.IsEnabled == true)
                .Include(p => p.Organization)
                .ThenInclude(o => o.User)
                .Include(p => p.ContentType)
                .Select(p => new OrganizationWithPermissionsDto
                {
                    OrganizationId = p.Organization.OrganizationId,
                    Name = p.Organization.Name,
                    Description = p.Organization.Description,
                    LogoUrl = p.Organization.LogoUrl,
                    OwnerId = p.Organization.UserId,
                    OwnerName = p.Organization.User.Username,
                    ContentType = new ContentTypeDto
                    {
                        ContentTypeId = p.ContentType.ContentTypeId,
                        Name = p.ContentType.Name,
                        Description = p.ContentType.Description
                    },
                    PermissionGrantedAt = p.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<List<OrganizationWithContentTypesDto>> GetOrganizationsWithContentTypesAsync(List<int> contentTypeIds)
        {

            try
            {
                return await context.OrganizationContentPermissions
                    .Where(p => contentTypeIds.Contains(p.ContentTypeId) && p.IsEnabled == true)
                    .Include(p => p.Organization)
                        .ThenInclude(o => o.User)
                    .Include(p => p.ContentType)
                    .Select(p => new {
                        Permission = p,
                        SafeDescription = p.Organization.Description ?? string.Empty,
                        SafeLogoUrl = p.Organization.LogoUrl ?? string.Empty,
                        UserId = p.Organization.User != null ? p.Organization.User.UserId : 0,
                        UserName = p.Organization.User != null ? p.Organization.User.Username : string.Empty
                    })
                    .GroupBy(x => x.Permission.Organization)
                    .Select(g => new OrganizationWithContentTypesDto
                    {
                        Organization = new OrganizationDto
                        {
                            OrganizationId = g.Key.OrganizationId,
                            Name = g.Key.Name,
                            Description = g.First().SafeDescription,
                            LogoUrl = g.First().SafeLogoUrl,
                            UserId = g.First().UserId,
                            UserName = g.First().UserName
                        },
                        AllowedContentTypes = g.Select(x => new ContentTypePermissionDto
                        {
                            ContentTypeId = x.Permission.ContentTypeId,
                            Name = x.Permission.ContentType.Name ?? string.Empty,
                            PermissionGrantedAt = x.Permission.CreatedAt
                        }).ToList()
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Loggear el error
                throw new Exception("Error al obtener organizaciones con tipos de contenido", ex);
            }

        }

        public async Task<bool> UpdatePermission(int organizationId, int contentTypeId, bool isEnabled)
        {
            var permission = await context.OrganizationContentPermissions
            .FirstOrDefaultAsync(p => p.OrganizationId == organizationId &&
                                    p.ContentTypeId == contentTypeId);

            if (permission == null)
            {
                permission = new OrganizationContentPermission
                {
                    OrganizationId = organizationId,
                    ContentTypeId = contentTypeId,
                    IsEnabled = isEnabled
                };
                context.OrganizationContentPermissions.Add(permission);
            }
            else
            {
                permission.IsEnabled = isEnabled;
                permission.UpdatedAt = DateTime.UtcNow;
            }

            await context.SaveChangesAsync();
            return true;
        }
    }
}
