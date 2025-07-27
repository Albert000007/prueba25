using Baylongo.Data.Data.MsSql.Models.DBBaylongo;
using BaylongoApi.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BaylongoApi.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly IMemoryCache _cache; // Para almacenar tokens invalidados
        public TokenService(IConfiguration config, IMemoryCache cache)
        {
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            _tokenHandler = new JwtSecurityTokenHandler();
            _cache = cache;
        }

        public string CreateToken(User user)
        {
            // Crear claims (información del usuario)
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.NameId, user.UserId.ToString()),
                new(JwtRegisteredClaimNames.UniqueName, user.Username),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(ClaimTypes.Role, user.UserTypeId.ToString()) // Rol basado en UserTypeId
            };

            // Credenciales de firma
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // Descriptor del token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(Convert.ToDouble(_config["Jwt:ExpireDays"])),
                SigningCredentials = creds,
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"]
            };

            // Generar token
            var token = _tokenHandler.CreateToken(tokenDescriptor);

            return _tokenHandler.WriteToken(token);
        }

        public async Task<bool> InvalidateToken(string token)
        {
            try
            {
                var principal = ValidateToken(token);
                if (principal == null) return false;

                // Obtener tiempo de expiración del token
                var expiryDate = principal.Claims
                    .FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value;

                if (string.IsNullOrEmpty(expiryDate)) return false;

                var expiryDateTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expiryDate)).DateTime;

                // Almacenar token en cache hasta su expiración
                _cache.Set($"invalidated_token_{token}", true, expiryDateTime);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsTokenValid(string token)
        {
            // Verifica si el token NO está en la lista negra
            return !_cache.TryGetValue($"invalidated_token_{token}", out _);
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            // Primero verificar si el token está en la lista negra
            if (_cache.TryGetValue($"invalidated_token_{token}", out _))
            {
                return null;
            }

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _key,
                ValidateIssuer = true,
                ValidIssuer = _config["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _config["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                return _tokenHandler.ValidateToken(token, validationParameters, out _);
            }
            catch
            {
                return null;
            }
        }
    }
}
