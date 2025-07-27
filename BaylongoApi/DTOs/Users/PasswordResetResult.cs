namespace BaylongoApi.DTOs.Users
{
    public record PasswordResetResult(bool Success, string? ErrorMessage = null);
}
