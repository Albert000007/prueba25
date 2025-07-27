using BaylongoApi.DTOs.Organizations;
using BaylongoApi.DTOs.Organizations.Enum;
using BaylongoApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BaylongoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrganizationInvitationsController(IOrganizationInvitationService invitationService) : ControllerBase
    {
        /// <summary>
        /// Envía una nueva invitación a una organización
        /// </summary>
        [HttpPost("SendInvitation")]
        public async Task<IActionResult> SendInvitation([FromBody] OrganizationInvitationRequestDto request)
        {
            try
            {
                var invitation = await invitationService.SendInvitation(
                    request.OrganizationId,
                    request.InvitingUserId,
                    request.InvitedUserEmail,
                    request.Purpose);

                return CreatedAtAction(nameof(GetInvitation),
                    new { invitationId = invitation.InvitationId },
                    invitation);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Acepta una invitación pendiente
        /// </summary>
        [HttpPost("AcceptInvitation/{token}")]
        [AllowAnonymous]
        public async Task<IActionResult> AcceptInvitation(string token)
        {
            try
            {
                var result = await invitationService.AcceptInvitation(token);

                if (!result)
                {
                    return BadRequest("Invitación no válida o expirada");
                }

                return Ok(new { Message = "Invitación aceptada exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Rechaza una invitación pendiente
        /// </summary>
        [HttpPost("RejectInvitation/{token}")]
        public async Task<IActionResult> RejectInvitation(string token)
        {
            try
            {
                var result = await invitationService.RejectInvitation(token);

                if (!result)
                {
                    return BadRequest("Invitación no válida");
                }

                return Ok(new { Message = "Invitación rechazada exitosamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Reenvía una invitación expirada o próxima a expirar
        /// </summary>
        [HttpPost("resend/{invitationId}")]
        public async Task<IActionResult> ResendInvitation(Guid invitationId)
        {
            try
            {
                var result = await invitationService.ResendInvitation(invitationId);

                if (!result)
                {
                    return NotFound("Invitación no encontrada o no está pendiente");
                }

                return Ok(new { Message = "Invitación reenviada exitosamente" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Obtiene una invitación por su ID
        /// </summary>
        [HttpGet("{invitationId}")]
        public async Task<IActionResult> GetInvitation(Guid invitationId)
        {
            try
            {
                var invitation = await invitationService.GetInvitation(invitationId);

                if (invitation == null)
                {
                    return NotFound();
                }

                return Ok(invitation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Obtiene todas las invitaciones pendientes para un usuario
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetPendingInvitationsForUser(int userId)
        {
            try
            {
                var invitations = await invitationService.GetPendingInvitationsForUser(userId);
                return Ok(invitations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        /// <summary>
        /// Obtiene todas las invitaciones pendientes para una organización
        /// </summary>
        [HttpGet("organization/{organizationId}")]
        public async Task<IActionResult> GetPendingInvitationsForOrganization(int organizationId)
        {
            try
            {
                var invitations = await invitationService.GetPendingInvitationsForOrganization(organizationId);
                return Ok(invitations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Cancela una invitación pendiente
        /// </summary>
        [HttpDelete("{invitationId}")]
        public async Task<IActionResult> CancelInvitation(Guid invitationId)
        {
            try
            {
                // Obtener el ID del usuario autenticado
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var result = await invitationService.CancelInvitation(invitationId, userId);

                if (!result)
                {
                    return NotFound("Invitación no encontrada");
                }

                return Ok(new { Message = "Invitación cancelada exitosamente" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Verifica si un usuario tiene permisos específicos en una organización
        /// </summary>
        [HttpGet("permissions/{userId}/{organizationId}")]
        public async Task<IActionResult> CheckPermissions(
            int userId,
            int organizationId,
            [FromQuery] InvitationPurpose purpose)
        {
            try
            {
                var hasPermission = await invitationService.HasPermission(userId, organizationId, purpose);
                return Ok(new { HasPermission = hasPermission });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPut("permissions/{invitationId}")]
        [Authorize]
        public async Task<IActionResult> UpdateInvitationPermissions(
    Guid invitationId,
    [FromBody] UpdateInvitationPermissionsDto permissionsDto)
        {
            try
            {
                var requestingUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                // Verificar que la invitación existe y está aceptada
                var invitation = await invitationService.GetInvitation(invitationId);
                if (invitation == null)
                {
                    return NotFound("Invitación no encontrada");
                }

                if (invitation.Status != "Accepted")
                {
                    return BadRequest("Solo se pueden asignar roles a invitaciones aceptadas");
                }

                // Verificar que el usuario que hace la solicitud tiene control total
                var hasFullControl = await invitationService.HasPermission(
                    requestingUserId,
                    invitation.OrganizationId,
                    InvitationPurpose.OrganizationManagement);

                if (!hasFullControl)
                {
                    return Forbid("Solo administradores pueden asignar roles");
                }

                // Validar que el rol coincida con el propósito
                //if (!await invitationService.ValidateRoleForPurpose(permissionsDto.RoleId, invitation.Purpose))
                //{
                //    return BadRequest("El rol seleccionado no es válido para el propósito especificado");
                //}

                // Actualizar los permisos
                var result = await invitationService.UpdateUserOrganizationRole(
                    invitation.OrganizationId,
                    invitation.InvitedUserId, // Ya está aceptada, así que tiene valor
                    permissionsDto.RoleId,
                    requestingUserId);

                await invitationService.UpdateInvitationRole(invitation.OrganizationId,
                    invitation.InvitedUserId,
                    permissionsDto.RoleId,
                    requestingUserId,
                    invitationId);


                if (!result)
                {
                    return BadRequest("No se pudo actualizar los permisos del usuario");
                }

                return Ok(new
                {
                    Message = "Permisos actualizados exitosamente",
                    RoleId = permissionsDto.RoleId,
                    Purpose = invitation.Purpose.ToString()
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("organization/{organizationId}/invitees")]
        [Authorize]
        public async Task<IActionResult> GetOrganizationInvitees(int organizationId)
        {
            try
            {
                // Obtener el ID del usuario autenticado
                var requestingUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                // Verificar que el usuario tiene permisos para ver los invitados
                var hasPermission = await invitationService.HasPermission(
                    requestingUserId,
                    organizationId,
                    InvitationPurpose.OrganizationManagement);

                if (!hasPermission)
                {
                    return Forbid("No tienes permisos para ver los invitados de esta organización");
                }

                // Obtener los invitados
                var invitees = await invitationService.GetOrganizationInvitees(organizationId);

                return Ok(invitees);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
