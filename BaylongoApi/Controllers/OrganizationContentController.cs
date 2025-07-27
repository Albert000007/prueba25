using Baylongo.Data.Data.MsSql.Contexts.SqlServer.BaylongoContext;
using BaylongoApi.Services;
using BaylongoApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using System.Security.Claims;

namespace BaylongoApi.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class OrganizationContentController(BaylongoContext context,OrganizationContentService contentService,ILogger<DanceController> logger) : ControllerBase
    {
        [HttpGet("available-types")]
        public async Task<IActionResult> GetAvailableContentTypes()
        {
            var types = await contentService.GetAllContentTypes();
            return Ok(types);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrganizationPermissions(int organizationId)
        {
            // Verificar que el usuario es admin de la organización
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var isAdmin = await context.UserOrganizations
                .AnyAsync(uo => uo.UserId == userId &&
                              uo.OrganizationId == organizationId &&
                              uo.Role.RoleName == "Administrador");

            if (!isAdmin)
            {
                return Forbid();
            }

            var permissions = await contentService.GetOrganizationPermissions(organizationId);
            return Ok(permissions);
        }

        [HttpPut("{contentTypeId}")]
        public async Task<IActionResult> UpdatePermission(
    int organizationId,
    int contentTypeId,
    [FromBody] bool isEnabled)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var isAdmin = await context.UserOrganizations
                .AnyAsync(uo => uo.UserId == userId &&
                              uo.OrganizationId == organizationId &&
                              uo.Role.RoleName == "Administrador");

            if (!isAdmin)
            {
                return Forbid();
            }

            var result = await contentService.UpdatePermission(organizationId, contentTypeId, isEnabled);
            return Ok(new
            {
                Success = result,
                Message = $"Permiso actualizado para el tipo de contenido ID {contentTypeId}"
            });
        }

        [HttpDelete("{contentTypeId}")]
        public async Task<IActionResult> DeletePermission(
    int organizationId,
    int contentTypeId)
        {
            try
            {
                // 1. Verificar permisos del usuario
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var isAdmin = await context.UserOrganizations
                    .AnyAsync(uo => uo.UserId == userId &&
                                  uo.OrganizationId == organizationId &&
                                  uo.Role.RoleName == "Administrador");

                if (!isAdmin)
                {
                    return Forbid();
                }

                // 2. Verificar que el tipo de contenido existe
                var contentTypeExists = await context.ContentTypes
                    .AnyAsync(ct => ct.ContentTypeId == contentTypeId);

                if (!contentTypeExists)
                {
                    return NotFound("Tipo de contenido no encontrado");
                }

                // 3. Eliminar el permiso
                var result = await contentService.DeletePermission(organizationId, contentTypeId);

                if (!result)
                {
                    return NotFound("No se encontró el permiso especificado");
                }

                return Ok(new
                {
                    Success = true,
                    Message = $"Permiso eliminado para el tipo de contenido ID {contentTypeId}",
                    OrganizationId = organizationId,
                    ContentTypeId = contentTypeId
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = "Error al eliminar permiso",
                    Details = ex.Message
                });
            }
        }
        [HttpGet("by-content-type/{contentTypeId}")]
        public async Task<IActionResult> GetOrganizationsByContentType(int contentTypeId)
        {
            try
            {
                var organizations = await contentService.GetOrganizationsByContentTypeAsync(contentTypeId);
                return Ok(new
                {
                    ContentTypeId = contentTypeId,
                    Count = organizations.Count,
                    Organizations = organizations
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = "Error al obtener organizaciones",
                    Details = ex.Message
                });
            }
        }

        [HttpGet("by-content-types")]
        public async Task<IActionResult> GetOrganizationsWithContentTypes([FromQuery] List<int> contentTypeIds)
        {
            try
            {
                if (contentTypeIds == null || !contentTypeIds.Any())
                {
                    return BadRequest("Debe especificar al menos un tipo de contenido");
                }

                var organizations = await contentService.GetOrganizationsWithContentTypesAsync(contentTypeIds);
                return Ok(new
                {
                    TotalOrganizations = organizations.Count,
                    Organizations = organizations
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = "Error al obtener organizaciones",
                    Details = ex.Message
                });
            }
        }
    }
}

