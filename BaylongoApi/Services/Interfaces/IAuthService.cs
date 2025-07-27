using BaylongoApi.DTOs.Users;

namespace BaylongoApi.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> Register(RegisterUserDto registerDto);
        Task<AuthResponse> Login(LoginDto loginDto);
        Task<bool> Logout(string token);
    }
}
