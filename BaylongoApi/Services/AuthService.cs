using Baylongo.Data.Data.MsSql.Contexts.SqlServer.BaylongoContext;
using Baylongo.Data.Data.MsSql.Models.DBBaylongo;
using BaylongoApi.DTOs.Email;
using BaylongoApi.DTOs.Users;
using BaylongoApi.Services.Email.Contracts;
using BaylongoApi.Services.Interfaces;
using BaylongoApi.Templates;
using BaylongoApi.Templates.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;

namespace BaylongoApi.Services
{
    public class AuthService(BaylongoContext context, ITemplateService templateService, ITokenService tokenService, IPasswordHasher passwordHash, IEmailService emailService, IConfiguration configuration, IErrorLoggerService errorLoggerService) : IAuthService
    {
        public async Task<AuthResponse> Login(LoginDto loginDto)
        {
            var user = await context.Users
      .Include(u => u.UserType)
      .Include(u => u.UserOrganizations)
          .ThenInclude(uo => uo.Organization)
      .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (user == null || !passwordHash.VerifyPassword(loginDto.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Username or password is incorrect");

            if (user.IsActive == false)
                throw new UnauthorizedAccessException("User account is disabled");

            var userType = await context.UserTypes.FindAsync(user.UserTypeId);
            var userTypeDto = new UserTypeDto
            {
                TypeId = userType.TypeId,
                TypeName = userType.TypeName,
                Description = userType.Description
            };
            var token = tokenService.CreateToken(user);

            return new AuthResponse
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddDays(7),
                User = new UserDto
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Phone = user.Phone,
                    Email = user.Email,
                    UserType = user.UserType.TypeName,
                    RegistrationDate = user.RegistrationDate,
                    ProfilePictureUrl = user.ProfilePictureUrl,
                    IsVerified = user.IsVerified,
                    UserTypes = userTypeDto,
                    UserTypeId = userTypeDto.TypeId == null ? 0 : userTypeDto.TypeId,
                    OrganizationId = user.UserOrganizations?.FirstOrDefault()?.OrganizationId,
                    OrganizationName = user.UserOrganizations?.FirstOrDefault()?.Organization?.Name ?? "No Organization",
                }
            };
        }


        public async Task<AuthResponse> Register(RegisterUserDto registerDto)
        {
            // Validar tipo de usuario
            var userType = await context.UserTypes.FindAsync(registerDto.UserTypeId);
            if (userType == null)
                throw new ArgumentException("Invalid user type");

            // Validar organización si es requerida
            if (userType.RequiresOrganization == true)
            {
                if (registerDto.OrganizationId == null)
                    throw new ArgumentException("Organization is required for this user type");
            }
            else
            {
                registerDto.OrganizationId = null;
            }
            // Verificar si el usuario ya existe
            if (await UserExists(registerDto.Username))
                throw new ArgumentException("Username is already taken");

            if (await EmailExists(registerDto.Email))
                throw new ArgumentException("Email is already registered");

            // Crear hash de la contraseña
            var password = passwordHash.HashPassword(registerDto.Password);

            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = password,
                Phone = registerDto.Phone,
                UserTypeId = registerDto.UserTypeId,
                RegistrationDate = DateTime.UtcNow,
                IsActive = true
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            await SendWelcomeEmail(user);
            // Generar token
            var token = tokenService.CreateToken(user);

            return new AuthResponse
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddDays(7),
                User = new UserDto
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    UserType = userType.TypeName,
                    RegistrationDate = DateTime.UtcNow,
                    OrganizationId = registerDto.OrganizationId
                }
            };
        }

        private async Task SendWelcomeEmail(User user)
        {
            try
            {

                // Reemplazar placeholders (eliminando UrlActivacion)

                var parameters = new Dictionary<string, string>
            {
                {"NombreUsuario", user.Username ?? "Usuario"},
                {"NombreEmpresa", configuration["Company:Name"] ?? "Nuestra Empresa"},
                {"Anio", DateTime.Now.Year.ToString()},
                {"Email", user.Email},
                {"UrlPrivacidad", configuration["Company:PrivacyUrl"] ?? "#"},
                {"UrlTerminos", configuration["Company:TermsUrl"] ?? "#"},
                {"UrlContacto", configuration["Company:ContactUrl"] ?? "#"}
            };

                var htmlContent = await templateService.RenderTemplateAsync(
                   "WelcomeEmail",
                   parameters);


                var emailRequest = new EmailRequest(
              ToEmail: user.Email,
              ToName: user.Username ?? user.Email,
              Subject: $"{parameters["NombreEmpresa"]} - Bienvenido",
              HtmlContent: htmlContent);

                await emailService.SendEmailAsync(emailRequest);
            }
            catch (Exception ex)
            {
                errorLoggerService.LogError(
                    source: "EmailService",
                    ex: ex,
                    userId: user.UserId,
                    email: user.Email,
                    additionalData: new
                    {
                        Template = "WelcomeEmail",
                        UserRegistrationDate = user.RegistrationDate
                    }
                );
            }
        }

        private async Task<bool> UserExists(string? username)
        {
            return await context.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower());
        }
        private async Task<bool> EmailExists(string? email)
        {
            return await context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<bool> Logout(string token)
        {
            return await tokenService.InvalidateToken(token);
        }
    }
}
