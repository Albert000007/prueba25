using BaylongoApi.DTOs.Users;
using BaylongoApi.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace BaylongoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUserService userService, IAuthService authService, ITokenService tokenService) : ControllerBase
    {
        [HttpGet("login-google")]
        public IActionResult Google(string returnUrl = "/")
        {
            var properties = new AuthenticationProperties { RedirectUri = returnUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// Autentica a un usuario en el sistema.
        /// </summary>
        /// <param name="loginDto">Objeto que contiene las credenciales del usuario (correo electrónico y contraseña).</param>
        /// <returns>
        /// Retorna un objeto <see cref="AuthResponse"/> con el token de autenticación y la información del usuario si las credenciales son válidas.
        /// </returns>
        /// <response code="200">Autenticación exitosa. Retorna el token y los datos del usuario.</response>
        /// <response code="401">Credenciales inválidas. El usuario no está autorizado.</response>
        /// <response code="400">Error en la solicitud. Puede deberse a datos faltantes o un problema inesperado.</response>
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginDto loginDto)
        {
            try
            {
                var response = await authService.Login(loginDto);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader))
                return BadRequest("Authorization header is missing");

            if (!AuthenticationHeaderValue.TryParse(authHeader, out var headerValue))
                return BadRequest("Invalid Authorization header format");

            if (!string.Equals(headerValue.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Invalid scheme");

            var token = headerValue.Parameter;
            if (string.IsNullOrEmpty(token))
                return BadRequest("Token is empty");

            var result = await tokenService.InvalidateToken(token);

            // Opción 1: Uniendo los valores
            //Response.Headers.Append("Clear-Site-Data", string.Join(", ", new[] { "\"cache\"", "\"cookies\"" }));


            return result
                ? Ok(new { success = true, message = "Logout successful" })
                : BadRequest("Failed to invalidate token");
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(RegisterUserDto registerDto)
        {
            try
            {
                var response = await authService.Register(registerDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
 
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var result = await userService.ForgotPasswordAsync(request.Email);
            return Ok(new { success = result });
        }

        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await userService.ResetPasswordAsync(
                request.Email,
                request.Code,
                request.NewPassword);

            return Ok(new { success = result });
        }

        [HttpPost("verify-code")]
        public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeRequest request)
        {
            var isValid = await userService.VerifyResetCodeAsync(request.Email, request.Code);
            return Ok(new { valid = isValid });
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            try
            {
                var user = await userService.GetUserById(id);
                return Ok(user);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [Authorize] // Solo admin
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id, int? requestingUserId = null)
        {
            try
            {
                await userService.DeleteUser(id, requestingUserId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Retorna la información del usuario autenticado usando el token.
        /// </summary>
        [Authorize]
        [HttpGet("curren-user")]
        public async Task<ActionResult<AuthResponse>> GetCurrentUser()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized("No user ID claim found in token");

            if (!int.TryParse(userIdClaim.Value, out int userId))
                return BadRequest("Invalid user ID claim");

            var response = await userService.GetCurrentUserAsync(userId);
            return Ok(response);
        }
    }
}
