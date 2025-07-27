using Baylongo.Data.Data.MsSql.Contexts.SqlServer.BaylongoContext;
using Baylongo.Data.Data.MsSql.Models.DBBaylongo;
using BaylongoApi.DTOs.Email;
using BaylongoApi.DTOs.Users;
using BaylongoApi.Services.Email.Contracts;
using BaylongoApi.Services.Interfaces;
using BaylongoApi.Templates.Contracts;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using System.Security.Cryptography;

namespace BaylongoApi.Services
{
    public class UserService(BaylongoContext context, IEmailService emailService, IConfiguration configuration, IPasswordHasher passwordHash, ITemplateService templateService
) : IUserService
    {
        public async Task<bool> DeleteUser(int id, int? requestingUserId )
        {
            var isAdmin = await IsUserAdmin(requestingUserId.Value);
            if (!isAdmin)
            {
                throw new UnauthorizedAccessException("Solo los administradores pueden eliminar usuarios");
            }

            var user = await context.Users.FindAsync(id);
            if (user == null)
            {
                //_logger.LogWarning("Intento de eliminar usuario no existente: {UserId}", id);
                throw new KeyNotFoundException($"Usuario con ID {id} no encontrado");
            }

            // Borrado lógico (mejor práctica que borrado físico)
            user.IsActive = false;
            user.Username = $"{user.Username}_DELETED_{DateTime.UtcNow.Ticks}";
            user.Email = $"{user.Email}_DELETED_{DateTime.UtcNow.Ticks}";

            try
            {
                await context.SaveChangesAsync();
                //_logger.LogInformation("Usuario {UserId} marcado como inactivo", id);
                return true;
            }
            catch (DbUpdateException ex)
            {
                //_logger.LogError(ex, "Error al marcar usuario {UserId} como inactivo", id);
                throw new Exception("Error al eliminar el usuario");
            }
        }

        public async Task<bool> IsUserAdmin(int userId)
        {
            var user = await context.Users
                .Include(u => u.UserType)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                throw new KeyNotFoundException("Usuario no encontrado");
            }

            // Asumiendo que en tu tabla UserTypes, los administradores tienen TypeId = 1
            return user.UserType?.TypeName?.ToLower() == "Administrador" || user.UserTypeId == 1;
        }
        public async Task<IEnumerable<UserDto>> GetAllUsers()
        {
            return null;
           // var users = await context.Users
           //.Include(u => u.UserType)
           //.Include(u => u.UserOrganizations)
           //    .ThenInclude(uo => uo.Organization)
           //.Where(u => u.IsActive)
           //.ToListAsync();

           // return users.Select(MapUserToDto);
        }

        public async Task<UserDto> GetUserById(int id)
        {
            var user = await context.Users
            .Include(u => u.UserType)
            .Include(u => u.UserOrganizations)
                .ThenInclude(uo => uo.Organization)
            .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                //_logger.LogWarning("Usuario con ID {UserId} no encontrado", id);
                throw new KeyNotFoundException($"Usuario con ID {id} no encontrado");
            }

            var userType = await context.UserTypes.FindAsync(user.UserTypeId);
            var userTypeDto = new UserTypeDto
            {
                TypeId = userType.TypeId,
                TypeName = userType.TypeName,
                Description = userType.Description
            };

