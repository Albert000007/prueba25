using Baylongo.Data.Data.MsSql.Contexts.SqlServer.BaylongoContext;
using Baylongo.Data.Data.MsSql.Models.DBBaylongo;
using BaylongoApi.DTOs.Email;
using BaylongoApi.DTOs.Organizations;
using BaylongoApi.DTOs.Organizations.Enum;
using BaylongoApi.Services.Email;
using BaylongoApi.Services.Email.Contracts;
using BaylongoApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;

namespace BaylongoApi.Services
{
    public class OrganizationInvitationService(
        BaylongoContext context,
        IEmailService emailService,
        IConfiguration configuration) : IOrganizationInvitationService
    {
        private const int MAX_PENDING_INVITATIONS = 10;

        public async Task<bool> AcceptInvitation(string token)
        {
            var invitation = await context.OrganizationInvitations
      .FirstOrDefaultAsync(i => i.InvitationToken == token && i.Status == "Pending");

            if (invitation == null)
            {
                return false;
            }

            if (invitation.ExpirationDate < DateTime.UtcNow)
            {
                invitation.Status = "Expired";
                await context.SaveChangesAsync();
                return false;
            }

            var user = await context.Users
                .FirstOrDefaultAsync(u => u.Email == invitation.InvitedUserEmail);

            if (user == null)
            {
                return false;
            }

            // Actualizar tipo de usuario si es necesario
            if (invitation.Purpose == "OrganizationManagement" ||
                invitation.Purpose == "EventManagement")
            {
                user.UserTypeId = 2; // Organizador
            }

            // Crear relación usuario-organización
            var userOrg = new UserOrganization
            {
                UserId = user.UserId,
                OrganizationId = invitation.OrganizationId,
                RoleId = invitation.RoleId,
                JoinDate = DateTime.UtcNow,
                IsActive = true
            };

            await context.UserOrganizations.AddAsync(userOrg);

            // Actualizar estado de la invitación
            invitation.Status = "Accepted";
            invitation.UpdatedAt = DateTime.UtcNow;
            invitation.InvitedUserId = user.UserId;

            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CancelInvitation(Guid invitationId, int requestingUserId)
        {
            var invitation = await context.OrganizationInvitations
                .Include(i => i.Organization)
                .FirstOrDefaultAsync(i => i.InvitationId == invitationId);

            if (invitation == null)
            {
                return false;
            }

            // Validar que el usuario que cancela es admin de la organización
            var isAdmin = await context.UserOrganizations
                .AnyAsync(uo => uo.UserId == requestingUserId &&
                               uo.OrganizationId == invitation.OrganizationId &&
                               uo.Role.RoleName == "Administrador");

            if (!isAdmin && invitation.InvitingUserId != requestingUserId)
            {
                throw new UnauthorizedAccessException("No tienes permisos para cancelar esta invitación");
            }

            if (invitation.Status != "Pending")
            {
                throw new InvalidOperationException("Solo se pueden cancelar invitaciones pendientes");
            }

            invitation.Status = "Cancelled";
            invitation.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return true;
        }

        public async Task<OrganizationInvitation?> GetInvitation(Guid invitationId)
        {
            return await context.OrganizationInvitations
                .Include(i => i.Organization)
                .Include(i => i.InvitingUser)
                .Include(i => i.InvitedUser)
                .Include(i => i.Role)
                .FirstOrDefaultAsync(i => i.InvitationId == invitationId);
        }

        public async Task<List<OrganizationInviteeDto>> GetOrganizationInvitees(int organizationId)
        {
            return await context.OrganizationInvitations
        .Where(i => i.OrganizationId == organizationId)
        .Include(i => i.InvitedUser)
        .Include(i => i.InvitingUser)
        .Include(i => i.Role)
        .Select(i => new OrganizationInviteeDto
        {
            InvitationId = i.InvitationId,
            InvitedUserId = i.InvitedUserId,
            InvitedUserName = i.InvitedUser != null ? i.InvitedUser.Username : i.InvitedUserEmail,
            InvitedUserEmail = i.InvitedUserEmail,
            InvitingUserId = i.InvitingUserId,
            InvitingUserName = i.InvitingUser.Username,
            RoleId = i.RoleId,
            RoleName = i.Role.RoleName,
            Status = i.Status,
            Purpose = i.Purpose,
            CreatedAt = i.CreatedAt,
            ExpirationDate = i.ExpirationDate,
            AcceptedAt = i.Status == "Accepted" ? i.UpdatedAt : (DateTime?)null
        })
        .OrderByDescending(i => i.CreatedAt)
        .ToListAsync();
        }

        public async Task<List<OrganizationInvitation>> GetPendingInvitationsForOrganization(int organizationId)
        {
            return await context.OrganizationInvitations
               .Where(i => i.OrganizationId == organizationId && i.Status == "Pending")
               .Include(i => i.InvitingUser)
               .Include(i => i.InvitedUser)
               .Include(i => i.Role)
               .ToListAsync();
        }

        public async Task<List<OrganizationInvitation>> GetPendingInvitationsForUser(int userId)
        {
            return await context.OrganizationInvitations
               .Where(i => (i.InvitedUserId == userId ||
                          context.Users.Any(u => u.UserId == userId && u.Email == i.InvitedUserEmail)) &&
                          i.Status == "Pending")
               .Include(i => i.Organization)
               .Include(i => i.InvitingUser)
               .Include(i => i.Role)
               .ToListAsync();
        }

        public async Task<bool> HasPermission(int userId, int organizationId, InvitationPurpose requiredPurpose)
        {
            var userOrg = await context.UserOrganizations
                .Include(uo => uo.Role)
                .FirstOrDefaultAsync(uo => uo.UserId == userId &&
                                          uo.OrganizationId == organizationId &&
                                          (uo.IsActive ?? false));

            if (userOrg == null)
            {
                return false;
            }

            // Administradores tienen todos los permisos
            if (userOrg.Role.RoleName == "Administrador")
            {
                return true;
            }

            // Verificar permisos según el propósito requerido
            return requiredPurpose switch
            {
                InvitationPurpose.OrganizationManagement =>
                    userOrg.Role.RoleName == "Administrador",

                InvitationPurpose.EventManagement =>
                    userOrg.Role.RoleName == "Administrador" ||
                    userOrg.Role.RoleName == "Event Manager",

                InvitationPurpose.TicketValidation =>
                    userOrg.Role.RoleName == "Administrador" ||
                    userOrg.Role.RoleName == "Event Manager" ||
                    userOrg.Role.RoleName == "Ticket Validator",

                _ => false
            };
        }

        public async Task<bool> RejectInvitation(string token)
        {
            var invitation = await context.OrganizationInvitations
    .FirstOrDefaultAsync(i => i.InvitationToken == token && i.Status == "Pending");

            if (invitation == null)
            {
                return false;
            }

            invitation.Status = "Rejected";
            invitation.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ResendInvitation(Guid invitationId)
        {
            var invitation = await context.OrganizationInvitations
                .FirstOrDefaultAsync(i => i.InvitationId == invitationId);

            if (invitation == null || invitation.Status != "Pending")
            {
                return false;
            }

            // Extender fecha de expiración
            invitation.ExpirationDate = DateTime.UtcNow.AddDays(7);
            invitation.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            // Reenviar email
            await SendInvitationEmail(invitation);

            return true;
        }

        public async Task<OrganizationInvitation> SendInvitation(
                                                    int organizationId,
                                                    int invitingUserId,
                                                    string invitedUserEmail,
                                                    InvitationPurpose purpose)
        {
            // Validación de límite de invitaciones
            var pendingCount = await context.OrganizationInvitations
                .CountAsync(i => i.OrganizationId == organizationId && i.Status == "Pending");

            if (pendingCount >= MAX_PENDING_INVITATIONS)
            {
                throw new InvalidOperationException(
                    $"No se pueden enviar más invitaciones. Límite de {MAX_PENDING_INVITATIONS} invitaciones pendientes alcanzado.");
            }

            // Validación de permisos del invitador
            var isAdmin = await context.UserOrganizations
                .AnyAsync(uo => uo.UserId == invitingUserId &&
                               uo.OrganizationId == organizationId &&
                               uo.Role.RoleName == "Administrador");

            if (!isAdmin)
            {
                throw new UnauthorizedAccessException("No tienes permisos para invitar usuarios");
            }

            // Validación de usuario existente
            var invitedUser = await context.Users
                .FirstOrDefaultAsync(u => u.Email == invitedUserEmail);

            if (invitedUser != null)
            {
                var alreadyMember = await context.UserOrganizations
                    .AnyAsync(uo => uo.UserId == invitedUser.UserId &&
                                  uo.OrganizationId == organizationId);

                if (alreadyMember)
                {
                    throw new InvalidOperationException("El usuario ya es miembro de esta organización");
                }
            }

            // Determinar el rol según el propósito
            int roleId = purpose switch
            {
                InvitationPurpose.OrganizationManagement => 1, // Administrador
                InvitationPurpose.EventManagement => 4,        // Gestor de Eventos
                InvitationPurpose.TicketValidation => 5,      // Validador
                _ => throw new ArgumentException("Propósito de invitación no válido")
            };

            // Crear la invitación
            var invitation = new OrganizationInvitation
            {
                OrganizationId = organizationId,
                InvitingUserId = invitingUserId,
                InvitedUserEmail = invitedUserEmail,
                InvitedUserId = invitedUser.UserId,
                RoleId = roleId,
                InvitationToken = Guid.NewGuid().ToString(),
                ExpirationDate = DateTime.UtcNow.AddDays(7),
                Purpose = purpose.ToString(),
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.OrganizationInvitations.Add(invitation);
            await context.SaveChangesAsync();

            // Enviar email de invitación
            await SendInvitationEmail(invitation);

            return invitation;
        }

        public async Task UpdateInvitationRole(int organizationId, int InvitedUserId, int roleId, int requestingUserId,Guid invitationId)
        {
            // Verificar que la relación usuario-organización existe
            var userOrg = await context.OrganizationInvitations
                .Include(uo => uo.Role)
                .FirstOrDefaultAsync(uo =>
                    uo.InvitedUserId == InvitedUserId &&
                    uo.OrganizationId == organizationId
                    && uo.InvitationId == invitationId);

            if (userOrg == null)
            {
                throw new KeyNotFoundException("El usuario no es miembro de esta organización");
            }

            // Verificar que el rol existe
            var newRole = await context.OrganizationRoles
                .FirstOrDefaultAsync(r => r.RoleId == roleId);

            if (newRole == null)
            {
                throw new KeyNotFoundException("El rol especificado no existe");
            }

            // Verificar que el solicitante es administrador
            var isAdmin = await context.UserOrganizations
                .AnyAsync(uo =>
                    uo.UserId == requestingUserId &&
                    uo.OrganizationId == organizationId &&
                    uo.Role.RoleName == "Administrador");

            if (!isAdmin)
            {
                throw new UnauthorizedAccessException("No tienes permisos para realizar esta acción");
            }

            // Actualizar el rol
            userOrg.RoleId = roleId;

            await context.SaveChangesAsync();
        }

        public async Task<bool> UpdateUserOrganizationRole(
                        int organizationId,
                        int userId,
                        int roleId,
                        int requestingUserId)
        {
            // Verificar que la relación usuario-organización existe
            var userOrg = await context.UserOrganizations
                .Include(uo => uo.Role)
                .FirstOrDefaultAsync(uo =>
                    uo.UserId == userId &&
                    uo.OrganizationId == organizationId);

            if (userOrg == null)
            {
                throw new KeyNotFoundException("El usuario no es miembro de esta organización");
            }

            // Verificar que el rol existe
            var newRole = await context.OrganizationRoles
                .FirstOrDefaultAsync(r => r.RoleId == roleId);

            if (newRole == null)
            {
                throw new KeyNotFoundException("El rol especificado no existe");
            }

            // Verificar que el solicitante es administrador
            var isAdmin = await context.UserOrganizations
                .AnyAsync(uo =>
                    uo.UserId == requestingUserId &&
                    uo.OrganizationId == organizationId &&
                    uo.Role.RoleName == "Administrador");

            if (!isAdmin)
            {
                throw new UnauthorizedAccessException("No tienes permisos para realizar esta acción");
            }

            // Actualizar el rol
            userOrg.RoleId = roleId;

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ValidateRoleForPurpose(int roleId, InvitationPurpose purpose)
        {
            var role = await context.OrganizationRoles
                .FirstOrDefaultAsync(r => r.RoleId == roleId);

            if (role == null) return false;

            return purpose switch
            {
                InvitationPurpose.OrganizationManagement =>
                    role.RoleName == "Administrador",
                InvitationPurpose.EventManagement =>
                    role.RoleName == "Administrador" || role.RoleName == "Event Manager",
                InvitationPurpose.TicketValidation =>
                    role.RoleName == "Ticket Validator" || role.RoleName == "Administrador",
                _ => false
            };
        }

        
        private async Task SendInvitationEmail(OrganizationInvitation invitation)
        {
            var organization = await context.Organizations
                .FirstAsync(o => o.OrganizationId == invitation.OrganizationId);

            var invitingUser = await context.Users
                .FirstAsync(u => u.UserId == invitation.InvitingUserId);

            var acceptUrl = $"{configuration["AppSettings:BaseUrl"]}/api/invitations/accept/{invitation.InvitationToken}";

            var purposeDescription = invitation.Purpose switch
            {
                "OrganizationManagement" => "gestión completa de la organización",
                "EventManagement" => "gestión de eventos y talleres",
                "TicketValidation" => "validación de tickets de eventos",
                _ => "participación en la organización"
            };

            var emailSubject = $"Invitación para {purposeDescription} en {organization.Name}";

            var emailBody = $@"
                <h2>¡Has sido invitado a {organization.Name}!</h2>
                <p>{invitingUser.Username} te ha invitado a unirte a {organization.Name} con permisos de {purposeDescription}.</p>
                <p><strong>Tipo de acceso:</strong> {purposeDescription}</p>
                <p>Esta invitación expirará el {invitation.ExpirationDate:dd/MM/yyyy}.</p>
                <p><a href='{acceptUrl}'>Haz clic aquí para aceptar la invitación</a></p>
                <p>Si no puedes hacer clic en el enlace, copia y pega esta URL en tu navegador: {acceptUrl}</p>
                <p>Si no reconoces esta invitación, por favor ignora este correo.</p>
            
            ";

            var emailRequest = new EmailRequest(
                                ToEmail: invitation.InvitedUserEmail,
                                ToName: invitingUser.Username,
                                Subject: emailSubject,
                                HtmlContent: emailBody);

            await emailService.SendEmailAsync(emailRequest);
        }
    }
}
