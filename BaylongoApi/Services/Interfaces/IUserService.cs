using BaylongoApi.DTOs.Users;

namespace BaylongoApi.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetUserById(int id);
        Task<IEnumerable<UserDto>> GetAllUsers();
        //Task<UserDto> UpdateUser(int id, UpdateUserDto updateUserDto);
        Task<bool> DeleteUser(int id, int? requestingUserId = null);
        Task<PasswordResetResult> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string code, string newPassword);
        Task<bool> VerifyResetCodeAsync(string email, string code);
        Task<AuthResponse> GetCurrentUserAsync(int userId);
    }
}