            return MapUserToDto(user, userTypeDto);
        }

        // Método privado para mapear User a UserDto
        private UserDto MapUserToDto(User user, UserTypeDto userType)
        {
            return new UserDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                Phone = user.Phone,
                UserType = user.UserType?.TypeName ?? "Desconocido",
                UserTypeId = user.UserTypeId,
                ProfilePictureUrl = user.ProfilePictureUrl,
                RegistrationDate = user.RegistrationDate,
                LastLogin = user.LastLogin,
                IsActive = user.IsActive,
                IsVerified = user.IsVerified,
                UserTypes = userType,
                OrganizationId = user.UserOrganizations.FirstOrDefault()?.OrganizationId,
                OrganizationName = user.UserOrganizations.FirstOrDefault()?.Organization?.Name
            };
        }

        public async Task<PasswordResetResult> ForgotPasswordAsync(string email)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                return new PasswordResetResult(true);
            }

            // Generate a 6-digit code
            string resetCode = GenerateResetCode();

            // Store the reset code in the database with expiration
            DateTime expiration = DateTime.UtcNow.AddMinutes(15);
            var resetToken = new PasswordResetToken
            {
                UserId = user.UserId,
                ResetCode = resetCode,
                ExpirationDate = expiration
            };

            // Eliminar tokens previos no usados
            var oldTokens = await context.PasswordResetTokens
                .Where(t => t.UserId == user.UserId && !t.IsUsed)
                .ToListAsync();

            context.PasswordResetTokens.RemoveRange(oldTokens);
            context.PasswordResetTokens.Add(resetToken);
            await context.SaveChangesAsync();

            try
            {
                var emailResult = await SendResetEmailAsync(user, resetCode, expiration);

                return emailResult.Success
                    ? new PasswordResetResult(true)
                    : new PasswordResetResult(false, "Mensaje de error");
            }
            catch (Exception ex)
            {
                // Loggear el error adecuadamente
                //logger.LogError(ex, $"Error sending password reset email to {email}");
                return new PasswordResetResult(false);
            }
        }

        private async Task<EmailResult> SendResetEmailAsync(User user, string resetCode, DateTime expiration)
        {
            try
            {
                var parameters = new Dictionary<string, string>
            {
                {"NombreUsuario", user.Username ?? "Usuario"},
                {"NombreEmpresa", configuration["Company:Name"] ?? "Nuestra Empresa"},
                {"ResetCode", resetCode},
                {"FechaExpiracion", expiration.ToString("f")},
                {"Anio", DateTime.Now.Year.ToString()},
                {"Email", user.Email},
                {"UrlPrivacidad", configuration["Company:PrivacyUrl"] ?? "#"},
                {"UrlTerminos", configuration["Company:TermsUrl"] ?? "#"},
                {"UrlContacto", configuration["Company:ContactUrl"] ?? "#"}
            };

                var htmlContent = await templateService.RenderTemplateAsync(
                    "SendPasswordReset",
                    parameters);

                var emailRequest = new EmailRequest(
                    ToEmail: user.Email,
                    ToName: user.Username ?? user.Email,
                    Subject: $"{parameters["NombreEmpresa"]} - Restablecimiento de contraseña",
                    HtmlContent: htmlContent);

                return await emailService.SendEmailAsync(emailRequest);
            }
            catch (Exception ex)
            {
                return new EmailResult(false, ex.Message);
            }
        }

        private string GenerateResetCode(int length = 6)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            using var rng = new RNGCryptoServiceProvider();
            byte[] randomBytes = new byte[length];
            rng.GetBytes(randomBytes);
            char[] result = new char[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = chars[randomBytes[i] % chars.Length];
            }
            return new string(result);
        }


        public async Task<bool> ResetPasswordAsync(string email, string code, string newPassword)
        {
            // Validar código primero
            if (!await VerifyResetCodeAsync(email, code))
                return false;

            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return false;

            // Marcar token como usado
            var token = await context.PasswordResetTokens
                .Where(t => t.UserId == user.UserId && t.ResetCode == code)
                .FirstOrDefaultAsync();

            if (token != null)
            {
                token.IsUsed = true;
                context.PasswordResetTokens.Update(token);
            }

            // Actualizar contraseña
            user.PasswordHash = passwordHash.HashPassword(newPassword);
            context.Users.Update(user);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> VerifyResetCodeAsync(string email, string code)
        {
            // Validaciones básicas
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(code))
                return false;

            // Buscar usuario y token
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) return false;

            var token = await context.PasswordResetTokens
                .Where(t => t.UserId == user.UserId &&
                           t.ResetCode == code &&
                           t.ExpirationDate >= DateTime.UtcNow &&
                           !t.IsUsed)
                .OrderByDescending(t => t.ExpirationDate)
                .FirstOrDefaultAsync();

            return token != null;
        }

        public static string CreatePasswordHash(string password)
        {
            // Parámetros de configuración (pueden ir en appsettings.json)
            const int iterations = 10000;
            const int hashSize = 32; // 256 bits
            const int saltSize = 16; // 128 bits

            // Generar salt aleatorio
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[saltSize]);

            // Generar el hash
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
            byte[] hash = pbkdf2.GetBytes(hashSize);

            // Combinar salt y hash
            byte[] hashBytes = new byte[saltSize + hashSize];
            Array.Copy(salt, 0, hashBytes, 0, saltSize);
            Array.Copy(hash, 0, hashBytes, saltSize, hashSize);

            // Convertir a Base64 para almacenamiento
            return Convert.ToBase64String(hashBytes);
        }

        public async Task<AuthResponse> GetCurrentUserAsync(int userId)
        {
            var user = await context.Users
           .Include(u => u.UserType)
           .Include(u => u.UserOrganizations)
               .ThenInclude(uo => uo.Organization)
           .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
                throw new Exception("User not found");

            var userTypeDto = new UserTypeDto
            {
                TypeId = user.UserType.TypeId,
                TypeName = user.UserType.TypeName,
                Description = user.UserType.Description
            };

            return new AuthResponse
            {
                User = new UserDto
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    Phone = user.Phone,
                    UserType = user.UserType.TypeName,
                    RegistrationDate = user.RegistrationDate,
                    ProfilePictureUrl = user.ProfilePictureUrl,
                    IsVerified = user.IsVerified,
                    UserTypes = userTypeDto,
                    UserTypeId = userTypeDto.TypeId == null ? 0 : userTypeDto.TypeId,
                    OrganizationId = user.UserOrganizations.FirstOrDefault()?.OrganizationId,
                    OrganizationName = user.UserOrganizations.FirstOrDefault()?.Organization?.Name
                }
            };
        }
    }
}
