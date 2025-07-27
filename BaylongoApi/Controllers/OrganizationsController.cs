using Baylongo.Data.Data.MsSql.Models.DBBaylongo;
using BaylongoApi.DTOs.Organizations;
using BaylongoApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace BaylongoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationsController : ControllerBase
    {
        private readonly IOrganizationService _organizationService;
        private readonly ILogger<OrganizationsController> _logger;

        public OrganizationsController(
            IOrganizationService organizationService,
            ILogger<OrganizationsController> logger)
        {
            _organizationService = organizationService;
            _logger = logger;
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateOrganization(CreateOrganizationDto createDto)
        {
            try
            {
                var organization = await _organizationService.CreateOrganization(createDto);
                // Versión corregida del CreatedAtAction
                return CreatedAtAction(
                    actionName: nameof(GetOrganization), // Nombre del método de acción
                    routeValues: new { id = organization.OrganizationId }, // Valores de ruta
                    value: organization); // Valor a retornar
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Error al crear organización");
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear organización");
                return StatusCode(500, ex.Message);
            }
        }
        [Authorize]
        [HttpPost("{id}/logo")]
        public async Task<IActionResult> UploadLogo(int id, IFormFile file)
        {
            try
            {
                var logoUrl = await _organizationService.OrganizationLogoAsync(id, file);
                return Ok(new { LogoUrl = logoUrl });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading logo");
                return StatusCode(500, "Internal server error");
            }
        }
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrganization(int id, [FromBody] UpdateOrganizationDto dto)
        {
            try
            {
                if (id != dto.OrganizationId)
                {
                    return BadRequest("ID mismatch");
                }

                var updatedOrganization = await _organizationService.UpdateOrganization(id, dto);
                return Ok(updatedOrganization);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating organization");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserOrganizationHierarchy(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string search = "",
        [FromQuery] int? organizationTypeId = null,
        [FromQuery] int? verificationStatusId = null)
        {
            try
            {

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                var organizations = await _organizationService.GetUserOrganizationHierarchy(
                    userId,
                    page,
                    pageSize,
                    search,
                    organizationTypeId,
                    verificationStatusId);

                // Agregar headers de paginación
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(organizations.Metadata));

                return Ok(organizations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las organizaciones");
                return StatusCode(500, new { message = "Ocurrió un error interno" });
            }
        }
        [Authorize]
        [HttpGet("{id}", Name = "GetOrganization")]
        public async Task<ActionResult<OrganizationDto>> GetOrganization(int id)
        {
            try
            {
                var organization = await _organizationService.GetOrganizationById(id);
                return Ok(organization);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteOrganization(int id)
        {
            try
            {
                // Obtener el ID del usuario autenticado
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var result = await _organizationService.DeleteOrganization(id, userId);

                if (result)
                {
                    return NoContent(); // 204 No Content
                }

                return BadRequest("No se pudo eliminar la organización");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar organización {OrganizationId}", id);
                return StatusCode(500, "Ocurrió un error interno al eliminar la organización");
            }
        }
    }
}
