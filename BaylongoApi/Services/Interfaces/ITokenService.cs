using Baylongo.Data.Data.MsSql.Models.DBBaylongo;
using System.Security.Claims;

namespace BaylongoApi.Services.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
        ClaimsPrincipal ValidateToken(string token);
        Task<bool> InvalidateToken(string token); // Nuevo método
        Task<bool> IsTokenValid(string token); // Nuevo método
    }
}
