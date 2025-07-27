using Baylongo.Data.Data.MsSql.Models.DBBaylongo;
using BaylongoApi.DTOs.Organizations;
using BaylongoApi.DTOs.Organizations.Enum;

namespace BaylongoApi.Services.Interfaces
{
    public interface IOrganizationInvitationService
    {
        /// <summary>
        /// Envía una nueva invitación a una organización
        /// </summary>
        /// <param name="organizationId">ID de la organización</param>
        /// <param name="invitingUserId">ID del usuario que envía la invitación</param>
        /// <param name="invitedUserEmail">Email del usuario invitado</param>
        /// <param name="purpose">Propósito de la invitación (gestión, eventos o validación)</param>
        /// <returns>Invitación creada</returns>
        Task<OrganizationInvitation> SendInvitation(
            int organizationId,
            int invitingUserId,
            string invitedUserEmail,
            InvitationPurpose purpose);
        Task<bool> AcceptInvitation(string token);
        Task<bool> RejectInvitation(string token);
        /// <summary>
        /// Reenvía una invitación expirada o próxima a expirar
        /// </summary>
        /// <param name="invitationId">ID de la invitación</param>
        /// <returns>True si se reenvió correctamente</returns>
        /// <summary>
        /// Rechaza una invitación pendiente
        /// </summary>
        /// <param name="token">Token de la invitación</param>
        /// <returns>True si se rechazó correctamente</returns>
        ///         /// <summary>
        /// Reenvía una invitación expirada o próxima a expirar
        /// </summary>
        /// <param name="invitationId">ID de la invitación</param>
        /// <returns>True si se reenvió correctamente</returns>
        Task<bool> ResendInvitation(Guid invitationId);
        /// <summary>
        /// Obtiene una invitación por su ID
        /// </summary>
        /// <param name="invitationId">ID de la invitación</param>
        /// <returns>Invitación encontrada o null</returns>
        Task<OrganizationInvitation?> GetInvitation(Guid invitationId);
        /// <summary>
        /// Obtiene todas las invitaciones pendientes para un usuario
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>Lista de invitaciones pendientes</returns>
        Task<List<OrganizationInvitation>> GetPendingInvitationsForUser(int userId);
        /// <summary>
        /// Obtiene todas las invitaciones pendientes para una organización
        /// </summary>
        /// <param name="organizationId">ID de la organización</param>
        /// <returns>Lista de invitaciones pendientes</returns>
        Task<List<OrganizationInvitation>> GetPendingInvitationsForOrganization(int organizationId);
        /// <summary>
        /// Cancela una invitación pendiente
        /// </summary>
        /// <param name="invitationId">ID de la invitación</param>
        /// <param name="requestingUserId">ID del usuario que realiza la solicitud</param>
        /// <returns>True si se canceló correctamente</returns>
        Task<bool> CancelInvitation(Guid invitationId, int requestingUserId);
        /// <summary>
        /// Verifica si un usuario puede realizar una acción específica en una organización
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="organizationId">ID de la organización</param>
        /// <param name="requiredPurpose">Propósito requerido para la acción</param>
        /// <returns>True si el usuario tiene los permisos necesarios</returns>
        Task<bool> HasPermission(
            int userId,
            int organizationId,
            InvitationPurpose requiredPurpose);

        Task<bool> ValidateRoleForPurpose(int roleId, InvitationPurpose purpose);
        Task<bool> UpdateUserOrganizationRole(int organizationId, int userId, int roleId, int requestingUserId);
        Task<List<OrganizationInviteeDto>> GetOrganizationInvitees(int organizationId);
        Task UpdateInvitationRole(int organizationId, int InvitedUserId, int roleId, int requestingUserId, Guid invitationId);
    }
}
